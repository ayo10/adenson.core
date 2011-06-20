using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Adenson.Configuration.Internal;
using System.Diagnostics.CodeAnalysis;

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
		private List<LogProfiler> profilers = new List<LogProfiler>();
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
			this.Write(LogSeverity.Info, message, arguments);
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
			this.Write(LogSeverity.Debug, message, arguments);
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
			this.Write(LogSeverity.Warn, message, arguments);
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
			this.Write(LogSeverity.Error, message, arguments);
		}
		
		/// <summary>
		/// Called to log errors
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		public void Error(Exception ex)
		{
			string message = Logger.ConvertToString(ex);
			this.Write(LogSeverity.Error, message);

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
		/// Starts a execution duration profiler
		/// </summary>
		/// <param name="identifier">Some kind of identifier</param>
		/// <returns>A profiler object</returns>
		public LogProfiler ProfilerStart(string identifier)
		{
			LogProfiler profile;
			lock (profilers)
			{
				profile = new LogProfiler(this);
				profilers.Add(profile);
				this.Write(LogSeverityInternal.Measure, "{0}{1}", "START", identifier);
			}
			return profile;
		}
		
		internal void MeasureStop(Guid uid)
		{
			lock(profilers)
			{
				LogProfiler profile = profilers.First(p => p.Uid == uid);
				var elt = profile.ElapsedTime;
				string elts = (elt == 0 ? "0.0" : elt.ToString()).PadRight(8, '0');
				this.Write(LogSeverityInternal.Measure, "STOP [@ {0} secs].{1}", elts);
				profilers.Remove(profile);
			}
		}
		private void Write(LogSeverityInternal severity, string message, params object[] arguments)
		{
			if (StringUtil.IsNullOrWhiteSpace(message)) throw new ArgumentNullException("message");

			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.TypeName = this.ClassType.Name;
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
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Info(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Info(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Info(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Debug(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Debug(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Debug(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Warn(object)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="value">The value</param>
		public static void Warn(Type type, object value)
		{
			Logger.GetLogger(type).Warn(value);
		}
		
		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Warn(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Warn(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Warn(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Error(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		public static void Error(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Error(message, arguments);
		}
		
		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Error(Exception)"/>
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
		/// <param name="exception">The Exception object to convert</param>
		/// <returns>String of the exception</returns>
		/// <remarks>Calls <see cref="ConvertToString(Exception, bool)"/>, with messageOnly = false</remarks>
		public static string ConvertToString(Exception exception)
		{
			return Logger.ConvertToString(exception, false);
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to convert</param>
		/// <param name="messageOnly"></param>
		/// <returns></returns>
		public static string ConvertToString(Exception exception, bool messageOnly)
		{
			if (exception == null) throw new ArgumentNullException("exception");

			StringBuilder message = new StringBuilder();
			Exception ex = exception;
			while (ex != null)
			{
				if (message.Length != 0)
				{
					if (messageOnly) message.Append(" ");
					else
					{
						message.Append(String.Empty.PadRight(20, '-'));
						message.Append(Environment.NewLine);
					}
				}

				if (messageOnly)
				{
					message.Append(ex.Message);
					message.Append(".");
				}
				else
				{
					message.Append(ex.GetType().FullName);
					message.Append(": ");
					message.AppendLine(ex.Message);
					message.AppendLine(ex.StackTrace);
				}
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

		/// <summary>
		/// Instantiates a Logger object, then calls <see cref="ProfilerStart(string)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="identifier">Some kind of identifier</param>
		/// <returns>A disposable profiler object</returns>
		public static LogProfiler ProfilerStart(Type type, string identifier)
		{
			return Logger.GetLogger(type).ProfilerStart(identifier);
		}

		internal static void LogInternalError(Exception ex)
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
			}
			
			folder = Path.GetDirectoryName(filePath);
			
			if (!Directory.Exists(folder))
			{
				filePath = null;
				System.Diagnostics.Trace.WriteLine(StringUtil.Format("Adenson.Log.Logger: ERROR: Folder {0} does not exist, file logging will not happen", folder));
			}

			return filePath;
		}
		private static void OutWriteLine(LogEntry entry)
		{
			var format = "{0}\t{1}\t[{2}]\t- {3}";
			var date = entry.Date.ToString("H:mm:ss.fff");
			var message = StringUtil.Format(format, entry.Severity.ToString(), date, entry.TypeName, entry.Message);
			
			if ((entry.LogType & LogTypes.Console) != LogTypes.None)
			{
				Console.WriteLine(message);
			}

			if ((entry.LogType & LogTypes.Trace) != LogTypes.None)
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
		private static bool SaveToDatabase(IEnumerable<LogEntry> entries)
		{
			return Config.LogSettings.DatabaseInfo.Save(entries);
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
			StringBuilder sb = new StringBuilder(entries.Count);
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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