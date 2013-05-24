using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading;

namespace Adenson.Log
{
	/// <summary>
	/// Logger of .... well, logs
	/// </summary>
	[DebuggerStepThrough]
	public sealed class Logger
	{
		#region Variables
		private static Dictionary<Type, Logger> staticLoggers = new Dictionary<Type, Logger>();
		private static LoggerSettings defaultSettings = LoggerSettings.Default;
		private static Action<LogEntry> globalBeforelog;
		private static bool eventSecurityWarned;
		private List<LogProfiler> profilers = new List<LogProfiler>();
		#endregion
		#region Constructors

		private Logger(Type classType)
		{
			if (classType == null)
			{
				throw new ArgumentNullException("classType");
			}

			this.ClassType = classType;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the type form which this instance is forged from
		/// </summary>
		public Type ClassType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the action that is invoked before the log is saved.
		/// </summary>
		public Action<LogEntry> BeforeLog
		{
			get;
			set;
		}

		#endregion
		#region Static Methods

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Critical(Object, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		public static void Critical(Type type, object message, params object[] arguments)
		{
			Logger.GetLogger(type).Critical(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Debug(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG")]
		public static void Debug(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Debug(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Error(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		public static void Error(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Error(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Error(Exception)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="ex">The Exception object to log.</param>
		public static void Error(Type type, Exception ex)
		{
			Logger.GetLogger(type).Error(ex);
		}

		/// <summary>
		/// Flushes the output buffer, and causes buffered data to be written.
		/// </summary>
		public static void Flush()
		{
			Trace.Flush();
		}

		/// <summary>
		/// Gets a pre initialized (or new) Logger for specified type
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>Existing, or newly minted logger</returns>
		public static Logger GetLogger(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			lock (staticLoggers)
			{
				if (!staticLoggers.ContainsKey(type))
				{
					var logger = new Logger(type);
					logger.BeforeLog = globalBeforelog;
					staticLoggers.Add(type, logger);
				}

				return staticLoggers[type];
			}
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Info(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void Info(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Info(message, arguments);
		}

		/// <summary>
		/// Instantiates a Logger object, then calls <see cref="ProfilerStart(string)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="identifier">Some kind of identifier.</param>
		/// <returns>A disposable profiler object</returns>
		public static LogProfiler ProfilerStart(Type type, string identifier)
		{
			return Logger.GetLogger(type).ProfilerStart(identifier);
		}
		
		/// <summary>
		/// Sets the standard error and standard output stream to the specified <see cref="TextWriter"/> object.
		/// </summary>
		/// <param name="writer">A stream that is the new standard output.</param>
		/// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
		public static void SetWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			Console.SetOut(writer);
			Console.SetError(writer);
			TextWriterTraceListener listener = new TextWriterTraceListener(writer);
			listener.Filter = new LogFilter();
			System.Diagnostics.Debug.Listeners.Add(listener);
			System.Diagnostics.Trace.Listeners.Add(listener);
		}

		/// <summary>
		/// Sets the before log predicate on *ALL* loggers, both present and future.
		/// </summary>
		/// <param name="predicate">The before log predicate.</param>
		/// <remarks>Setting <paramref name="predicate"/> to null will cause the predicate to be set on ALL types.</remarks>
		public static void SetBeforeLog(Action<LogEntry> predicate)
		{
			globalBeforelog = predicate;

			lock (staticLoggers)
			{
				foreach (Logger logger in staticLoggers.Values)
				{
					logger.BeforeLog = predicate;
				}
			}
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to convert</param>
		/// <returns>String of the exception</returns>
		/// <remarks>Calls <see cref="ToString(Exception, bool)"/>, with messageOnly = false</remarks>
		[Obsolete("Use StringUtil.ToString(Exception)")]
		public static string ToString(Exception exception)
		{
			return StringUtil.ToString(exception, false);
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to convert</param>
		/// <param name="messageOnly">If to return the message portions only</param>
		/// <returns>The string</returns>
		[Obsolete("Use StringUtil.ToString(Exception, bool)")]
		public static string ToString(Exception exception, bool messageOnly)
		{
			return StringUtil.ToString(exception, messageOnly);
		}

		/// <summary>
		/// Sets the before log predicate on the logger registered to the specified type.
		/// </summary>
		/// <param name="type">The type of logger.</param>
		/// <param name="predicate">The before log predicate.</param>
		/// <remarks>Setting <paramref name="type"/> to null will cause the predicate to be set on ALL types.</remarks>
		public static void SetBeforeLog(Type type, Action<LogEntry> predicate)
		{
			Logger.GetLogger(type).BeforeLog = predicate;
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Warn(object)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void Warn(Type type, object value)
		{
			Logger.GetLogger(type).Warn(value);
		}

		/// <summary>
		/// Calls <see cref="GetLogger(Type)"/>, then calls <see cref="Warn(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void Warn(Type type, string message, params object[] arguments)
		{
			Logger.GetLogger(type).Warn(message, arguments);
		}

		#endregion
		#region Methods

		/// <summary>
		/// Log critical message, converting the specified value to string.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		public void Critical(object message, params object[] arguments)
		{
			this.Write(LogSeverity.Critical, StringUtil.ToString(message), arguments);
		}

		/// <summary>
		/// Log debug messages, converting the specified value to string. Executes if DEBUG is defined.
		/// </summary>>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG")]
		public void Debug(object value)
		{
			this.Debug(StringUtil.ToString(value));
		}

		/// <summary>
		/// Log debug message. Executes if DEBUG is defined.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG")]
		public void Debug(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Debug, message, arguments);
		}

		/// <summary>
		/// Log error message, converting the specified value to string.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Error(object value)
		{
			this.Error(StringUtil.ToString(value));
		}

		/// <summary>
		/// Log error message.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		public void Error(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, message, arguments);
		}

		/// <summary>
		/// Log exception.
		/// </summary>
		/// <param name="ex">The Exception object to log.</param>
		public void Error(Exception ex)
		{
			string message = StringUtil.ToString(ex, false);
			this.Write(LogSeverity.Error, message);

			if (ex is OutOfMemoryException)
			{
				Thread.CurrentThread.Abort();
			}
		}

		/// <summary>
		/// Log debug message, converting the specified value to string. Executes if DEBUG is defined.
		/// </summary>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public void Info(object value)
		{
			this.Info(StringUtil.ToString(value));
		}

		/// <summary>
		/// Log information message. Executes if DEBUG is defined.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public void Info(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Info, message, arguments);
		}

		/// <summary>
		/// Starts a execution duration profiler
		/// </summary>
		/// <param name="identifier">Some kind of identifier.</param>
		/// <returns>A profiler object</returns>
		public LogProfiler ProfilerStart(string identifier)
		{
			if (StringUtil.IsNullOrWhiteSpace(identifier))
			{
				throw new ArgumentNullException("identifier");
			}

			LogProfiler profiler = new LogProfiler(this, identifier);
			lock (profilers)
			{
				profilers.Add(profiler);
			}

			return profiler;
		}

		/// <summary>
		/// Log warning message, converting the specified value to string. Executes if DEBUG or TRACE is defined.
		/// </summary>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public void Warn(object value)
		{
			this.Warn(StringUtil.ToString(value));
		}

		/// <summary>
		/// Log warning message. Executes if DEBUG or TRACE is defined.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public void Warn(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Warn, message, arguments);
		}

		internal static string Round(double seconds)
		{
			return seconds.ToString("0.000000", System.Globalization.CultureInfo.CurrentCulture);
		}

		internal void ProfilerStop(Guid uid)
		{
			lock (profilers)
			{
				profilers.Remove(profilers.First(p => p.Uid == uid));
			}
		}

		internal void Write(LogSeverity severity, string message, params object[] arguments)
		{
			if (StringUtil.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentNullException("message");
			}

			if (defaultSettings.Types == LogTypes.None)
			{
				return;
			}

			if ((int)defaultSettings.Severity > (int)severity)
			{
				return;
			}

			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.TypeName = this.ClassType.Name;
			entry.Date = DateTime.Now;
			entry.LogTypes = defaultSettings.Types;

			if (arguments == null || arguments.Length == 0)
			{
				entry.Message = message;
			}
			else
			{
				entry.Message = StringUtil.Format(message, arguments);
			}

			if (this.BeforeLog != null)
			{
				this.BeforeLog(entry);
			}

			if ((entry.LogTypes & LogTypes.Console) != LogTypes.None)
			{
				switch (entry.Severity)
				{
					case LogSeverity.Critical:
						Console2.WriteCritical(entry.ToString());
						break;
					case LogSeverity.Debug:
					case LogSeverity.Profile:
						Console2.WriteDebug(entry.ToString());
						break;
					case LogSeverity.Error:
						Console2.WriteError(entry.ToString());
						break;
					case LogSeverity.Info:
						Console2.WriteInfo(entry.ToString());
						break;
					case LogSeverity.Warn:
						Console2.WriteWarning(entry.ToString());
						break;
				}
			}

			if ((entry.LogTypes & LogTypes.Debug) != LogTypes.None)
			{
				System.Diagnostics.Debug.WriteLine(entry);
			}

			if ((entry.LogTypes & LogTypes.Trace) != LogTypes.None)
			{
				Trace.WriteLine(entry);
			}

			if ((entry.LogTypes & LogTypes.Database) != LogTypes.None)
			{
				Logger.SaveToDatabase(entry);
			}

			if ((entry.LogTypes & LogTypes.EventLog) != LogTypes.None)
			{
				Logger.SaveToEventLog(entry);
			}

			if ((entry.LogTypes & LogTypes.Email) != LogTypes.None && !defaultSettings.Email.IsEmpty())
			{
				SmtpUtil.TrySend(defaultSettings.Email.From, defaultSettings.Email.To, defaultSettings.Email.Subject, entry.ToString(), false);
			}
		}

		private static bool SaveToDatabase(LogEntry entry)
		{
			return defaultSettings.Database.Save(entry);
		}

		private static void SaveToEventLog(LogEntry entry)
		{
			string source = defaultSettings.EventLogSource;
			try
			{
				if (!EventLog.SourceExists(source))
				{
					EventLog.CreateEventSource(source, entry.TypeName);
				}
			}
			catch (SecurityException e)
			{
				if (!eventSecurityWarned)
				{
					System.Diagnostics.Debug.WriteLine(SR.EventLogWarning, source, e.Message);
					eventSecurityWarned = true;
				}

				source = "Application";
			}
			
			EventLogEntryType eventLogType;
			switch (entry.Severity)
			{
				case LogSeverity.Critical:
				case LogSeverity.Error:
					eventLogType = EventLogEntryType.Error;
					break;
				case LogSeverity.Warn:
					eventLogType = EventLogEntryType.Warning;
					break;
				default:
					eventLogType = EventLogEntryType.Information;
					break;
			}

			EventLog.WriteEntry(source, entry.ToString(), eventLogType);
		}

		#endregion
	}
}