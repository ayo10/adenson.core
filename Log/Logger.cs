using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log
{
	/// <summary>
	/// Logger of .... well, logs
	/// </summary>
	#if !DEBUG
	[DebuggerStepThrough]
	#endif
	public sealed class Logger
	{
		#region Variables
		private static Dictionary<Type, Logger> staticLoggers = new Dictionary<Type, Logger>();
		private static Settings _settings = Logger.ReadSettings();
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
		/// Gets the global settings.
		/// </summary>
		public static Settings Settings
		{
			get { return _settings; }
		}

		/// <summary>
		/// Gets the type form which this instance is forged from
		/// </summary>
		public Type ClassType
		{
			get;
			private set;
		}

		#endregion
		#region Methods

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
					Logger logger = new Logger(type);
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

		/// <summary>
		/// Log critical message, converting the specified value to string.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		public void Critical(object message, params object[] arguments)
		{
			this.Write(Severity.Critical, StringUtil.ToString(message), arguments);
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
			this.Write(Severity.Debug, message, arguments);
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
			this.Write(Severity.Error, message, arguments);
		}

		/// <summary>
		/// Log exception.
		/// </summary>
		/// <param name="ex">The Exception object to log.</param>
		public void Error(Exception ex)
		{
			string message = StringUtil.ToString(ex, false);
			this.Write(Severity.Error, message);

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
			this.Write(Severity.Info, message, arguments);
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
			this.Write(Severity.Warn, message, arguments);
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

		internal void Write(Severity severity, string message, params object[] arguments)
		{
			if (StringUtil.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentNullException("message");
			}

			if (_settings.Handlers.Count == 0)
			{
				return;
			}

			if ((int)_settings.Severity > (int)severity)
			{
				return;
			}

			LogEntry entry = new LogEntry();
			entry.Severity = severity;
			entry.TypeName = this.ClassType.Name;
			entry.Date = DateTime.Now;
			entry.Message = arguments == null || arguments.Length == 0 ? message : StringUtil.Format(message, arguments);

			foreach (BaseHandler handler in _settings.Handlers)
			{
				handler.Write(entry);
			}
		}

		private static Settings ReadSettings()
		{
			SettingsConfiguration config = (SettingsConfiguration)ConfigurationManager.GetSection("adenson/logSettings");
			return Settings.FromConfig(config);
		}

		#endregion
	}
}