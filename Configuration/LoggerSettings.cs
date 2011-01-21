using System;
using System.Xml.Serialization;
using Adenson.Log;

namespace Adenson.Configuration
{
	[XmlType(TypeName = "loggerSettings")]
	public sealed class LoggerSettings
	{
		#region Variables
		private LogTypes? _type;
		private LogSeverity? _severity;
		#endregion
		#region Constructor

		public LoggerSettings()
		{
			this.Severity = "Error";
			this.Type = "Debug";
			this.Source = "Logger";
			this.DateTimeFormat = "HH:mm:ss:fff";
			this.EmailInfo = new LoggerSettingEmailInfo { From = "errors@adenson.com" };
			this.FileName = SR.EventLogFile;
		}

		#endregion
		#region Properties

		[XmlAttribute(AttributeName = "severity")]
		public string Severity
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "type")]
		public string Type
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "batchSize")]
		public short BatchSize
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "source")]
		public string Source
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "dateTimeFormat")]
		public string DateTimeFormat
		{
			get;
			set;
		}

		[XmlAttribute(AttributeName = "fileName")]
		public string FileName
		{
			get;
			set;
		}

		[XmlElement(ElementName = "email")]
		public LoggerSettingEmailInfo EmailInfo
		{
			get;
			set;
		}

		internal LogTypes TypeActual
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
								LogTypes t = (LogTypes)Enum.Parse(typeof(LogTypes), str.Trim());
								result += (int)t;
							}
						}
					}
					catch
					{
						result = (int)LogTypes.Debug;
					}
					_type = (LogTypes)result;
				}
				return _type.Value;
			}
		}
		internal LogSeverity SeverityActual
		{
			get
			{
				if (_severity == null)
				{
					try
					{
						_severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), this.Severity);
					}
					catch
					{
						_severity = LogSeverity.Error;
					}
				}
				return _severity.Value;
			}
		}

		#endregion
	}
}