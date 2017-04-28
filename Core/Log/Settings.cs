using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	public sealed class Settings
	{
		#region Variables
		private BaseFormatter _formatter;
		#endregion
		#region Constructor

		internal Settings()
		{
			this.Formatter = new DefaultFormatter();
			this.Severity = Severity.Error;
			this.Handlers = new HandlerCollection(this);
			this.Handlers.Add(new TraceHandler());
			this.SecondsFormat = "N3";
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the format name.
		/// </summary>
		public BaseFormatter Formatter
		{
			get
			{
				return _formatter; 
			}
			set
			{
				Arg.IsNotNull(value, "value");
				_formatter = value;
			}
		}

		/// <summary>
		/// Gets the different handlers the logger uses.
		/// </summary>
		public HandlerCollection Handlers
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the severity.
		/// </summary>
		public Severity Severity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the formatting to use for profiler seconds measurement, defaults to '0.000000'.
		/// </summary>
		public string SecondsFormat
		{
			get;
			set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Reads values from the app settings and returns a <see cref="Settings"/> object if any, null otherwise.
		/// </summary>
		/// <returns>Created settings object.</returns>
		/// <remarks>
		/// <para>Not supported in .NET 3.5.</para>
		/// <para>
		///	Takes the form:
		///	&lt;appSettings&gt;
		///		&lt;add key="logger:formatter" value="AFormatterTypeFullName" /&gt;
		///		&lt;add key="logger:secondsFormat" value="0.0" /&gt;
		///		&lt;add key="logger:severity" value="Info" /&gt;
		///		&lt;add key="logger:handlers" value="Console;Debug;Custom:ACustomTypeFullName;AnotherCustomTypeFullNameTreatedAsCustom" /&gt;
		///	&lt;/appSettings&gt;
		/// </para>
		/// <para>
		/// For handlers, only supports either <see cref="HandlerType"/> OR a full type that implements <see cref="BaseHandler"/>. In the example, Custom:ACustomTypeFullName and AnotherCustomTypeFullNameTreatedAsCustom
		/// are both treated as <see cref="HandlerType.Custom"/>
		/// </para>
		/// </remarks>
		public static Settings FromAppSettings()
		{
			#if !NET35
			string[] loggerKeys = ConfigurationManager.AppSettings.Keys.Cast<string>().Where(k => k.StartsWith("logSettings:", StringComparison.CurrentCultureIgnoreCase)).ToArray();
			if (loggerKeys.Length > 0)
			{
				SettingsConfiguration sc = new SettingsConfiguration();
				foreach (string k in loggerKeys)
				{
					switch (k.Split(':')[1].ToLower())
					{
						case "formatter":
							sc.Formatter = ConfigurationManager.AppSettings[k];
							break;
						case "secondsformat":
							sc.SecondsFormat = ConfigurationManager.AppSettings[k];
							break;
						case "severity":
							sc.Severity = (Severity)Enum.Parse(typeof(Severity), ConfigurationManager.AppSettings[k]);
							break;
						case "handlers":
							sc.Handlers = SettingsConfiguration.HandlerElementCollection.FromString(ConfigurationManager.AppSettings[k]);
							break;
					}
				}

				return Settings.FromConfig(sc);
			}
			#endif
			
			return null;
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="Settings"/> class from the config object. If config is null, creates a default settings object.
		/// </summary>
		/// <returns>Created settings object if any, null otherwise.</returns>
		public static Settings FromConfigSection()
		{
			SettingsConfiguration config = (ConfigurationManager.GetSection("adenson/logSettings") ?? ConfigurationManager.GetSection("logSettings")) as SettingsConfiguration;
			return config == null ? new Settings() : Settings.FromConfig(config);
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="Settings"/> class from the config object. If config is null, creates a default settings object.
		/// </summary>
		/// <param name="section">The configuration object.</param>
		/// <returns>Created settings object.</returns>
		[Obsolete("No longer supported or used.", false)]
		public static Settings FromConfig(ConfigurationSection section)
		{
			SettingsConfiguration config = section as SettingsConfiguration;
			Settings settings = new Settings();
			return config == null ? new Settings() : Settings.FromConfig(config);
		}

		private static Settings FromConfig(SettingsConfiguration config)
		{
			Settings settings = new Settings();
			settings.Severity = config.Severity;
			settings.SecondsFormat = config.SecondsFormat;
			settings.Formatter = String.IsNullOrEmpty(config.Formatter) ? new DefaultFormatter() : TypeUtil.CreateInstance<BaseFormatter>(config.Formatter);
			settings.Handlers = HandlerCollection.FromConfig(settings, config.Handlers);
			return settings;
		}

		#endregion
	}
}