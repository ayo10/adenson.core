using System;
using System.Diagnostics;

namespace Adenson.Log
{
	/// <summary>
	/// Shortcuts to Logger methods.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Critical(object)"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">The message to log.</param>
		public static void LogCritical<T>(this T source, object message) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Critical(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Critical(string, object[])"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		public static void LogCritical<T>(this T source, string message, params object[] arguments) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Critical(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Critical(object)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">The message to log.</param>
		public static void LogCritical(this Type type, object message)
		{
			Logger.Get(type).Critical(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Critical(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		public static void LogCritical(this Type type, string message, params object[] arguments)
		{
			Logger.Get(type).Critical(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Debug(object)"/>.
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		[Conditional("DEBUG")]
		public static void LogDebug<T>(this T source, object message) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Debug(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Debug(string, object[])"/>.
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG")]
		public static void LogDebug<T>(this T source, string message, params object[] arguments) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Debug(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Debug(object)"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		[Conditional("DEBUG")]
		public static void LogDebug(this Type type, object message)
		{
			Logger.Get(type).Debug(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Debug(string, object[])"/>.
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG")]
		public static void LogDebug(this Type type, string message, params object[] arguments)
		{
			Logger.Get(type).Debug(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Error(object)"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		public static void LogError<T>(this T source, object message) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Error(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Error(string, object[])"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		public static void LogError<T>(this T source, string message, params object[] arguments) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Error(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/> then <see cref="Logger.Error(object)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		public static void LogError(this Type type, object message)
		{
			Logger.Get(type).Error(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Error(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		public static void LogError(this Type type, string message, params object[] arguments)
		{
			Logger.Get(type).Error(message, arguments);
		}

		/// <summary>
		/// Instantiates a Logger object, then calls <see cref="Logger.GetProfiler(string)"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="identifier">Some kind of identifier.</param>
		/// <returns>A disposable profiler object</returns>
		public static LogProfiler LogGetProfiler<T>(this T source, string identifier) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			return Logger.Get(source.GetType()).GetProfiler(identifier);
		}

		/// <summary>
		/// Instantiates a Logger object, then calls <see cref="Logger.GetProfiler(string)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="identifier">Some kind of identifier.</param>
		/// <returns>A disposable profiler object</returns>
		public static LogProfiler LogGetProfiler(this Type type, string identifier)
		{
			return Logger.Get(type).GetProfiler(identifier);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Info(string, object[])"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogInfo<T>(this T source, string message, params object[] arguments) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Info(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Info(object)"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogInfo<T>(this T source, object message) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Info(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Info(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogInfo(this Type type, string message, params object[] arguments)
		{
			Logger.Get(type).Info(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Info(object)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogInfo(this Type type, object message)
		{
			Logger.Get(type).Info(message);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Warn(object)"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogWarn<T>(this T source, object value) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Warn(value);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Warn(string, object[])"/>
		/// </summary>
		/// <param name="source">The object, whose type, Logger would be called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogWarn<T>(this T source, string message, params object[] arguments) where T : class
		{
			if (source == null)
			{
				throw new ArgumentNullException("obj");
			}

			Logger.Get(source.GetType()).Warn(message, arguments);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Warn(object)"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="value">The value.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogWarn(this Type type, object value)
		{
			Logger.Get(type).Warn(value);
		}

		/// <summary>
		/// Calls <see cref="Logger.Get(Type)"/>, then calls <see cref="Logger.Warn(string, object[])"/>
		/// </summary>
		/// <param name="type">Type where Logger is being called on.</param>
		/// <param name="message">Message to log.</param>
		/// <param name="arguments">Arguments, if any to format message.</param>
		[Conditional("DEBUG"), Conditional("INFO"), Conditional("TRACE")]
		public static void LogWarn(this Type type, string message, params object[] arguments)
		{
			Logger.Get(type).Warn(message, arguments);
		}
	}
}