using System;
using System.Xml.Serialization;
using Adenson.Log;

namespace Adenson.Configuration
{
	[XmlType(TypeName = "loggerSettings")]
	public sealed class LoggerSettings
	{
		#region Constructor

		public LoggerSettings()
		{
			#if DEBUG
			this.Severity = LogSeverity.Debug;
			#else
			this.Severity = LogSeverity.Error;
			#endif
			this.Type = LogType.None;
			this.Source = "Logger";
			this.DateTimeFormat = "HH:mm:ss:fff";
			this.Email = new EmailInfo { From = "errors@adenson.com" };
		}

		#endregion
		#region Properties

		[XmlAttribute(AttributeName = "severity")]
		public LogSeverity Severity { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public LogType Type { get; set; }

		[XmlAttribute(AttributeName = "batchSize")]
		public ushort BatchSize { get; set; }

		[XmlAttribute(AttributeName = "source")]
		public string Source { get; set; }

		[XmlAttribute(AttributeName = "dateTimeFormat")]
		public string DateTimeFormat { get; set; }

		[XmlElement(ElementName = "email")]
		public EmailInfo Email { get; set; }

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