#if !NETSTANDARD1_0
using System;

namespace Adenson.Log
{
	/// <summary>
	/// Console writer handler.
	/// </summary>
	public sealed class ConsoleHandler : BaseHandler
	{
		/// <summary>
		/// Writes the specified error.
		/// </summary>
		/// <param name="message">The error message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteCritical(string message, params object[] arguments)
		{
			ConsoleHandler.WriteLine(ConsoleColor.DarkRed, message, arguments);
		}

		/// <summary>
		/// Writes the specified debug message.
		/// </summary>
		/// <param name="message">The info message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteDebug(string message, params object[] arguments)
		{
			ConsoleHandler.WriteLine(ConsoleColor.Gray, message, arguments);
		}

		/// <summary>
		/// Writes the specified info.
		/// </summary>
		/// <param name="message">The info message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteInfo(string message, params object[] arguments)
		{
			ConsoleHandler.WriteLine(Console.ForegroundColor, message, arguments);
		}

		/// <summary>
		/// Writes the specified error.
		/// </summary>
		/// <param name="message">The error message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteError(string message, params object[] arguments)
		{
			ConsoleHandler.WriteLine(ConsoleColor.Red, message, arguments);
		}

		/// <summary>
		/// Writes the specified warning.
		/// </summary>
		/// <param name="message">The error message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteWarning(string message, params object[] arguments)
		{
			ConsoleHandler.WriteLine(ConsoleColor.Yellow, message, arguments);
		}

		/// <summary>
		/// Writes the specified exception.
		/// </summary>
		/// <param name="ex">The exception to write.</param>
		public static void WriteException(Exception ex)
		{
			ConsoleHandler.WriteError(StringUtil.ToString(ex));
		}

		/// <summary>
		/// Writes the log to the console (using <see cref="Console"/>.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True, regardless.</returns>
		public override bool Write(LogEntry entry)
		{
			Arg.IsNotNull(entry, "entry");
			string formatted = this.Formatter.Format(entry);
			switch (entry.Severity)
			{
				case Severity.Critical:
					ConsoleHandler.WriteCritical(formatted);
					break;
				case Severity.Debug:
					ConsoleHandler.WriteDebug(formatted);
					break;
				case Severity.Error:
					ConsoleHandler.WriteError(formatted);
					break;
				case Severity.Info:
					ConsoleHandler.WriteInfo(formatted);
					break;
				case Severity.Warn:
					ConsoleHandler.WriteWarning(formatted);
					break;
			}

			return true;
		}

		private static void WriteLine(ConsoleColor color, string message, params object[] arguments)
		{
			var origcolor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(message, arguments);
			Console.ForegroundColor = origcolor;
		}
	}
}
#endif