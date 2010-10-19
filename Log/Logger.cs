using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Adenson.Configuration;
using Adenson.Data;
using System.Configuration;
using System.Xml.Serialization;

namespace Adenson.Log
{
	/// <summary>
	/// Logger of .... well, logs
	/// </summary>
	public sealed class Logger : IDisposable
	{
		#region Variables
		private static List<LogEntry> entries = new List<LogEntry>();
		private static List<Type> suspendedTypes = new List<Type>();
		private static Dictionary<Type, Logger> staticLoggers = new Dictionary<Type, Logger>();
		private static string OutFileName;
		private Type _classType;
		private short _batchLogSize;
		private LogType _logType = LogType.None;
		private LogSeverity _severity = LogSeverity.None;
		private string _dateTimeFormat;
		private string _source;
		#endregion
		#region Constructors

		static Logger()
		{
			string filePath = Config.LogSettings.FileName;
			string folder = null;
			if (!Path.IsPathRooted(filePath))
			{
				var context = System.Web.HttpContext.Current;//context can indeed be null sometimes, depending on when Logger is called, even in a ASP.NET project
				if (context != null) filePath = context.Server.MapPath(filePath);
				else filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath.Replace("/", "\\"));
				folder = Path.GetDirectoryName(filePath);
			}
			if (!Directory.Exists(folder))
			{
				#if DEBUG
				System.Diagnostics.Debug.WriteLine(String.Format("Folder {0} does not exist, file logging will not happen", folder));
				#endif
				return;
			}

