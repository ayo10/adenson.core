using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log.Config
{
	internal sealed class LoggerSettings : XElementSettingBase
	{
		#region Constructor

		public LoggerSettings(XElement element) : base(element)
		{
			this.Severity = this.GetValue("Severity", LogSeverity.Error);
			this.Source = this.GetValue("Source", "Logger");
			this.DateTimeFormat = this.GetValue("DateTimeFormat", "HH:mm:ss:fff");
			this.FileName = this.GetValue("FileName", "eventlogger.log");

			this.EmailInfo = new LoggerSettingEmailInfo(element == null ? null : element.Element("EmailInfo", StringComparison.OrdinalIgnoreCase));
			this.DatabaseInfo = new LoggerSettingDatabaseInfo(element == null ? null : element.Element("DatabaseInfo", StringComparison.OrdinalIgnoreCase));

			var types = this.GetValue("Types", LogTypes.None);
			if (types == LogTypes.None)
			{
				types = this.GetValue("Type", LogTypes.None);
			}

			if (types == LogTypes.None)
			{
				types = LogTypes.Trace;
			}

			this.Types = types;

			if (this.Severity != LogSeverity.None && this.Types != LogTypes.None)
			{
				var listeners = new List<TraceListener>();

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
						try
						{
							var lastWriteTime = File.GetLastWriteTime(filePath);
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
						}
						catch
						{
						}

						listeners.Add(new TextWriterTraceListener(filePath));
					}
					else
					{
						Trace.WriteLine(StringUtil.Format("Adenson.Log.Logger: ERROR: Folder {0} does not exist, file logging will not happen", folder));
					}
				}

				if ((this.Types & LogTypes.EventLog) != LogTypes.None)
				{
					listeners.Add(new EventLogTraceListener());
				}

				foreach (var listener in listeners)
				{
					listener.Filter = new LogFilter();
					Trace.Listeners.Add(listener);
				}
			}
		}

		#endregion
		#region Properties

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

		internal static LoggerSettings ReadSettings()
		{
			var section = ConfigHelper.GetSection<XDocument>("adenson", "loggerSettings");
			return new LoggerSettings((section != null) ? section.Root : null);
		}

		#endregion
	}
}