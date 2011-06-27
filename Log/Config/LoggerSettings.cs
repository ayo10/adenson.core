using System;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log.Config
{
	internal sealed class LoggerSettings : XElementSettingBase
	{
		#region Variables
		internal static LoggerSettings Default = ReadSettings();
		#endregion
		#region Constructor

		public LoggerSettings(XElement element) : base(element)
		{
			this.BatchSize = this.GetValue("BatchSize", default(short));
			this.Severity = this.GetValue("Severity", LogSeverity.Error);
			this.Source = this.GetValue("Source", "Logger");
			this.DateTimeFormat = this.GetValue("DateTimeFormat", "HH:mm:ss:fff");
			this.FileName = this.GetValue("FileName", "eventlogger.log");

			this.EmailInfo = new LoggerSettingEmailInfo(element == null ? null : element.Element("EmailInfo", StringComparison.OrdinalIgnoreCase));
			this.DatabaseInfo = new LoggerSettingDatabaseInfo(element == null ? null : element.Element("DatabaseInfo", StringComparison.OrdinalIgnoreCase));

			this.Types = this.GetValue("Types", LogTypes.Trace);
		}

		#endregion
		#region Properties

		public short BatchSize
		{
			get;
			set;
		}
		public LogSeverity Severity
		{
			get;
			set;
		}
		public LogTypes Types
		{
			get;
			set;
		}
		public string Source
		{
			get;
			set;
		}
		public string DateTimeFormat
		{
			get;
			set;
		}
		public string FileName
		{
			get;
			set;
		}
		public LoggerSettingDatabaseInfo DatabaseInfo
		{
			get;
			set;
		}
		public LoggerSettingEmailInfo EmailInfo
		{
			get;
			set;
		}

		#endregion
		#region Methods

		private static LoggerSettings ReadSettings()
		{
			var section = ConfigHelper.GetSection<XDocument>("adenson", "loggerSettings");
			return new LoggerSettings((section != null) ? section.Root : null);
		}

		#endregion
	}
}