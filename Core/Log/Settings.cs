using System;
using System.Collections.Generic;
#if !NETSTANDARD1_6 && !NETSTANDARD1_3 && !NETSTANDARD1_0
using System.Configuration;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

					Settings.LoadHandlers(settings, r);
				}
				else
				{
					switch (key)
					{
						case "formatter":
							settings.Formatter = TypeUtil.CreateInstance<BaseFormatter>(value);
							break;
						case "secondsformat":
							settings.SecondsFormat = value;
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
			SettingsConfiguration config = ConfigurationManager.GetSection(sectionName) as SettingsConfiguration;
			Settings settings = null;
			if (config != null)
			{
				settings = new Settings
				{
					Severity = config.Severity,
					SecondsFormat = config.SecondsFormat,
					Formatter = String.IsNullOrEmpty(config.Formatter) ? new DefaultFormatter() : TypeUtil.CreateInstance<BaseFormatter>(config.Formatter),
				};
				Settings.LoadHandlers(settings, config.Handlers.Cast<HandlerElement>());
			}

			return settings;
		}

		private static void LoadHandlers(Settings settings, IEnumerable<HandlerElement> handlers)
		{
			if (handlers.Any())
			{
				settings.Handlers.Clear();
			}

			foreach (HandlerElement element in handlers)
			{
				BaseHandler handler = null;
				switch (element.Handler)
				{
					case HandlerType.Console:
						handler = new ConsoleHandler();
						break;
					case HandlerType.Debug:
						handler = new DebugHandler();
						break;
					case HandlerType.Database:
						handler = HandlerElement.ParseDbElement(element);
						break;
					case HandlerType.Email:
						handler = new EmailHandler(element.GetValue("From", "logger@devnull"), element.GetValue("To", null), element.GetValue("Subject", "Adenson.Log.Logger"));
						break;
					case HandlerType.EventLog:
						string eventId = element.GetValue("eventid", null);
						handler = new EventLogHandler(element.GetValue("source", ".NET Runtime"), String.IsNullOrEmpty(eventId) ? EventLogHandler.DefaultEventId : Convert.ToInt32(eventId));
						break;
					case HandlerType.File:
						handler = new FileHandler(element.GetValue("fileName", "eventlogger.log"));
						break;
					case HandlerType.Trace:
						handler = new TraceHandler();
						break;
					case HandlerType.Custom:
						if (String.IsNullOrEmpty(element.CustomType))
						{
							throw new ConfigurationErrorsException("Type must be set for custom log handler.");
						}

						handler = TypeUtil.CreateInstance<BaseHandler>(element.CustomType);
						break;
				}

				handler.Severity = element.Severity;
				if (!String.IsNullOrEmpty(element.Formatter))
				{
					handler.Formatter = TypeUtil.CreateInstance<BaseFormatter>(element.Formatter);
				}

				settings.Handlers.Add(handler);
			}
		}
#endif

		#endregion
		#region Inner Classes

#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_3 && !NETSTANDARD1_0
		/// <summary>
		/// Represents the <see cref="Logger"/> default settings.
		/// </summary>
		private sealed class SettingsConfiguration : ConfigurationSection
		{
			#region Properties

			/// <summary>
			/// Gets the format name.
			/// </summary>
			[ConfigurationProperty("formatter")]
			public string Formatter
			{
				get { return (string)this["formatter"]; }
			}

			/// <summary>
			/// Gets the different handlers the logger uses.
			/// </summary>
			[ConfigurationProperty("handlers", IsDefaultCollection = false)]
			public HandlerElementCollection Handlers
			{
				get { return (HandlerElementCollection)this["handlers"]; }
			}

			/// <summary>
			/// Gets the format name.
			/// </summary>
			[ConfigurationProperty("secondsFormat", DefaultValue = "0.00")]
			public string SecondsFormat
			{
				get { return (string)this["secondsFormat"]; }
			}

			/// <summary>
			/// Gets the severity.
			/// </summary>
			[ConfigurationProperty("severity", DefaultValue = Severity.Error)]
			public Severity Severity
			{
				get { return (Severity)this["severity"]; }
			}

			#endregion
		}

		/// <summary>
		/// Represents the api config element.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034", Justification = "Auto initialized, should never be initialized outside of SettingsConfiguration.")]
		private class HandlerElement : ConfigurationElement
		{
			#region Properties

			/// <summary>
			/// Gets the handler.
			/// </summary>
			[ConfigurationProperty("handler", IsRequired = true, IsKey = true)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public HandlerType Handler
			{
				get { return (HandlerType)this["handler"]; }
				internal set { this["handler"] = value; }
			}

			/// <summary>
			/// Gets the formatter.
			/// </summary>
			[ConfigurationProperty("formatter", IsRequired = false, IsKey = false)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public string Formatter
			{
				get { return (string)this["formatter"]; }
				internal set { this["formatter"] = value; }
			}

			/// <summary>
			/// Gets the severity.
			/// </summary>
			[ConfigurationProperty("severity", IsRequired = false, IsKey = false)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public Severity Severity
			{
				get { return (Severity)this["severity"]; }
				internal set { this["severity"] = value; }
			}

			/// <summary>
			/// Gets the custom type.
			/// </summary>
			[ConfigurationProperty("customType", IsRequired = false, IsKey = true)]
			public string CustomType
			{
				get { return (string)this["customType"]; }
				internal set { this["customType"] = value; }
			}

			#endregion
			#region Methods

			internal static BaseHandler ParseDbElement(HandlerElement element)
			{
				Data.SqlHelperBase sqlHelper = (Data.SqlHelperBase)Activator.CreateInstance(
					Arg.IsNotNull(
						TypeUtil.GetType(
							Arg.IsNotNull(element.GetValue("sqlHelper", null),
							"Requires a 'sqlHelper' value."),
							false),
						"The 'sqlHelper' value must be a valid full type name that implements Adenson.Data.SqlHelperBase."),
					element.GetValue("connection", "logger"));
				return new DatabaseHandler(
					sqlHelper,
					element.GetValue("tableName", "EventLog"),
					element.GetValue("dateColumn", "Date"),
					element.GetValue("typeColumn", "Type"),
					element.GetValue("severityColumn", "Severity"),
					element.GetValue("messageColumn", "Message"));
			}

			internal string GetValue(string key, string defaultValue)
			{
				ConfigurationProperty prop = this.Properties.Cast<ConfigurationProperty>().FirstOrDefault(p => String.Equals(p.Name, key, StringComparison.CurrentCultureIgnoreCase));
				return prop == null ? defaultValue : (string)prop.DefaultValue;
			}

			/// <inheritdoc />
			protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
			{
				ConfigurationProperty p = new ConfigurationProperty(name, typeof(string), value);
				this.Properties.Add(p);
				return true;
			}

			#endregion
		}

		/// <summary>
		/// Represents a configuration element for apis.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Yes it was, dummas.")]
		[SuppressMessage("Microsoft.Design", "CA1034", Justification = "Auto initialized, should never be initialized outside of SettingsConfiguration.")]
		[SuppressMessage("Microsoft.Design", "CA1010", Justification = "Auto initialized, should never be initialized outside of SettingsConfiguration.")]
		private sealed class HandlerElementCollection : ConfigurationElementCollection
		{
			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="HandlerElementCollection"/> class.
			/// </summary>
			public HandlerElementCollection()
			{
				this.BaseAdd(new HandlerElement { Severity = Severity.Error, Handler = HandlerType.Trace, Formatter = "Adenson.Log.DefaultFormatter, Adenson.Core" });
			}

			#endregion
			#region Methods

			/// <summary>
			/// Creates a new element
			/// </summary>
			/// <returns>The created element</returns>
			protected override ConfigurationElement CreateNewElement()
			{
				return new HandlerElement();
			}

			/// <summary>
			/// Gets the element key for the <see cref="ConfigurationElement"/> element.
			/// </summary>
			/// <param name="element">The <see cref="ConfigurationElement"/> to return the key for.</param>
			/// <returns>An System.Object that acts as the key for the specified <see cref="ConfigurationElement"/>.</returns>
			protected override object GetElementKey(ConfigurationElement element)
			{
				HandlerElement e = (HandlerElement)element;
				return e.Handler == HandlerType.Custom ? (object)e.CustomType : (object)e.Handler;
			}

			#endregion
		}

#endif
		#endregion
	}
}