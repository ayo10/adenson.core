using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Adenson.Collections;
using Adenson.Configuration;
using Adenson.Configuration.Internal;
using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Logger of .... well, logs
	/// </summary>
	public sealed class Logger
	{
		#region Variables
		private static string OutFileName = GetOutFileName();
		private static Dictionary<Type, Logger> staticLoggers = new Dictionary<Type, Logger>();
		private List<LogEntry> entries = new List<LogEntry>();
		private Type _classType;
		private short _batchLogSize;
		private LogTypes? _logTypes;
		private LogSeverity? _severity;
		private string _dateTimeFormat;
		private string _source;
		private Hashtable<string, DateTime> trackers = new Hashtable<string, DateTime>();
		#endregion
		#region Constructors

		/// <summary>	
		/// Creates a new instance of Logger from type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type classType)
		{
			if (classType == null) throw new ArgumentNullException("classType");
			_classType = classType;
		}
		/// <summary>	
		/// Creates a new instance of Logger from type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type classType, string source) : this(classType, LogTypes.None, source)
		{
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="logType">The log type</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public Logger(Type classType, LogTypes logType) : this(classType)
		{
			_logTypes = logType;
		}
		/// <summary>
		/// Instantiates a new Logger class for specified type
		/// </summary>
		/// <param name="classType">Type to use to create new instance</param>
		/// <param name="logType">The log type</param>
		/// <param name="source">A string to identify logs in Event Logs if the log type is as such</param>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		/// <exception cref="ArgumentNullException">if source is null and logType includes EventLog</exception>
		public Logger(Type classType, LogTypes logType, string source) : this(classType)
		{
			if ((logType & LogTypes.EventLog) != LogTypes.None && String.IsNullOrEmpty(source)) throw new ArgumentNullException("source", Exceptions.EventLogTypeWithSourceNull);
			_logTypes = logType;
			_source = source;
		}
		/// <summary>
		/// Clean up when logger is destroyed
		/// </summary>
		~Logger()
		{
			this.Flush();
			lock (staticLoggers)
			{
				if (staticLoggers.ContainsKey(this.ClassType)) staticLoggers.Remove(this.ClassType);
			}
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the number of logs thats kept in memory before they are dumped into the database, defaults to 0 (flush imediately)
		/// </summary>
		public short BatchSize
		{
			get { return _batchLogSize == 0 ? Config.LogSettings.BatchSize : _batchLogSize; }
			set
			{
				if (value < 1) throw new ArgumentException(SR.MsgExMinLogBatchSize, "value");
				_batchLogSize = value;
			}
		}
		/// <summary>
		/// Gets the severity level that is logged.
		/// </summary>
		public LogSeverity Severity
		{
			get { return _severity == null ? Config.LogSettings.Severity : _severity.Value; }
			set { _severity = value; }
		}
		/// <summary>
		/// Gets the logging type
		/// </summary>
		public LogTypes Types
		{
			get { return _logTypes == null ? Config.LogSettings.Types : _logTypes.Value; }
			set { _logTypes = value; }
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
			set { _source = value; }
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
			this.Info(StringUtil.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Info
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if message is null or whitespace</exception>
		public void Info(string message, params object[] arguments)
		{
			if ((int)this.Severity > (int)LogSeverity.Info) return;
			this.Write(LogSeverity.Info, null, message, arguments);
		}
		/// <summary>
		/// Logs the value into the log of type debug, converting value to string
		/// </summary>>
		/// <param name="value">The value</param>
		public void Debug(object value)
		{
			this.Debug(StringUtil.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Debug
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if message is null or whitespace</exception>
		public void Debug(string message, params object[] arguments)
		{
			if ((int)this.Severity > (int)LogSeverity.Debug) return;
			this.Write(LogSeverity.Debug, null, message, arguments);
		}
		/// <summary>
		/// Called to log errors of type Warning converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void Warn(object value)
		{
			this.Warn(StringUtil.ToString(value));
		}
		/// <summary>
		/// Called to log errors of type Warning
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if message is null or whitespace</exception>
		public void Warn(string message, params object[] arguments)
		{
			if ((int)this.Severity > (int)LogSeverity.Warn) return;
			this.Write(LogSeverity.Warn, null, message, arguments);
		}
		/// <summary>
		/// Log the value into the log of type Error, converting value to string
		/// </summary>
		/// <param name="value">The value</param>
		public void Error(object value)
		{
			this.Error(StringUtil.ToString(value));
		}
		/// <summary>
		/// Called to log errors
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if message is null or whitespace</exception>
		public void Error(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, null, message, arguments);
		}
		/// <summary>
		/// Called to log errors
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		public void Error(Exception ex)
		{
			string message = Logger.ConvertToString(ex);
			this.Write(LogSeverity.Error, null, message);

			if (ex is OutOfMemoryException) Thread.CurrentThread.Abort();
		}
		/// <summary>
		/// Forces writing out of what is in the current log
		/// </summary>
		public void Flush()
		{
			lock (entries)
			{
				if (entries.Count == 0) return;
				if ((this.Types & LogTypes.Database) != LogTypes.None) Logger.SaveToDatabase(entries);
				if ((this.Types & LogTypes.File) != LogTypes.None) Logger.SaveToFile(entries);
				if ((this.Types & LogTypes.EventLog) != LogTypes.None) Logger.SaveToEntryLog(entries);
				entries.Clear();
			}
		}
		/// <summary>
		/// When Severity is Debug or higher, enables and starts tracking of time passage, unique to case sensitive specified identifier (to be used in conjuction with <seealso cref="MeasureStop(String)"/>)
		/// </summary>
		/// <remarks>Resets tracking if MeasureStart was called using the same identifier, but MeasureStop was not called.</remarks>
		/// <param name="identifier">The unique case sensitive identifier to use as track</param>
		/// <exception cref="ArgumentNullException">if identifier is null (or whitespace)</exception>
		public void MeasureStart(string identifier)
		{
			if (StringUtil.IsNullOrWhiteSpace(identifier)) throw new ArgumentNullException("identifier");
			if ((int)this.Severity > (int)LogSeverity.Debug) return;

			lock (trackers)
			{
				if (trackers.ContainsKey(identifier))
				{
					this.Write(LogSeverityInternal.Measure, identifier, "Force stopping existing measurement.", null);
					this.MeasureStopAndRemove(identifier);
				}
				trackers.Add(identifier, DateTime.Now);
				this.Write(LogSeverityInternal.Measure, identifier, "STARTED", null);
			}
		}
		/// <summary>
		/// When Severity is Debug or higher, ends all tracking started by <seealso cref="MeasureStart"/> regardless of identifier
		/// </summary>
		public void MeasureStop()
		{
			if ((int)this.Severity > (int)LogSeverity.Debug) return;

			lock (trackers)
			{
				foreach (var identifier in trackers.Keys.ToList()) this.MeasureStopAndRemove(identifier);
			}
		}
		/// <summary>
		/// When Severity is Debug or higher, ends tracking started with <seealso cref="MeasureStart(String)"/> using specified case sensitive identiifer
		/// </summary>
		/// <param name="identifier">The unique case sensitive identifier to use as track</param>
		/// <exception cref="ArgumentNullException">if identifier is null (or whitespace)</exception>
		public void MeasureStop(string identifier)
		{
			if (StringUtil.IsNullOrWhiteSpace(identifier)) throw new ArgumentNullException("identifier");
			if ((int)this.Severity > (int)LogSeverity.Debug) return;

			lock (trackers)
			{
				this.MeasureStopAndRemove(identifier);
			}
		}
		/// <summary>
		/// When Severity is Debug or higher, and <seealso cref="MeasureStart"/> was called with specified identifier, logs the value 
		/// </summary>
		/// <param name="identifier">The unique case sensitive identifier to use as track</param>
		/// <param name="value">The value to log</param>
		/// <exception cref="ArgumentNullException">if identifier is null (or whitespace)</exception>
		public void MeasureWrite(string identifier, object value)
		{
			this.MeasureWrite(identifier, StringUtil.ToString(value));
		}
		/// <summary>
		/// When Severity is Debug or higher, and <seealso cref="MeasureStart"/> was called with specified identifier, logs the value, formatting with specified arguments
		/// </summary>
		/// <param name="identifier">The unique case sensitive identifier to use as track</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if identifier OR message is null (or whitespace)</exception>
		public void MeasureWrite(string identifier, string message, params object[] arguments)
		{
			if (StringUtil.IsNullOrWhiteSpace(identifier)) throw new ArgumentNullException("identifier");
			if ((int)this.Severity > (int)LogSeverity.Debug) return;

			lock (trackers)
			{
				if (trackers.ContainsKey(identifier))
				{
					this.Write(LogSeverityInternal.Measure, identifier, "{0}. {1} secs", StringUtil.Format(message, arguments), DateTime.Now.Subtract(trackers[identifier]).TotalSeconds);
				}
			}
		}

		private void MeasureStopAndRemove(string identifier)
		{
			if (trackers.ContainsKey(identifier))
			{
				this.Write(LogSeverityInternal.Measure, identifier, "STOP, Elapsed Time: {0} secs", DateTime.Now.Subtract(trackers[identifier]).TotalSeconds);
				trackers.Remove(identifier);
			}
		}
		private void Write(LogSeverityInternal severity, string identifier, string message, params object[] arguments)
		{
			if (StringUtil.IsNullOrWhiteSpace(message)) throw new ArgumentNullException("message");

			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.TypeName = this.ClassType.Name + (String.IsNullOrEmpty(identifier) ? String.Empty : ("." + identifier));
			entry.Source = this.Source;
			entry.Date = DateTime.Now;
			entry.LogType = this.Types;

			if (arguments == null || arguments.Length == 0) entry.Message = message;
			else entry.Message = StringUtil.Format(message, arguments);

			Logger.OutWriteLine(entry);

			//Careful with the lock now, Flush locks entries too
			bool flush = false;
			lock (entries)
			{
				entries.Add(entry);
				flush = (entries.Count >= this.BatchSize);
			}
			if (flush) this.Flush();
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
			if (type == null) throw new ArgumentNullException("type");
			lock (staticLoggers)
			{
				if (!staticLoggers.ContainsKey(type)) staticLoggers.Add(type, new Logger(type));
				else staticLoggers[type].Flush();
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
		/// <summary>
		/// Forces writing out of what is in all logs
		/// </summary>
		public static void FlushAll()
		{
			lock (staticLoggers)
			{
				foreach (var logger in staticLoggers.Values) logger.Flush();
			}
		}

		private static void LogInternalError(Exception ex)
		{
			System.Diagnostics.Trace.WriteLine(Logger.ConvertToString(ex));
			#if !DEBUG
			try
			{
				EventLog.WriteEntry("Adenson.Log.Logger", ConvertToString(ex), EventLogEntryType.Warning);
			}
			catch { }
			#endif
		}
		private static string GetOutFileName()
		{
			string filePath = Config.LogSettings.FileName;
			string folder = null;
			if (!Path.IsPathRooted(filePath))
			{
				filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath.Replace("/", "\\"));
				folder = Path.GetDirectoryName(filePath);
			}
			if (!Directory.Exists(folder))
			{
				filePath = null;
				System.Diagnostics.Trace.WriteLine(StringUtil.Format("Adenson.Log.Logger: ERROR: Folder {0} does not exist, file logging will not happen", folder));
			}

			return filePath;
		}
		private static void OutWriteLine(LogEntry entry)
		{
			var format = "{0} {1} [{2}] - {3}";
			var date = entry.Date.ToString("H:mm:ss.fff", CultureInfo.InvariantCulture);
			var message = StringUtil.Format(format, entry.Severity.ToString(), date, entry.TypeName, entry.Message);
			
			if ((entry.LogType & LogTypes.Console) != LogTypes.None)
			{
				Console.WriteLine(message);
			}
			
			if ((entry.LogType & LogTypes.Debug) != LogTypes.None)
			{
				System.Diagnostics.Trace.WriteLine(message);
			}
			
			if ((entry.LogType & LogTypes.Email) != LogTypes.None)
			{
				if (!Config.LogSettings.EmailInfo.IsEmpty())
				{
					SmtpUtil.TrySend(Config.LogSettings.EmailInfo.From, Config.LogSettings.EmailInfo.To, Config.LogSettings.EmailInfo.Subject, message, false);
				}
			}
		}
		private static bool SaveToDatabase(List<LogEntry> entries)
		{
			var sqlHelper = SqlHelperProvider.Create(ConnectionStrings.Get("Logger", true));
			if (sqlHelper == null) return false;

			System.Text.StringBuilder sb = new System.Text.StringBuilder(entries.Count);
			var statement = Config.LogSettings.DatabaseInfo.CreateInsertStatement();
			foreach (LogEntry entry in entries)
			{
				sb.AppendLine(StringUtil.Format(statement, entry.Severity, entry.TypeName, entry.Message.Replace("'", "''"), entry.Date));
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
		private static bool SaveToFile(List<LogEntry> entries)
		{
			if (StringUtil.IsNullOrWhiteSpace(Logger.OutFileName)) return false;

			try
			{
				var lastWriteTime = File.GetLastWriteTime(Logger.OutFileName);
				if (File.Exists(Logger.OutFileName) && lastWriteTime.Date < DateTime.Now.AddDays(-1))
				{
					string fileName = Path.GetFileNameWithoutExtension(Logger.OutFileName);
					string extension = Path.GetExtension(Logger.OutFileName);
					string oldNewFileName = String.Concat(fileName, lastWriteTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture), extension);
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
			foreach (LogEntry row in entries) sb.AppendLine(StringUtil.Format("{0}	{1}	{2}	{3}", row.Date, row.Severity, row.TypeName, row.Message));
			FileInfo traceFile = new FileInfo(Logger.OutFileName);
			try
			{
				stream = traceFile.Open(FileMode.Append, FileAccess.Write);
				writer = new StreamWriter(stream);
				writer.Write(sb.ToString());
				return true;
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
		private static bool SaveToEntryLog(List<LogEntry> entries)
		{
			try
			{
				foreach (LogEntry entry in entries)
				{
					EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
					if (entry.Severity == LogSeverity.Error) eventLogEntryType = EventLogEntryType.Error;
					else if (entry.Severity == LogSeverity.Warn) eventLogEntryType = EventLogEntryType.Warning;
					EventLog.WriteEntry(entry.Source, StringUtil.Format("Date: {0}\nType: {1}\n\n{2}", DateTime.Now, entry.TypeName, entry.Message), eventLogEntryType);
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogInternalError(ex);
			}
			return false;
		}

		#endregion
	}
}