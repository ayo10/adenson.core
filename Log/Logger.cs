using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	[DebuggerStepThrough]
	public sealed class Logger
	{
		#region Variables
		private static Dictionary<Type, Logger> staticLoggers = new Dictionary<Type, Logger>();
		private static LoggerSettings defaultSettings = LoggerSettings.ReadSettings();
		private Type _classType;
		private List<LogProfiler> profilers = new List<LogProfiler>();
		#endregion
		#region Constructors

		private Logger(Type classType)
		{
			if (classType == null)
			{
				throw new ArgumentNullException("classType");
			}

			_classType = classType;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the type form which this instance is forged from
		/// </summary>
		public Type ClassType
		{
			get { return _classType; }
		}

		#endregion
		#region Public Static Methods

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
		/// Flushes the output buffer, and causes buffered data to be written.
		/// </summary>
		public static void Flush()
		{
			Trace.Flush();
		}

		/// <summary>
		/// Gets a pre initialized (or new) Logger for specified type
		/// </summary>
		/// <param name="type">The type</param>
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
					staticLoggers.Add(type, new Logger(type));
				}

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
		/// Instantiates a Logger object, then calls <see cref="ProfilerStart(string)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on</param>
		/// <param name="identifier">Some kind of identifier</param>
		/// <returns>A disposable profiler object</returns>
		public static LogProfiler ProfilerStart(Type type, string identifier)
		{
			return Logger.GetLogger(type).ProfilerStart(identifier);
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to convert</param>
		/// <returns>String of the exception</returns>
		/// <remarks>Calls <see cref="ToString(Exception, bool)"/>, with messageOnly = false</remarks>
		public static string ToString(Exception exception)
		{
			return Logger.ToString(exception, false);
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null
		/// </summary>
		/// <param name="exception">The Exception object to convert</param>
		/// <param name="messageOnly">If to return the message portions only</param>
		/// <returns>The string</returns>
		public static string ToString(Exception exception, bool messageOnly)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}

			StringBuilder message = new StringBuilder();
			Exception ex = exception;
			while (ex != null)
			{
				if (message.Length != 0)
				{
					if (messageOnly)
					{
						message.Append(" ");
					}
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
					if (ex.StackTrace != null)
					{
						message.AppendLine(ex.StackTrace);
					}
				}

				ex = ex.InnerException;
			}

			return message.ToString();
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

		#endregion
		#region Methods

		/// <summary>
		/// Log debug messages, converting the specified value to string.
		/// </summary>>
		/// <param name="value">The value</param>
		[Conditional("DEBUG")]
		public void Debug(object value)
		{
			this.Debug(StringUtil.ToString(value));
		}

		/// <summary>
		/// Log debug message
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG")]
		public void Debug(string message, params object[] arguments)
		{
			if ((int)defaultSettings.Severity > (int)LogSeverity.Debug)
			{
				return;
			}

			this.Write(LogSeverity.Debug, message, arguments);
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
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG")]
		public void Error(string message, params object[] arguments)
		{
			this.Write(LogSeverity.Error, message, arguments);
		}

		/// <summary>
		/// Log exception
		/// </summary>
		/// <param name="ex">The Exception object to log</param>
		[Conditional("DEBUG")]
		public void Error(Exception ex)
		{
			string message = Logger.ToString(ex);
			this.Write(LogSeverity.Error, message);

			if (ex is OutOfMemoryException)
			{
				Thread.CurrentThread.Abort();
			}
		}

		/// <summary>
		/// Log debug message, converting the specified value to string.
		/// </summary>
		/// <param name="value">The value</param>
		[Conditional("DEBUG")]
		public void Info(object value)
		{
			this.Info(StringUtil.ToString(value));
		}

		/// <summary>
		/// Log information message
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG")]
		public void Info(string message, params object[] arguments)
		{
			if ((int)defaultSettings.Severity > (int)LogSeverity.Info)
			{
				return;
			}

			this.Write(LogSeverity.Info, message, arguments);
		}

		/// <summary>
		/// Starts a execution duration profiler
		/// </summary>
		/// <param name="identifier">Some kind of identifier</param>
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
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		public void Warn(string message, params object[] arguments)
		{
			if ((int)defaultSettings.Severity > (int)LogSeverity.Warn)
			{
				return;
			}

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

		internal void Write(LogSeverityInternal severity, string message, params object[] arguments)
		{
			if (StringUtil.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentNullException("message");
			}

			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.TypeName = this.ClassType.Name;
			entry.Source = defaultSettings.Source;
			entry.Date = DateTime.Now;
			entry.LogType = defaultSettings.Types;

			if (arguments == null || arguments.Length == 0)
			{
				entry.Message = message;
			}
			else
			{
				entry.Message = StringUtil.Format(message, arguments);
			}

			Logger.OutWriteLine(entry);
		}

		private static void OutWriteLine(LogEntry entry)
		{
			if ((entry.LogType & LogTypes.Console) != LogTypes.None)
			{
				Console.WriteLine(entry);
			}

			if ((entry.LogType & LogTypes.Trace) != LogTypes.None)
			{
				Trace.WriteLine(entry);
			}

			if ((entry.LogType & LogTypes.Database) != LogTypes.None)
			{
				Logger.SaveToDatabase(entry);
			}

			if ((entry.LogType & LogTypes.Email) != LogTypes.None && !defaultSettings.EmailInfo.IsEmpty())
			{
				SmtpUtil.TrySend(defaultSettings.EmailInfo.From, defaultSettings.EmailInfo.To, defaultSettings.EmailInfo.Subject, entry.ToString(), false);
			}
		}

		private static bool SaveToDatabase(LogEntry entry)
		{
			return defaultSettings.DatabaseInfo.Save(entry);
		}

		#endregion
	}
}