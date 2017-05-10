#if !NETSTANDARD16
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	public sealed partial class Settings
	{
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
			Settings settings = new Settings();
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
						HandlerElement e = new HandlerElement();
						r.Add(e);
						HandlerType? handlerType = null;
						#if NET35
						try
						{
							handlerType = (HandlerType)Enum.Parse(typeof(HandlerType), iv[0]);
						}
						catch
						{
						}
						#else
						HandlerType t;
						if (Enum.TryParse<HandlerType>(iv[0], out t))
						{
							handlerType = t;
						}
						#endif
						if (handlerType != null)
						{
							e.Handler = handlerType.Value;
							if (handlerType == HandlerType.Custom)
							{
								e.CustomType = iv.Length > 1 ? String.Join(",", iv.Skip(1).ToArray()) : null;
							}
						}
						else
						{
							e.Handler = HandlerType.Custom;
							e.CustomType = v;
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
		/// <returns>Created settings object if any, null otherwise.</returns>
		public static Settings FromConfigSection()
		{
			SettingsConfiguration config = (ConfigurationManager.GetSection("adenson/logSettings") ?? ConfigurationManager.GetSection("logSettings")) as SettingsConfiguration;
			Settings settings = new Settings();
			settings.Severity = config.Severity;
			settings.SecondsFormat = config.SecondsFormat;
			settings.Formatter = String.IsNullOrEmpty(config.Formatter) ? new DefaultFormatter() : TypeUtil.CreateInstance<BaseFormatter>(config.Formatter);
			Settings.LoadHandlers(settings, config.Handlers.Cast<HandlerElement>());
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
						handler = new DatabaseHandler(element.GetValue("connection", "Logger"), element.GetValue("tableName", "EventLog"), element.GetValue("severityColumn", "Severity"), element.GetValue("dateColumn", "Date"), element.GetValue("typeColumn", "Type"), element.GetValue("messageColumn", "Message"));
						break;
					case HandlerType.Email:
						handler = new EmailHandler(element.GetValue("From", "logger@devnull"), element.GetValue("To", null), element.GetValue("Subject", "Adenson.Log.Logger"));
						break;
					case HandlerType.EventLog:
						handler = new EventLogHandler(element.GetValue("source", "Application"));
						break;
					case HandlerType.File:
						handler = new FileHandler(element.GetValue("fileName", "eventlogger.log"));
						break;
					case HandlerType.Trace:
						handler = new TraceHandler();
						break;
					case HandlerType.Custom:
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

		#endregion
		#region Inner Classes

		/// <summary>
		/// Represents the <see cref="Logger"/> default settings.
		/// </summary>
		internal sealed class SettingsConfiguration : ConfigurationSection
		{
			#region Properties

			/// <summary>
			/// Gets the format name.
			/// </summary>
			[ConfigurationProperty("formatter")]
			public string Formatter
			{
				get { return (string)base["formatter"]; }
				internal set { this["formatter"] = value; }
			}

			/// <summary>
			/// Gets the different handlers the logger uses.
			/// </summary>
			[ConfigurationProperty("handlers", IsDefaultCollection = false)]
			public HandlerElementCollection Handlers
			{
				get { return (HandlerElementCollection)base["handlers"]; }
				internal set { this["handlers"] = value; }
			}

			/// <summary>
			/// Gets the format name.
			/// </summary>
			[ConfigurationProperty("secondsFormat", DefaultValue = "0.00")]
			public string SecondsFormat
			{
				get { return (string)base["secondsFormat"]; }
				internal set { this["secondsFormat"] = value; }
			}

			/// <summary>
			/// Gets the severity.
			/// </summary>
			[ConfigurationProperty("severity", DefaultValue = Severity.Error)]
			public Severity Severity
			{
				get { return (Severity)base["severity"]; }
				internal set { this["severity"] = value; }
			}

			#endregion
		}

		/// <summary>
		/// Represents the api config element.
		/// </summary>
		internal class HandlerElement : ConfigurationElement
		{
			#region Properties

			/// <summary>
			/// Gets or sets the handler.
			/// </summary>
			[ConfigurationProperty("handler", IsRequired = true, IsKey = true)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public HandlerType Handler
			{
				get { return (HandlerType)this["handler"]; }
				internal set { this["handler"] = value; }
			}

			/// <summary>
			/// Gets or sets the formatter.
			/// </summary>
			[ConfigurationProperty("formatter", IsRequired = false, IsKey = false)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public string Formatter
			{
				get { return (string)this["formatter"]; }
				internal set { this["formatter"] = value; }
			}

			/// <summary>
			/// Gets or sets the severity.
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
				get
				{
					string type = (string)this["customType"];
					if (this.Handler == HandlerType.Custom && String.IsNullOrEmpty(type))
					{
						throw new ConfigurationErrorsException(Exceptions.CustomHandlerTypeTypeMustBeSpecified);
					}

					return type;
				}
				internal set
				{
					this["customType"] = value;
				}
			}

			#endregion
			#region Methods

			internal string GetValue(string key, string defaultValue)
			{
				ConfigurationProperty prop = this.Properties.Cast<ConfigurationProperty>().FirstOrDefault(p => String.Equals(p.Name, key, StringComparison.CurrentCultureIgnoreCase));
				return prop == null ? defaultValue : (string)prop.DefaultValue;
			}

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
		internal sealed class HandlerElementCollection : ConfigurationElementCollection
		{
			#region Constructor

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

		#endregion
	}
}
#endif