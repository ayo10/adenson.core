using System;
using System.Collections.Generic;
using System.Configuration;
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
		private static string configEmailErrorTo, configSource;
		private static ushort configBatchLogSize = 0;
		private static LogSeverity configSeverityLevel = LogSeverity.None;
		private static LogType configLogType = LogType.None;
		private static SqlHelperBase sqlHelper;
		private static object isInWindowsContext, isInWebContext, isUsingSNWeb;
		private static string executablePath; 
		private Type _type;
		private ushort _batchLogSize = 10;
		private ErrorAlertType _errorAlertType;
		private LogType _logType = LogType.None;
		private LogSeverity _severity = LogSeverity.None;
		private string _emailErrorTo;
		private string _source;
		#endregion
		#region Constructors

		static Logger()
		{
			Dictionary<string, string> config = ConfigSectionHelper.GetDictionary("Logger");
			if (config != null)
			{
				if (config.ContainsKey("LogType"))
				{
					string logType = config["LogType"];
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

				if (config.ContainsKey("Severity")) configSeverityLevel = (LogSeverity)Enum.Parse(typeof(LogSeverity), config["Severity"], true);
				else if (config.ContainsKey("SeverityLevel")) configSeverityLevel = (LogSeverity)Enum.Parse(typeof(LogSeverity), config["SeverityLevel"], true);
				if (config.ContainsKey("EmailErrorTo")) configEmailErrorTo = config["EmailErrorTo"];
				if (config.ContainsKey("BatchSize")) configBatchLogSize = Convert.ToUInt16(config["BatchSize"]);
				if (config.ContainsKey("ErrorAlertType")) configErrorAlertType = ErrorAlertType.Parse(config["ErrorAlertType"]);
				if (config.ContainsKey("Source")) configSource = config["Source"];
			}
			else
			{
				#if DEBUG
				configSeverityLevel = LogSeverity.Debug;
				#else
				configSeverityLevel = LogSeverity.Error;
				#endif

				if (Logger.CheckIsInWebContext())
				{
					configLogType = LogType.WebProjects;
					#if DEBUG 
					if (Logger.IsUsingAdensonWeb()) configErrorAlertType = ErrorAlertType.Parse("Adenson.Web.UI, Adenson.Web.UI.MessageBox, Show");
					#endif
				}
				if (Logger.CheckIsInWindowsContext())
				{
					configLogType = LogType.WinFormProjects;
					#if DEBUG 
					configErrorAlertType = ErrorAlertType.Parse("System.Windows.Forms, System.Windows.Forms.MessageBox, Show");
					#endif
				}
				else configLogType = LogType.ConsoleProjects;
			}

			if ((configLogType & LogType.EventLog) != LogType.None && string.IsNullOrEmpty(configSource)) configSource = "SnUtilsLogger";
			string connString = ConnectionStrings.TryGet("logger", true);
			if (!String.IsNullOrEmpty(connString)) sqlHelper = SqlHelperProvider.Create(connString, true);
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
		/// <param name="severity">The severity</param>
		/// <param name="logType">The log type</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		/// <exception cref="ArgumentNullException">if source is null and logType includes EventLog</exception>
		public Logger(Type type, LogType logType, string source) : this(type)
		{
			if ((logType & LogType.EventLog) != LogType.None && string.IsNullOrEmpty(source)) throw new ArgumentNullException("source", ExceptionMessages.EventLogTypeWithSourceNull);
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
		/// Gets the errors email recipient (if LogError(Type, Exception, true/false, true) OR its non static version is called)
		/// </summary>
		public string EmailErrorTo
		{
			get { return string.IsNullOrEmpty(_emailErrorTo) ? configEmailErrorTo : _emailErrorTo; }
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
			get { return string.IsNullOrEmpty(_source) ? configSource : _source; }
			internal set { _source = value; }
		}

		internal string RequestPath
		{
			get
			{
				Logger.CheckIsInWebContext();
				Logger.CheckIsInWindowsContext();
				if (!string.IsNullOrEmpty(executablePath)) return executablePath;
				Assembly assembly = Assembly.GetEntryAssembly();
				if (assembly == null) assembly = Assembly.GetExecutingAssembly();
				if (assembly == null) assembly = Assembly.GetCallingAssembly();
				if (assembly == null) return string.Empty;
				return Uri.EscapeUriString(assembly.CodeBase);
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Logs the value into the log of type info, converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void LogInfo(object value)
		{
			this.LogInfo(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Info
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void LogInfo(string message, params object[] arguments)
		{
			if (Convert.ToInt32(this.Severity) > Convert.ToInt32(LogSeverity.Info)) return;
			this.Write(LogSeverity.Info, arguments == null ? message : string.Format(message, arguments));
		}
		/// <summary>
		/// Logs the value into the log of type debug, converting value to string
		/// </summary>>
		/// <param name="value">The value</param>
		public void LogDebug(object value)
		{
			this.LogDebug(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Debug
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void LogDebug(string message, params object[] arguments)
		{
			if (Convert.ToInt32(this.Severity) > Convert.ToInt32(LogSeverity.Debug)) return;
			this.Write(LogSeverity.Debug, arguments == null ? message : string.Format(message, arguments));
		}
		/// <summary>
		/// Called to log errors of type Warning converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void LogWarning(object value)
		{
			this.LogWarning(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Warning
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void LogWarning(string message, params object[] arguments)
		{
			if (Convert.ToInt32(this.Severity) > Convert.ToInt32(LogSeverity.Warning)) return;
			this.Write(LogSeverity.Warning, arguments == null ? message : string.Format(message, arguments));
		}
		/// <summary>
		/// Log the value into the log of type Error, converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void LogError(object value)
		{
			this.LogError(Convert.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Error
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void LogError(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, arguments == null ? message : string.Format(message, arguments));
		}
		/// <summary>
		/// Called to log errors of type Error
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		public void LogError(Exception ex)
		{
			this.LogError(ex, false);
		}
		/// <summary>
		/// Called to log errors of type Error
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		/// <param name="phoneHome">If to send an email to Logger.EmailErrorTo</param>
		public void LogError(Exception ex, bool phoneHome)
		{
			string message = Logger.ConvertToString(ex);
			LogEntry entry = this.Write(LogSeverity.Error, message);

			if (ex is System.OutOfMemoryException) Thread.CurrentThread.Abort();
			if (ex is System.Data.Common.DbException && sqlHelper != null) sqlHelper.ClearParameterCache();

			if (phoneHome)
			{
				this.Flush();
				if (this.EmailErrorTo != null)
				{
					string body = "System Error:\n\n" + this.RequestPath + "\n\n" + message;
					string subject = "Application Error: " + System.DateTime.Today.ToLongDateString();
					Adenson.Net.Mailer.SendAsync("errors@adenson.com", this.EmailErrorTo, subject, body, false);
				}
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
			entry.Path = this.RequestPath;
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
				Console.WriteLine(String.Format(SR.VarLoggerConsoleOutput, entry.Severity.ToString().Substring(0, 1), entry.Date.ToString(SR.LoggerDateFormat), TypeToString(entry.Type), entry.Message));
			}
			if ((entry.LogType & LogType.DiagnosticsDebug) != LogType.None)
			{
				Debug.WriteLine(String.Format(SR.VarLoggerConsoleOutput, entry.Severity.ToString().Substring(0, 1), entry.Date.ToString(SR.LoggerDateFormat), TypeToString(entry.Type), entry.Message));
			}
		}

		#endregion
		#region Static Methods

		/// <summary>
		/// Instantiates a Logger object, then calls LogInfo 
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void LogInfo(Type type, string message, params object[] arguments)
		{
			new Logger(type).LogInfo(message, arguments);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogDebug
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void LogDebug(Type type, string message, params object[] arguments)
		{
			new Logger(type).LogDebug(message, arguments);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogError
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void LogError(Type type, string message, params object[] arguments)
		{
			new Logger(type).LogError(message, arguments);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogError
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="ex">The Exception object to log</param>
		public static void LogError(Type type, Exception ex)
		{
			new Logger(type).LogError(ex);
		}
		/// <summary>
		/// Instantiates a Logger object, then calls LogError
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="ex">The Exception object to log</param>
		/// <param name="phoneHome">If to send an email to Logger.EmailErrorTo</param>
		public static void LogError(Type type, Exception ex, bool phoneHome)
		{
			new Logger(type).LogError(ex, phoneHome);
		}
		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to log</param>
		/// <returns>String of the exception</returns>
		public static string ConvertToString(Exception exception)
		{
			Exception ex = exception;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			while (ex != null)
			{
				sb.Append(String.Format(SR.VarLoggerExceptionMessage, ex.Message, ex.Source, ex.TargetSite, ex.StackTrace));
				ex = ex.InnerException;
			}
			return sb.ToString().TrimEnd('\r','\n');
		}

		internal static void Flush(LogType logType)
		{
			Monitor.Enter(entries);
			try
			{
				if (entries.Count > 0)
				{
					bool dbed = true, filed = true, entried;
					if ((logType & LogType.DataBase) != LogType.None) dbed = SaveToDataBase();
					if ((logType & LogType.File) != LogType.None) filed = SaveToFile();
					if ((logType & LogType.EventLog) != LogType.None) entried = SaveToEntryLog();
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

			Logger.SuspendLogging(typeof(ConnectionStrings));
			string connectionString = ConnectionStrings.Get("logger");
			Logger.ResumeLogging(typeof(ConnectionStrings));
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException(ExceptionMessages.LoggerNoConnString);
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
				Logger.LogInternalError(ex, true);
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
				Logger.LogInternalError(ex, true);
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
					else if (entry.Severity == LogSeverity.Warning) eventLogEntryType = EventLogEntryType.Warning;
					EventLog.WriteEntry(entry.Source, String.Format(SR.VarLoggerEventLogMessage, DateTime.Now, entry.Type, entry.Path, entry.Message), eventLogEntryType);
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogInternalError(ex, true);
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

		private static object TypeToString(Type type)
		{
			string str = type == null ? "None" : type.Name;
			if (str.Length <= 15) return str.PadRight(15);
			else return string.Concat(str.Substring(0, 7), "..", str.Substring(str.Length - 6));
		}
		private static void LogInternalError(Exception ex, bool logToSystemFile)
		{
			try
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
#else
				EventLog.WriteEntry("SnUtilsLogger", ConvertToString(ex), EventLogEntryType.Warning);
#endif
				//if (logToSystemFile)
				//{
				//    string path = Path.Combine(Environment.SystemDirectory, string.Concat("snutils.", SR.VarEventLogFile));
				//    ActualFileWrite(path, ConvertToString(ex), 1);
				//}
			}
			catch { }
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
						string traceFileName = string.Concat(fileName, postPend, extension);
						FileInfo[] fs = traceFile.Directory.GetFiles("*" + traceFileName);
						if (fs.Length > 0) traceFileName = string.Concat(fileName, postPend, fs.Length, extension);
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
					Logger.LogInternalError(ex, false);
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
		private static bool CheckIsInWindowsContext()
		{
			if (isInWindowsContext == null)
			{
				try
				{
					Assembly assembly = Assembly.Load("System.Windows.Form");
					Type type = assembly.GetType("System.Windows.Form.Application");
					PropertyInfo info = type.GetProperty("ExecutablePath", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
					executablePath = (string)info.GetValue(null, null);
					isInWindowsContext = true;
				}
				catch { }
				isInWindowsContext = false;
			}
			return (bool)isInWindowsContext;
		}
		private static bool CheckIsInWebContext()
		{
			if (isInWebContext == null)
			{
				try
				{
					Assembly assembly = Assembly.Load("System.Web");
					Type type = assembly.GetType("HttpContext");
					PropertyInfo info = type.GetProperty("Current", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
					object currentContext = info.GetValue(null, null);
					info = currentContext.GetType().GetProperty("Request", BindingFlags.Public | BindingFlags.Instance);
					object request = info.GetValue(currentContext, null);
					info = request.GetType().GetProperty("RawUrl", BindingFlags.Public | BindingFlags.Instance);
					executablePath = (string)info.GetValue(request, null);
					isInWebContext = true;
				}
				catch { }
				isInWebContext = false;
			}
			return (bool)isInWebContext;
		}
		private static bool IsUsingAdensonWeb()
		{
			if (isUsingSNWeb == null)
			{
				try
				{
					Assembly.Load("Adenson.Web.UI");
					isUsingSNWeb = true;
				}
				catch {}
				isUsingSNWeb = false;
			}
			return (bool)isUsingSNWeb;
		}

		#endregion
	}
}