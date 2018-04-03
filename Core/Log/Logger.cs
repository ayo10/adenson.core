using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Adenson.Log
{	
	/// <summary>
	/// Logger of .... well, logs
	/// </summary>
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
			this.ClassType = Arg.IsNotNull(classType);
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
		/// Gets the type form which this instance is forged from.
		/// </summary>
		public Type ClassType
		{
			get;
			private set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Calls <see cref="Get(Type)"/> then <see cref="Critical(object)"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">The message to log.</param>
		public static void Critical(Type type, object message)
		{
			Logger.Get(type).Critical(message);
		}
		

		/// <summary>
		/// Calls <see cref="Get(Type)"/> then <see cref="Debug(object)"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		[Conditional("DEBUG")]
		public static void Debug(Type type, object message)
		{
			Logger.Get(type).Debug(message);
		}

		/// <summary>
		/// Calls <see cref="Get(Type)"/> then <see cref="Error(object)"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		public static void Error(Type type, object message)
		{
			Logger.Get(type).Error(message);
		}

		/// <summary>
		/// Gets a pre initialized (or new) Logger for specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>Existing, or newly minted logger</returns>
		public static Logger Get(Type type)
		{
			Arg.IsNotNull(type);
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
		/// Calls <see cref="Get(Type)"/>, then calls <see cref="Info(object)"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		public static void Info(Type type, object message)
		{
			Logger.Get(type).Info(message);
		}

		/// <summary>
		/// Calls <see cref="Get(Type)"/>, then calls <see cref="Warn(object)"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="value">The value.</param>
		public static void Warn(Type type, object value)
		{
			Logger.Get(type).Warn(value);
		}

		/// <summary>
		/// Logs critical message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		public void Critical(object message)
		{
			this.Write(Severity.Critical, message);
		}

		/// <summary>
		/// Log debug messages. Executes if DEBUG is defined.
		/// </summary>>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG")]
		public void Debug(object value)
		{
			this.Write(Severity.Debug, value);
		}

		/// <summary>
		/// Log error message.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Error(object value)
		{
			this.Write(Severity.Error, value);
		}

		/// <summary>
		/// Log exception.
		/// </summary>
		/// <param name="ex">The Exception object to log.</param>
		public void Error(Exception ex)
		{
			this.Write(Severity.Error, ex);
		}

		/// <summary>
		/// Starts a execution duration profiler.
		/// </summary>
		/// <param name="identifier">Some kind of identifier.</param>
		/// <returns>A profiler object</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public LogProfiler Profiler(string identifier)
		{
			Arg.IsNotEmpty(identifier);
			LogProfiler profiler = new LogProfiler(this, identifier);
			profiler.Disposed = () =>
			{
				lock (profilers)
				{
					profilers.Remove(profiler);
				}
			};

			lock (profilers)
			{
				profilers.Add(profiler);
			}

			return profiler;
		}

		/// <summary>
		/// Log debug message. Executes if DEBUG is defined.
		/// </summary>
		/// <param name="value">The value.</param>
		[Conditional("TRACE")]
		public void Info(object value)
		{
			this.Write(Severity.Info, value);
		}

		/// <summary>
		/// Log warning message. Executes if DEBUG or TRACE is defined.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Warn(object value)
		{
			this.Write(Severity.Warn, value);
		}

		internal static string Round(TimeSpan time)
		{
			if (time.TotalMilliseconds < 1000)
			{
				return $"{time.TotalMilliseconds.Round(4)} ms";
			}
			else if (time.TotalSeconds < 60)
			{
				return $"{time.TotalSeconds.Round(4)} s";
			}
			else if (time.TotalMinutes < 60)
			{
				return $"{time.TotalMinutes.Round(4)} m";
			}

			return $"{time.TotalHours.Round(4)} h";
		}

		internal void Write(Severity severity, object message)
		{
			if (_settings.Handlers.Count == 0)
			{
				return;
			}

			if ((int)_settings.Severity > (int)severity)
			{
				return;
			}

			LogEntry entry = new LogEntry
			{
				Severity = severity,
				TypeName = this.ClassType.Name,
				Date = DateTime.Now,
				Message = message
			};
			foreach (BaseHandler handler in _settings.Handlers)
			{
				if ((int)handler.Severity > (int)severity)
				{
					continue;
				}

				handler.Write(entry);
			}
		}

		private static Settings ReadSettings()
		{
#if NETSTANDARD2_0 || NETSTANDARD1_6 || NETSTANDARD1_5 || NETSTANDARD1_3 || NETSTANDARD1_0
			return new Settings();
#else
			return Settings.FromConfigSection("adenson/logSettings") ?? Settings.FromConfigSection("logSettings") ?? Settings.FromAppSettings() ?? new Settings();
#endif
		}

		#endregion
	}
}