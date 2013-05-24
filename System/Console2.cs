using System;
using Adenson.Log;

namespace System
{
	/// <summary>
	/// Basically calling <see cref="System.Console"/> with coloring.
	/// </summary>
	public static class Console2
	{
		/// <summary>
		/// Writes the specified error.
		/// </summary>
		/// <param name="message">The error message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteCritical(string message, params object[] arguments)
		{
			Console2.WriteLine(ConsoleColor.DarkRed, message, arguments);
		}

		/// <summary>
		/// Writes the specified debug message.
		/// </summary>
		/// <param name="message">The info message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteDebug(string message, params object[] arguments)
		{
			Console2.WriteLine(ConsoleColor.Gray, message, arguments);
		}

		/// <summary>
		/// Writes the specified info.
		/// </summary>
		/// <param name="message">The info message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteInfo(string message, params object[] arguments)
		{
			Console2.WriteLine(Console.ForegroundColor, message, arguments);
		}

		/// <summary>
		/// Writes the specified error.
		/// </summary>
		/// <param name="message">The error message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteError(string message, params object[] arguments)
		{
			Console2.WriteLine(ConsoleColor.Red, message, arguments);
		}

		/// <summary>
		/// Writes the specified warning.
		/// </summary>
		/// <param name="message">The error message to write.</param>
		/// <param name="arguments"> An array of objects to write using format.</param>
		public static void WriteWarning(string message, params object[] arguments)
		{
			Console2.WriteLine(ConsoleColor.DarkYellow, message, arguments);
		}

		/// <summary>
		/// Writes the specified exception.
		/// </summary>
		/// <param name="ex">The exception to write.</param>
		public static void WriteException(Exception ex)
		{
			Console2.WriteError(StringUtil.ToString(ex));
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