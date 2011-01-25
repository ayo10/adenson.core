﻿using System;
using System.Xml.Linq;
using Adenson.Log;

namespace Adenson.Configuration.Internal
{
	internal sealed class LoggerSettings : XmlSettingsBase
	{
		#region Constructor

		public LoggerSettings(XElement element) : base(element)
		{
			this.BatchSize = this.GetValue("BatchSize", default(short));
			this.Severity = this.GetValue("Severity", LogSeverity.Error);
			this.Source = this.GetValue("Source", "Logger");
			this.DateTimeFormat = this.GetValue("DateTimeFormat", "HH:mm:ss:fff");
			this.FileName = this.GetValue("FileName", "eventlogger.log");
			this.EmailInfo = new LoggerSettingEmailInfo(element.Element("EmailInfo", StringComparison.OrdinalIgnoreCase));
			this.DatabaseInfo = new LoggerSettingDatabaseInfo(element.Element("DatabaseInfo", StringComparison.OrdinalIgnoreCase));
			
			if (element.HasElement("Type", StringComparison.OrdinalIgnoreCase)) this.Types = this.GetValue("Type", LogTypes.Debug);//Backward compatibility
			else this.Types = this.GetValue("Types", LogTypes.Debug);
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
	}
}