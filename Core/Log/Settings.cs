using System;
using System.Collections.Generic;
#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_3 && !NETSTANDARD1_0
using System.Configuration;
#endif
using System.Linq;
#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_3 && !NETSTANDARD1_0
using static Adenson.Log.SettingsConfiguration;
#endif

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	public sealed partial class Settings
	{
		#region Variables
		private BaseFormatter _formatter;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Settings"/> class.
		/// </summary>
		public Settings()
		{
			this.Handlers = new HandlerCollection(this);
			this.Formatter = new DefaultFormatter();
			this.Severity = Severity.Error;
			this.Round = 2;
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
		/// Gets or sets the formatting to use for profiler time measurement, defaults to 4.
		/// </summary>
		public int Round
		{
			get;
			set;
		}

		#endregion
		#region Methods

#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_3 && !NETSTANDARD1_0

		/// <summary>
		/// Reads values from the app settings and returns a <see cref="Settings"/> object if any, null otherwise.
		/// </summary>
		/// <returns>Created settings object.</returns>
		/// <remarks>
		/// <para>Not supported in .NET 3.5.</para>
		/// <code>
		/// Takes the form:
		/// &lt;appSettings&gt;
		/// \t&lt;add key="logger:formatter" value="AFormatterTypeFullName" /&gt;
		/// \t&lt;add key="logger:secondsFormat" value="0.0" /&gt;
		/// \t&lt;add key="logger:severity" value="Info" /&gt;
		/// \t&lt;add key="logger:handlers" value="Console;Debug;Custom:ACustomTypeFullName;AnotherCustomTypeFullNameTreatedAsCustom" /&gt;
		/// &lt;/appSettings&gt;
		/// </code>
		/// <para>
		/// For handlers, only supports either <see cref="HandlerType"/> OR a full type that implements <see cref="BaseHandler"/>. In the example, Custom:ACustomTypeFullName and AnotherCustomTypeFullNameTreatedAsCustom
		/// are both treated as <see cref="HandlerType.Custom"/>
		/// </para>
		/// </remarks>
		public static Settings FromAppSettings()
		{
			Settings settings = new Settings();
			settings.Handlers.Add(new TraceHandler());
			string[] loggerKeys = ConfigurationManager.AppSettings.Keys.Cast<string>().Where(k => k.StartsWith("logSettings:", StringComparison.CurrentCultureIgnoreCase) || k.StartsWith("adenson:logSettings:", StringComparison.CurrentCultureIgnoreCase)).ToArray();
			foreach (string k in loggerKeys)
			{
				string key = k.Split(':').Last();
				string value = ConfigurationManager.AppSettings[k];
				if (String.Equals(key, "handlers", StringComparison.CurrentCultureIgnoreCase))
				{
					List<HandlerElement> r = new List<HandlerElement>();
					string[] values = value.Split(';').Select(s => s.Trim()).Where(s => !String.IsNullOrEmpty(s)).Distinct().ToArray();
					foreach (string v in values)
					{
						string[] iv = v.Split(':');
						HandlerElement element = new HandlerElement();
						r.Add(element);
						HandlerType? handlerType = null;
						if (Enum.TryParse(iv[0], out HandlerType t))
						{
							handlerType = t;
						}

						if (handlerType != null)
						{
							element.Handler = handlerType.Value;
							if (handlerType == HandlerType.Custom)
							{
								element.CustomType = iv.Length > 1 ? String.Join(",", iv.Skip(1).ToArray()) : null;
							}
						}
						else
						{
							element.Handler = HandlerType.Custom;
							element.CustomType = v;
						}
					}

					HandlerElement.Load(settings, r);
				}
				else
				{
					switch (key)
					{
						case "formatter":
							settings.Formatter = TypeUtil.CreateInstance<BaseFormatter>(value);
							break;
						case "round":
							settings.Round = Int32.Parse(value);
							break;
						case "severity":
							settings.Severity = (Severity)Enum.Parse(typeof(Severity), value);
							break;
					}
				}
			}

			return settings;
		}

		/// <summary>
		/// Instantiates a new instance of the <see cref="Settings"/> class from the config object. If config is null, creates a default settings object.
		/// </summary>
		/// <param name="sectionName">The name of the section.</param>
		/// <returns>Created settings object if any, null otherwise.</returns>
		public static Settings FromConfigSection(string sectionName)
		{
			Settings settings = null;
			if (ConfigurationManager.GetSection(sectionName) is SettingsConfiguration config)
			{
				settings = new Settings
				{
					Severity = config.Severity,
					Round = config.Round,
					Formatter = String.IsNullOrEmpty(config.Formatter) ? new DefaultFormatter() : TypeUtil.CreateInstance<BaseFormatter>(config.Formatter),
				};
				HandlerElement.Load(settings, config.Handlers.Cast<HandlerElement>());
			}

			return settings;
		}

#endif

		#endregion
	}
}