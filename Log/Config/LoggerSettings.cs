using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log.Config
{
	internal sealed class LoggerSettings : XElementSettingBase
	{
		#region Variables
		private static LoggerSettings _defaultSettings = LoggerSettings.ReadSettings();
		#endregion
		#region Constructor

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Null should be returned, regardless of what exception was thrown during deserialization.")]
		public LoggerSettings(XElement element) : base(element)
		{
			this.Format = this.GetFormat();
			this.Severity = this.GetValue("Severity", LogSeverity.Error);
			this.FileName = this.GetValue("FileName", "eventlogger.log");
			this.Types = this.GetValue("Types", LogTypes.Trace);
			this.EmailInfo = new LoggerSettingEmailInfo(element == null ? null : element.Element("EmailInfo", StringComparison.OrdinalIgnoreCase));
			this.DatabaseInfo = new LoggerSettingDatabaseInfo(element == null ? null : element.Element("DatabaseInfo", StringComparison.OrdinalIgnoreCase));

			if (this.Severity != LogSeverity.None && this.Types != LogTypes.None)
			{
				if ((this.Types & LogTypes.File) != LogTypes.None)
				{
					string filePath = this.FileName;
					string folder = null;
					if (!Path.IsPathRooted(filePath))
					{
						filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath.Replace("/", "\\"));
					}

					folder = Path.GetDirectoryName(filePath);

					if (Directory.Exists(folder))
					{
						DateTime lastWriteTime = File.GetLastWriteTime(filePath);
						if (File.Exists(filePath) && lastWriteTime.Date < DateTime.Now.AddDays(-1))
						{
							string fileName = Path.GetFileNameWithoutExtension(filePath);
							string extension = Path.GetExtension(filePath);
							string oldNewFileName = String.Concat(fileName, lastWriteTime.ToString("yyyyMMdd", CultureInfo.CurrentCulture), extension);
							string oldNewFilePath = Path.Combine(folder, oldNewFileName);
							if (!File.Exists(oldNewFilePath))
							{
								File.Move(fileName, oldNewFilePath);
							}
						}

						TextWriter writer = new StreamWriter(filePath);
						Logger.SetWriter(writer);
					}
					else
					{
						Trace.WriteLine(StringUtil.Format("Adenson.Log.Logger: ERROR: Folder {0} does not exist, file logging will not happen", folder));
					}
				}

				if ((this.Types & LogTypes.EventLog) != LogTypes.None)
				{
					EventLogTraceListener listener = new EventLogTraceListener();
					Debug.Listeners.Add(listener);
					Trace.Listeners.Add(listener);
				}
			}
		}

		#endregion
		#region Properties

		public string Format
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

		internal static LoggerSettings Default
		{
			get { return _defaultSettings; }
		}

		#endregion
		#region Methods

		internal static LoggerSettings ReadSettings()
		{
			XDocument section = ConfigHelper.GetSection<XDocument>("adenson", "loggerSettings");
			return new LoggerSettings((section != null) ? section.Root : null);
		}

		private string GetFormat()
		{
			return this.GetValue("Format", "{Date:H:mm:ss.fff} [{Severity,5}]\t{TypeName:15} {Message}")
									.Replace("{Severity", "{0")
									.Replace("{Date", "{1")
									.Replace("{TypeName", "{2")
									.Replace("{Message", "{3");
		}

		#endregion
	}
}