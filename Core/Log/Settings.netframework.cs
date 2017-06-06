#if !NETSTANDARD16
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using static Adenson.Log.SettingsConfiguration;

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
						handler = element.GetValue("type", null) == "flat" 
							? new DatabaseHandler(element.GetValue("connection", "Logger"), element.GetValue("tableName", "EventLog"), element.GetValue("messageColumn", "Message")) 
							: new DatabaseHandler(element.GetValue("connection", "Logger"), element.GetValue("tableName", "EventLog"), element.GetValue("severityColumn", "Severity"), element.GetValue("dateColumn", "Date"), element.GetValue("typeColumn", "Type"), element.GetValue("messageColumn", "Message"));
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
							throw new ConfigurationErrorsException(Exceptions.CustomHandlerTypeTypeMustBeSpecified);
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

		#endregion
	}
}
#endif