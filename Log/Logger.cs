using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Adenson.Configuration;
using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Logger of ...., logs
	/// </summary>
	public sealed class Logger : IDisposable
	{
		#region Variables
		private static List<LogEntry> entries = new List<LogEntry>();
		private static ErrorAlertType configErrorAlertType = null;
		private static List<Type> suspendedTypes = new List<Type>();
		private static Dictionary<Type, Logger> staticLoggers = new Dictionary<Type, Logger>();
		private static string configEmailErrorTo;
		private static string configEmailErrorFrom = "errors@adenson.com";

		private static string configSource;
		private static ushort configBatchLogSize = 0;
		private static LogSeverity configSeverityLevel = LogSeverity.Error;
		private static LogType configLogType = LogType.None;
		private static string configDateTimeFormat = "HH:mm:ss:fff";
		private static SqlHelperBase sqlHelper;
		private Type _type;
		private ushort _batchLogSize;
		private ErrorAlertType _errorAlertType;
		private LogType _logType = LogType.None;
		private LogSeverity _severity = LogSeverity.None;
		private string _dateTimeFormat;
		private string _emailErrorFrom;
		private string _emailErrorTo;
		private string _source;
		#endregion
		#region Constructors

		static Logger()
		{
			#if DEBUG
			configLogType = LogType.DiagnosticsDebug;
			configSeverityLevel = LogSeverity.Debug;
			#endif
			Dictionary<string, string> config = ConfigSectionHelper.GetDictionary("logger");
			if (config != null)
			{
				if (config.ContainsKey("logType", StringComparison.CurrentCultureIgnoreCase))
				{
					string logType = config.GetValue("logType");
					if (logType.IndexOf("|") > 0)
					{
						string[] splits = logType.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string str in splits)
						{
							configLogType |= (LogType)Enum.Parse(typeof(LogType), str.Trim());
						}
					}
					else configLogType = (LogType)Enum.Parse(typeof(LogType), logType);
				}

				if (config.ContainsKey("severity", StringComparison.CurrentCultureIgnoreCase)) configSeverityLevel = (LogSeverity)Enum.Parse(typeof(LogSeverity), config.GetValue("severity"), true);
				if (config.ContainsKey("emailErrorTo", StringComparison.CurrentCultureIgnoreCase)) configEmailErrorTo = config.GetValue("emailErrorTo");
				if (config.ContainsKey("emailErrorFrom", StringComparison.CurrentCultureIgnoreCase)) configEmailErrorFrom = config.GetValue("emailErrorFrom");
				if (config.ContainsKey("batchSize", StringComparison.CurrentCultureIgnoreCase)) configBatchLogSize = Convert.ToUInt16(config.GetValue("batchSize"));
				if (config.ContainsKey("errorAlertType", StringComparison.CurrentCultureIgnoreCase)) configErrorAlertType = ErrorAlertType.Parse(config.GetValue("errorAlertType"));
				if (config.ContainsKey("source", StringComparison.CurrentCultureIgnoreCase)) configSource = config.GetValue("source");
				if (config.ContainsKey("dateTimeFormat", StringComparison.CurrentCultureIgnoreCase)) configDateTimeFormat = config.GetValue<string>("datetimeformat");
			}
			else
			{
				#if DEBUG
				configSeverityLevel = LogSeverity.Debug;
				#else
				configSeverityLevel = LogSeverity.Error;
				#endif
				configLogType = LogType.ConsoleProjects;
			}

			if ((configLogType & LogType.EventLog) != LogType.None && String.IsNullOrEmpty(configSource)) configSource = "SnUtilsLogger";
			if ((configLogType & LogType.DataBase) != 0) sqlHelper = SqlHelperProvider.Create(ConnectionStrings.Get("Logger", true));
		}

		/// <summary>	
		/// Creates a new instance of Logger from type
		/// </summary>
		/// <param name="type">Type to use to create new instance</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type type)
		{
			if (type == null) throw new ArgumentNullException(ExceptionMessages.ArgumentNull);
			_type = type;
		}
		/// <summary>	
		/// Creates a new instance of Logger from type
		/// </summary>
		/// <param name="type">Type to use to create new instance</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type type, string source) : this(type, LogType.None, source)
		{
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="type">Type to use to create new instance</param>
		/// <param name="logType">The log type</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type type, LogType logType) : this(type)
		{
			if (type == null) throw new ArgumentNullException(ExceptionMessages.ArgumentNull);
			_type = type;
			_logType = logType;
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="type">Type to use to create new instance</param>
		/// <param name="logType">The log type</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		/// <exception cref="ArgumentNullException">if source is null and logType includes EventLog</exception>
		public Logger(Type type, LogType logType, string source) : this(type)
		{
			if ((logType & LogType.EventLog) != LogType.None && String.IsNullOrEmpty(source)) throw new ArgumentNullException("source", ExceptionMessages.EventLogTypeWithSourceNull);
			_logType = logType;
			_source = source;
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="type">Type to use to create new instance</param>
		/// <param name="instance"></param>
		public Logger(Type type, Logger instance) : this(type)
		{
			if (instance == null) throw new ArgumentNullException("instance", ExceptionMessages.ArgumentNull);
			this.BatchSize = instance.BatchSize;
			this.Severity = instance.Severity;
			this.LogType = instance.LogType;
			this.EmailErrorFrom = instance.EmailErrorFrom;
			this.EmailErrorTo = instance.EmailErrorTo;
			this.ErrorAlertType = instance.ErrorAlertType;
			this.Source = instance.Source;
		}
		/// <summary>
		/// Clean up when logger is destroyed
		/// </summary>
		~Logger()
		{
			Logger.Flush(this.LogType);
			lock (staticLoggers)
			{
				if (staticLoggers.ContainsKey(this.Type)) staticLoggers.Remove(this.Type);
			}
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the number of logs thats kept in memory before they are dumped into the database, defaults to 10
		/// </summary>
		public ushort BatchSize
		{
			get { return _batchLogSize == 0 ? configBatchLogSize : _batchLogSize; }
			set
			{
				if (value < 1) throw new ArgumentException("value", SR.MsgExMinLogBatchSize);
				_batchLogSize = value; 
			}
		}
		/// <summary>
		/// Gets the severity level that is logged.
		/// </summary>
		public LogSeverity Severity
		{
			get { return _severity == LogSeverity.None ? configSeverityLevel : _severity; }
			set { _severity = value; }
		}
		/// <summary>
		/// Gets the logging type
		/// </summary>
		public LogType LogType
		{
			get { return _logType == LogType.None ? configLogType : _logType; }
			set { _logType = value; }
		}
		/// <summary>
		/// Gets the errors email from (if LogError(Type, Exception, true/false, true) OR its non static version is called)
		/// </summary>
		public string EmailErrorFrom
		{
			get { return String.IsNullOrEmpty(_emailErrorFrom) ? configEmailErrorFrom : _emailErrorFrom; }
			set { _emailErrorFrom = value; }
		}
		/// <summary>
		/// Gets the errors email recipient (if LogError(Type, Exception, true/false, true) OR its non static version is called)
		/// </summary>
		public string EmailErrorTo
		{
			get { return String.IsNullOrEmpty(_emailErrorTo) ? configEmailErrorTo : _emailErrorTo; }
			set { _emailErrorTo = value; }
		}
		/// <summary>
		/// Gets or sets if to show a message box for Error severity
		/// </summary>
		public ErrorAlertType ErrorAlertType
		{
			get { return _errorAlertType == null ? configErrorAlertType : _errorAlertType; }
			set { _errorAlertType = value; }
		}
		/// <summary>
		/// The type form which this instance is forged from
		/// </summary>
		public Type Type
		{
			get { return _type; }
		}
		/// <summary>
		/// Gets the string tht will be used as source for Window's Event Log
		/// </summary>
		public string Source
		{
			get { return String.IsNullOrEmpty(_source) ? configSource : _source; }
			internal set { _source = value; }
		}
		/// <summary>
		/// Gets the string tht will be used as source for Window's Event Log
		/// </summary>
		public string DateTimeFormat
		{
			get { return (String.IsNullOrEmpty(this._dateTimeFormat) ? configDateTimeFormat : this._dateTimeFormat); }
			set { this._dateTimeFormat = value; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Logs the value into the log of type info, converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void Info(object value)
		{
			this.Info(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Info
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void Info(string message, params object[] arguments)
		{
			if (Convert.ToInt32(this.Severity) > Convert.ToInt32(LogSeverity.Info)) return;
			this.Write(LogSeverity.Info, arguments == null ? message : String.Format(message, arguments));
		}
		/// <summary>
		/// Logs the value into the log of type debug, converting value to string
		/// </summary>>
		/// <param name="value">The value</param>
		public void Debug(object value)
		{
			this.Debug(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Debug
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void Debug(string message, params object[] arguments)
		{
			if (Convert.ToInt32(this.Severity) > Convert.ToInt32(LogSeverity.Debug)) return;
			this.Write(LogSeverity.Debug, arguments == null ? message : String.Format(message, arguments));
		}
		/// <summary>
		/// Called to log errors of type Warning converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void Warn(object value)
		{
			this.Warn(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Warning
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void Warn(string message, params object[] arguments)
		{
			if (Convert.ToInt32(this.Severity) > Convert.ToInt32(LogSeverity.Warn)) return;
			this.Write(LogSeverity.Warn, arguments == null ? message : String.Format(message, arguments));
		}
		/// <summary>
		/// Log the value into the log of type Error, converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void Error(object value)
		{
			this.Error(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Error
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void Error(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, arguments == null ? message : String.Format(message, arguments));
		}
		/// <summary>
		/// Called to log errors of type Error
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		public void Error(Exception ex)
		{
			this.Error(ex, false);
		}
		/// <summary>
		/// Called to log errors of type Error
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		/// <param name="phoneHome">If to send an email to Logger.EmailErrorTo</param>
		public void Error(Exception ex, bool phoneHome)
		{
			string message = Logger.ConvertToString(ex);
			LogEntry entry = this.Write(LogSeverity.Error, message);

			if (ex is System.OutOfMemoryException) Thread.CurrentThread.Abort();
			if (ex is System.Data.Common.DbException && sqlHelper != null) sqlHelper.ClearParameterCache();

			if (phoneHome && this.EmailErrorTo != null)
			{
				this.Flush();
				string body = "System Error:\n\n" + message;
				string subject = "Application Error: " + System.DateTime.Today.ToLongDateString();
				Adenson.Net.Mailer.SendAsync(this.EmailErrorFrom, this.EmailErrorTo, subject, body, false);
			}

			if (this.ErrorAlertType != null) this.ErrorAlertType.Show(entry.Message);
		}
		/// <summary>
		/// Forces writing out of what is in the log
		/// </summary>
		public void Flush()
		{
			Flush(this.LogType);
		}
		/// <summary>
		/// Flushes the logger, then disposes of its internal cache
		/// </summary>
		public void Dispose()
		{
			this.Flush();
		}

		internal LogEntry Write(LogSeverity severity, string message)
		{
			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.Type = this.Type;
			entry.Message = message;
			entry.Source = this.Source;
			entry.Date = DateTime.Now;
			entry.LogType = this.LogType;
			entries.Add(entry);

			if (suspendedTypes.Contains(this.Type)) return entry;

			this.OutWriteLine(entry);

			try
			{
				if (entries.Count >= this.BatchSize) Logger.Flush(this.LogType);
			}
			catch
			{
			}
			return entry;
		}

		private void OutWriteLine(LogEntry entry)
		{
			if ((entry.LogType & LogType.Console) != LogType.None)
			{
				Console.WriteLine(String.Format(SR.VarLoggerConsoleOutput, entry.Severity.ToString().ToUpper(), entry.Date, entry.Type.Name, entry.Message));
			}
			if ((entry.LogType & LogType.DiagnosticsDebug) != LogType.None)
			{
				System.Diagnostics.Debug.WriteLine(String.Format(SR.VarLoggerConsoleOutput, entry.Severity.ToString().ToUpper(), entry.Date, entry.Type.Name, entry.Message));
			}
		}

		#endregion
		#region Static Methods

		/// <summary>
		/// Gets a pre initialized (or new) Logger for specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <returns>Existing, or newly minted logger</returns>
		public static Logger GetLogger(Type type)
		{
			if (type == null) throw new ArgumentException("type");
			lock (staticLoggers)
			{
				if (!staticLoggers.ContainsKey(type)) staticLoggers.Add(type, new Logger(type));
				return staticLoggers[type];
			}
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogInfo 
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Info(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Info(message, arguments);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogDebug
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Debug(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Debug(message, arguments);
		}
		/// <summary>
		/// Called to log errors of type Warning converting value to string
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="value">The value</param>
		public static void Warn(Type type, object value)
		{
			Logger.GetLogger(type).Warn(value);
		}
		/// <summary>
		/// Called to log errors of type Warning
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Warn(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Warn(message, arguments);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogError
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Error(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Error(message, arguments);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogError
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="ex">The Exception object to log</param>
		public static void Error(Type type, Exception ex)
		{
			Logger.GetLogger(type).Error(ex);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogError
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="ex">The Exception object to log</param>
		/// <param name="phoneHome">If to send an email to Logger.EmailErrorTo</param>
		public static void Error(Type type, Exception ex, bool phoneHome)
		{
			Logger.GetLogger(type).Error(ex, phoneHome);
		}
		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to log</param>
		/// <returns>String of the exception</returns>
		public static string ConvertToString(Exception exception)
		{
			return ConvertToString(exception, false);
		}
		public static string ConvertToString(Exception exception, bool probeDeep)
		{
			System.Text.StringBuilder message = new System.Text.StringBuilder();
			for (Exception ex = exception; ex != null; ex = exception.InnerException)
			{
				if (message.Length != 0)
				{
					message.Append(String.Empty.PadRight(20, '-'));
					message.Append(Environment.NewLine);
				}
				message.Append(ex.Message);
				message.Append(Environment.NewLine);
				message.Append(ex.StackTrace);
				if (!probeDeep) break;
			}
			return message.ToString();
		}

		internal static void Flush(LogType logType)
		{
			Monitor.Enter(entries);
			try
			{
				if (entries.Count > 0)
				{
					bool dbed = true;
					bool filed = true;
					if ((logType & LogType.DataBase) != LogType.None) dbed = SaveToDataBase();
					if ((logType & LogType.File) != LogType.None) filed = SaveToFile();
					if ((logType & LogType.EventLog) != LogType.None) SaveToEntryLog();
					if (dbed && filed) entries.Clear();
				}
			}
			finally
			{
				Monitor.Exit(entries);
			}
		}
		internal static bool SaveToDataBase()
		{
			if (sqlHelper == null) return false;

			System.Text.StringBuilder sb = new System.Text.StringBuilder(entries.Count);
			foreach (LogEntry row in entries)
			{
				sb.AppendLine(String.Format(SR.VarEventLoggerSqlInsertStatement, row.Severity, row.Type, row.Message.Replace("'", "''"), row.Path, row.Date));
			}
			try
			{
				sqlHelper.ExecuteNonQuery(System.Data.CommandType.Text, sb.ToString());
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogInternalError(ex);
			}
			return false;
		}
		internal static bool SaveToFile()
		{
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(entries.Count);
				foreach (LogEntry row in entries)
				{
					sb.AppendLine(String.Format(SR.VarEventLoggerFileInsert, row.Date, row.Severity, row.Type, row.Message, row.Path));
				}
				ActualFileWrite(sb.ToString(), 0);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogInternalError(ex);
			}
			return false;
		}

		internal static bool SaveToEntryLog()
		{
			try
			{
				foreach (LogEntry entry in entries)
				{
					EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
					if (entry.Severity == LogSeverity.Error) eventLogEntryType = EventLogEntryType.Error;
					else if (entry.Severity == LogSeverity.Warn) eventLogEntryType = EventLogEntryType.Warning;
					EventLog.WriteEntry(entry.Source, String.Format(SR.VarLoggerEventLogMessage, DateTime.Now, entry.Type, entry.Path, entry.Message), eventLogEntryType);
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogInternalError(ex);
			}
			return false;
		}
		internal static void ResumeLogging(params Type[] types)
		{
			foreach (Type type in types)
			{
				if (suspendedTypes.Contains(type)) suspendedTypes.Remove(type);
			}
		}

		internal static void SuspendLogging(params Type[] types)
		{
			foreach (Type type in types)
			{
				if (!suspendedTypes.Contains(type)) suspendedTypes.Add(type);
			}
		}

		private static void LogInternalError(Exception ex)
		{
			#if DEBUG
			System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			#else
			try
			{
				EventLog.WriteEntry("SnUtilsLogger", ConvertToString(ex), EventLogEntryType.Warning);
			}
			catch { }
			#endif
		}
		private static void ActualFileWrite(string str, int numAttempts) 
		{
			Logger.ActualFileWrite(SR.VarEventLogFile, str, numAttempts);
		}
		private static void ActualFileWrite(string filePath, string str, int numAttempts)
		{
			bool failed = false;
			FileInfo traceFile = new FileInfo(filePath);
			try
			{
				string fileName = Path.GetFileNameWithoutExtension(traceFile.Name);
				string extension = traceFile.Extension;
				if (traceFile.Exists)
				{
					if (traceFile.LastWriteTime.Date < DateTime.Now.AddDays(-1))
					{
						string postPend = traceFile.LastWriteTime.ToString("yyyyMMdd");
						string traceFileName = String.Concat(fileName, postPend, extension);
						FileInfo[] fs = traceFile.Directory.GetFiles("*" + traceFileName);
						if (fs.Length > 0) traceFileName = String.Concat(fileName, postPend, fs.Length, extension);
						traceFile.MoveTo(traceFileName);
					}
				}
			}
			catch (Exception ex)
			{
				failed = true;
				System.Diagnostics.Debug.WriteLine(ex);
			}

			if (!failed)
			{
				TextWriter writer = null;
				Stream stream = null;
				try
				{
					stream = traceFile.Open(FileMode.Append, FileAccess.Write);
					writer = new StreamWriter(stream);
					writer.Write(str);
				}
				catch (Exception ex)
				{
					failed = true;
					Logger.LogInternalError(ex);
				}
				finally
				{
					if (writer != null) writer.Close();
					if (stream != null) stream.Close();
				}
			}

			if (failed && numAttempts < 3)
			{
				numAttempts++;
				Thread.Sleep(1000);
				ActualFileWrite(filePath, str, numAttempts);
			}
		}

		#endregion
	}
}