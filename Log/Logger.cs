using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Adenson.Log.Config;

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
		private Type _classType;
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
		private Logger(Type classType)
		{
			if (classType == null) throw new ArgumentNullException("classType");
			_classType = classType;
		}

		#endregion
		#region Properties
	
		/// <summary>
		/// Gets the severity level that is logged.
		/// </summary>
		public LogSeverity Severity
		{
			get { return _severity == null ? LoggerSettings.Default.Severity : _severity.Value; }
			set { _severity = value; }
		}
		
		/// <summary>
		/// Gets the logging type
		/// </summary>
		public LogTypes Types
		{
			get { return _logTypes == null ? LoggerSettings.Default.Types : _logTypes.Value; }
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
			get { return String.IsNullOrEmpty(_source) ? LoggerSettings.Default.Source : _source; }
			set { _source = value; }
		}
		
		/// <summary>
		/// Gets the string tht will be used as source for Window's Event Log
		/// </summary>
		public string DateTimeFormat
		{
			get { return (String.IsNullOrEmpty(_dateTimeFormat) ? LoggerSettings.Default.DateTimeFormat : _dateTimeFormat); }
			set { _dateTimeFormat = value; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Log debug message, converting the specified value to string.
		/// </summary>
		/// <param name="value">The value</param>
		public void Info(object value)
		{
			this.Info(StringUtil.ToString(value));
		}
		
		/// <summary>
		/// Log information message
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
		/// Log debug messages, converting the specified value to string.
		/// </summary>>
		/// <param name="value">The value</param>
		public void Debug(object value)
		{
			this.Debug(StringUtil.ToString(value));
		}
		
		/// <summary>
		/// Log debug message
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
		/// Log warning message, converting the specified value to string.
		/// </summary>
		/// <param name="value">The value</param>
		public void Warn(object value)
		{
			this.Warn(StringUtil.ToString(value));
		}
		
		/// <summary>
		/// Log warning message
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
		/// Log error message, converting the specified value to string.
		/// </summary>
		/// <param name="value">The value</param>
		public void Error(object value)
		{
			this.Error(StringUtil.ToString(value));
		}
		
		/// <summary>
		/// Log error message
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if message is null or whitespace</exception>
		public void Error(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, message, arguments);
		}
		
		/// <summary>
		/// Log exception
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		public void Error(Exception ex)
		{
			string message = Logger.ConvertToString(ex);
			this.Write(LogSeverity.Error, message);

			if (ex is OutOfMemoryException) Thread.CurrentThread.Abort();
		}
		
		/// <summary>
		/// Starts a execution duration profiler
		/// </summary>
		/// <param name="identifier">Some kind of identifier</param>
		/// <returns>A profiler object</returns>
		public LogProfiler ProfilerStart(string identifier)
		{
			if (StringUtil.IsNullOrWhiteSpace(identifier)) throw new ArgumentNullException("identifier");

			LogProfiler profiler = new LogProfiler(this, identifier);
			lock (profilers)
			{
				profilers.Add(profiler);
			}
			return profiler;
		}
		
		internal void ProfilerStop(Guid uid)
		{
			lock (profilers)
			{
				profilers.Remove(profilers.First(p => p.Uid == uid));
			}
		}
		internal void Write(LogSeverityInternal severity, string message, params object[] arguments)
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

			if ((this.Types & LogTypes.Database) != LogTypes.None) Logger.SaveToDatabase(entry);
			if ((this.Types & LogTypes.File) != LogTypes.None) Logger.SaveToFile(entry);
			if ((this.Types & LogTypes.EventLog) != LogTypes.None) Logger.SaveToEntryLog(entry);
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
					if (ex.StackTrace != null) message.AppendLine(ex.StackTrace);
				}
				ex = ex.InnerException;
			}

			return message.ToString();
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
			string filePath = LoggerSettings.Default.FileName;
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
			var format = "{0}\t{1}\t[{2}]\t {3}";
			var date = entry.Date.ToString("H:mm:ss.fff", CultureInfo.CurrentCulture);
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
				if (!LoggerSettings.Default.EmailInfo.IsEmpty())
				{
					SmtpUtil.TrySend(LoggerSettings.Default.EmailInfo.From, LoggerSettings.Default.EmailInfo.To, LoggerSettings.Default.EmailInfo.Subject, message, false);
				}
			}
		}
		private static bool SaveToDatabase(LogEntry entry)
		{
			return LoggerSettings.Default.DatabaseInfo.Save(entry);
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static bool SaveToFile(LogEntry entry)
		{
			if (StringUtil.IsNullOrWhiteSpace(Logger.OutFileName)) return false;

			try
			{
				var lastWriteTime = File.GetLastWriteTime(Logger.OutFileName);
				if (File.Exists(Logger.OutFileName) && lastWriteTime.Date < DateTime.Now.AddDays(-1))
				{
					string fileName = Path.GetFileNameWithoutExtension(Logger.OutFileName);
					string extension = Path.GetExtension(Logger.OutFileName);
					string oldNewFileName = String.Concat(fileName, lastWriteTime.ToString("yyyyMMdd", CultureInfo.CurrentCulture), extension);
					string oldNewFilePath = Path.Combine(Path.GetDirectoryName(Logger.OutFileName), oldNewFileName);
					if (!File.Exists(oldNewFilePath)) File.Move(Logger.OutFileName, oldNewFilePath);
				}
			}
			catch (Exception)
			{
			}

			TextWriter writer = null;
			Stream stream = null;
			string sb = StringUtil.Format("{0}	{1}	{2}	{3}", entry.Date, entry.Severity, entry.TypeName, entry.Message);
			FileInfo traceFile = new FileInfo(Logger.OutFileName);
			try
			{
				stream = traceFile.Open(FileMode.Append, FileAccess.Write);
				writer = new StreamWriter(stream);
				writer.Write(sb);
				return true;
			}
			catch (IOException ex)
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static bool SaveToEntryLog(LogEntry entry)
		{
			try
			{
				EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
				if (entry.Severity == LogSeverity.Error) eventLogEntryType = EventLogEntryType.Error;
				else if (entry.Severity == LogSeverity.Warn) eventLogEntryType = EventLogEntryType.Warning;
				EventLog.WriteEntry(entry.Source, StringUtil.Format("Date: {0}\nType: {1}\n\n{2}", DateTime.Now, entry.TypeName, entry.Message), eventLogEntryType);
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