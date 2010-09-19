using System;
using System.Xml.Serialization;
using Adenson.Log;

namespace Adenson.Configuration
{
	[XmlType(TypeName = "loggerSettings")]
	public sealed class LoggerSettings
	{
		#region Variables
		private LogType? _type;
		#endregion
		#region Constructor

		public LoggerSettings()
		{
			#if DEBUG
			this.Severity = LogSeverity.Debug;
			#else
			this.Severity = LogSeverity.Error;
			#endif
			this.Type = "Debug";
			this.Source = "Logger";
			this.DateTimeFormat = "HH:mm:ss:fff";
			this.Email = new EmailInfo { From = "errors@adenson.com" };
			this.FileName = SR.EventLogFile;
		}

		#endregion
		#region Properties

		[XmlAttribute(AttributeName = "severity")]
		public LogSeverity Severity { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "batchSize")]
		public ushort BatchSize { get; set; }

		[XmlAttribute(AttributeName = "source")]
		public string Source { get; set; }

		[XmlAttribute(AttributeName = "dateTimeFormat")]
		public string DateTimeFormat { get; set; }

		[XmlAttribute(AttributeName = "fileName")]
		public string FileName { get; set; }

		[XmlElement(ElementName = "email")]
		public EmailInfo Email { get; set; }

		public LogType TypeActual
		{
			get
			{
				if (_type == null)
				{
					int result = 0;
					try
					{
						if (!Int32.TryParse(this.Type, out result))
						{
							var splits = this.Type.Split(',', '|');
							foreach (var str in splits)
							{
								LogType t = (LogType)Enum.Parse(typeof(LogType), str.Trim());
								result += (int)t;
							}
						}
					}
					catch
					{
						result = (int)LogType.Debug;
					}
					_type = (LogType)result;
				}
				return _type.Value;
			}
		}

		#endregion
		#region Inner Classes

		public sealed class EmailInfo
		{
			[XmlAttribute(AttributeName = "from")]
			public string From { get; set; }
			[XmlAttribute(AttributeName = "to")]
			public string To { get; set; }
		}

		#endregion
	}
}