			OutFileName = filePath;
		}
		/// <summary>	
		/// Creates a new instance of Logger from type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type classType)
		{
			if (classType == null) throw new ArgumentNullException(Exceptions.ArgumentNull);
			_classType = classType;
		}
		/// <summary>	
		/// Creates a new instance of Logger from type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type classType, string source) : this(classType, LogType.None, source)
		{
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="logType">The log type</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type classType, LogType logType) : this(classType)
		{
			_logType = logType;
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="logType">The log type</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		/// <exception cref="ArgumentNullException">if source is null and logType includes EventLog</exception>
		public Logger(Type classType, LogType logType, string source) : this(classType)
		{
			if ((logType & LogType.EventLog) != LogType.None && String.IsNullOrEmpty(source)) throw new ArgumentNullException("source", Exceptions.EventLogTypeWithSourceNull);
			_logType = logType;
			_source = source;
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="instance"></param>
		public Logger(Type classType, Logger instance) : this(classType)
		{
			if (instance == null) throw new ArgumentNullException("instance", Exceptions.ArgumentNull);
			this.BatchSize = instance.BatchSize;
			this.Severity = instance.Severity;
			this.Type = instance.Type;
			this.Source = instance.Source;
		}
		/// <summary>
		/// Clean up when logger is destroyed
		/// </summary>
		~Logger()
		{
			Logger.Flush(this.Type);
			lock (staticLoggers)
			{
				if (staticLoggers.ContainsKey(this.ClassType)) staticLoggers.Remove(this.ClassType);
			}
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the number of logs thats kept in memory before they are dumped into the database, defaults to 10
		/// </summary>
		public short BatchSize
		{
			get { return _batchLogSize == 0 ? Config.LogSettings.BatchSize : _batchLogSize; }
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
			get { return _severity == LogSeverity.None ? Config.LogSettings.Severity : _severity; }
			set { _severity = value; }
		}
		/// <summary>
		/// Gets the logging type
		/// </summary>
		public LogType Type
		{
			get { return _logType == LogType.None ? Config.LogSettings.TypeActual : _logType; }
			set { _logType = value; }
		}
		/// <summary>
		/// The type form which this instance is forged from
		/// </summary>
		public Type ClassType
		{
			get { return _classType; }
		}
		/// <summary>
		/// Gets the string tht will be used as source for Window's Event Log
		/// </summary>
		public string Source
		{
			get { return String.IsNullOrEmpty(_source) ? Config.LogSettings.Source : _source; }
			internal set { _source = value; }
		}
		/// <summary>
		/// Gets the string tht will be used as source for Window's Event Log
		/// </summary>
		public string DateTimeFormat
		{
			get { return (String.IsNullOrEmpty(_dateTimeFormat) ? Config.LogSettings.DateTimeFormat : _dateTimeFormat); }
			set { _dateTimeFormat = value; }
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
			this.Write(LogSeverity.Info, message, arguments);
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
			this.Write(LogSeverity.Debug, message, arguments);
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
			this.Write(LogSeverity.Warn, message, arguments);
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
		/// Called to log errors
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public void Error(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, message, arguments);
		}
		/// <summary>
		/// Called to log errors
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		public void Error(Exception ex)
		{
			string message = Logger.ConvertToString(ex);
			LogEntry entry = this.Write(LogSeverity.Error, message);

			if (ex is OutOfMemoryException) Thread.CurrentThread.Abort();
		}
		/// <summary>
		/// Forces writing out of what is in the log
		/// </summary>
		public void Flush()
		{
			//new System.Threading.Thread(new ThreadStart(delegate()
			//{
			Logger.Flush(this.Type);
			//})).Start();
		}
		/// <summary>
		/// Flushes the logger, then disposes of its internal cache
		/// </summary>
		public void Dispose()
		{
			this.Flush();
		}

		internal LogEntry Write(LogSeverity severity, string message, params object[] arguments)
		{
			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.Type = this.ClassType;
			entry.Source = this.Source;
			entry.Date = DateTime.Now;
			entry.LogType = this.Type;
			if (arguments.Length == 0) entry.Message = message;
			else
			{
				try
				{
					entry.Message = String.Format(message, arguments);
				}
				catch (FormatException)
				{
					var str = message;
					for (int i = 0; i < arguments.Length; i++) str = str.Replace("{" + i + "}", (arguments[i] == null ? "null" : Convert.ToString(arguments[i])));
					entry.Message = str;
				}
				catch
				{
					entry.Message = message;
				}
			}
			entries.Add(entry);

			if (suspendedTypes.Contains(this.ClassType)) return entry;

			Logger.OutWriteLine(entry);

			try
			{
				if (entries.Count >= this.BatchSize) this.Flush();
			}
			catch
			{
			}
			return entry;
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
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to log</param>
		/// <returns>String of the exception</returns>
		public static string ConvertToString(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException("exception");
			System.Text.StringBuilder message = new System.Text.StringBuilder();
			Exception ex = exception;
			while (ex != null)
			{
				if (message.Length != 0)
				{
					message.Append(String.Empty.PadRight(20, '-'));
					message.Append(Environment.NewLine);
				}
				message.Append(ex.GetType());
				message.Append(": ");
				message.AppendLine(ex.Message);
				message.AppendLine(ex.StackTrace);
				ex = ex.InnerException;
			}
			return message.ToString();
		}

		internal static void Flush(LogType logType)
		{
			try
			{
				Monitor.Enter(entries);
				if (entries.Count > 0)
				{
					if ((logType & LogType.DataBase) != LogType.None) Logger.SaveToDatabase();
					if ((logType & LogType.File) != LogType.None) Logger.SaveToFile();
					if ((logType & LogType.EventLog) != LogType.None) Logger.SaveToEntryLog();
					entries.Clear();
				}
			}
			finally
			{
				Monitor.Exit(entries);
			}
		}
		internal static bool SaveToDatabase()
		{
			var sqlHelper = SqlHelperProvider.Create(ConnectionStrings.Get("Logger", true));
			if (sqlHelper == null) return false;

			System.Text.StringBuilder sb = new System.Text.StringBuilder(entries.Count);
			foreach (LogEntry row in entries)
			{
				sb.AppendLine(String.Format(SR.EventLoggerSqlInsertStatement, row.Severity, row.Type, row.Message.Replace("'", "''"), row.Path, row.Date));
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
			if (String.IsNullOrWhiteSpace(Logger.OutFileName)) return false;

			try
			{
				var lastWriteTime = File.GetLastWriteTime(Logger.OutFileName);
				if (File.Exists(Logger.OutFileName) && lastWriteTime.Date < DateTime.Now.AddDays(-1))
				{
					string fileName = Path.GetFileNameWithoutExtension(Logger.OutFileName);
					string extension = Path.GetExtension(Logger.OutFileName);
					string oldNewFileName = String.Concat(fileName, lastWriteTime.ToString("yyyyMMdd"), extension);
					string oldNewFilePath = Path.Combine(Path.GetDirectoryName(Logger.OutFileName), oldNewFileName);
					if (!File.Exists(oldNewFilePath)) File.Move(Logger.OutFileName, oldNewFilePath);
				}
			}
			catch (Exception)
			{
			}

			TextWriter writer = null;
			Stream stream = null;
			System.Text.StringBuilder sb = new System.Text.StringBuilder(entries.Count);
			foreach (LogEntry row in entries) sb.AppendLine(String.Format(SR.EventLoggerFileInsert, row.Date, row.Severity, row.Type, row.Message, row.Path));
			FileInfo traceFile = new FileInfo(Logger.OutFileName);
			try
			{
				stream = traceFile.Open(FileMode.Append, FileAccess.Write);
				writer = new StreamWriter(stream);
				writer.Write(sb.ToString());
				return true;
			}
			catch (UnauthorizedAccessException ex)
			{
				Logger.LogInternalError(ex);
			}
			catch (Exception ex)
			{
				Logger.LogInternalError(ex);
			}
			finally
			{
				if (writer != null) writer.Close();
				if (stream != null) stream.Close();
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
					EventLog.WriteEntry(entry.Source, String.Format(SR.LoggerEventLogMessage, DateTime.Now, entry.Type, entry.Path, entry.Message), eventLogEntryType);
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
			System.Diagnostics.Debug.WriteLine(Logger.ConvertToString(ex));
			#else
			try
			{
				EventLog.WriteEntry("Adenson.Log.Logger", ConvertToString(ex), EventLogEntryType.Warning);
			}
			catch { }
			#endif
		}
		private static void OutWriteLine(LogEntry entry)
		{
			if ((entry.LogType & LogType.Console) != LogType.None)
			{
				Console.WriteLine(String.Format(SR.LoggerConsoleOutput, entry.Severity.ToString().ToUpper(), entry.Date.ToString("H:mm:ss.fff"), entry.Type.Name, entry.Message));
			}
			if ((entry.LogType & LogType.Debug) != LogType.None)
			{
				System.Diagnostics.Debug.WriteLine(String.Format(SR.LoggerConsoleOutput, entry.Severity.ToString().ToUpper(), entry.Date.ToString("H:mm:ss.fff"), entry.Type.Name, entry.Message));
			}
		}

		#endregion
	}
}