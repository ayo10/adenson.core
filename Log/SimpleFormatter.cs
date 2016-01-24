using System;
using System.Diagnostics;

namespace Adenson.Log
{
	/// <summary>
	/// The built-in <see cref="BaseFormatter"/> object that just prints LogEntry.Message, ignoring everything else.
	/// </summary>
	public class SimpleFormatter : BaseFormatter
	{
		#region Methods

		/// <summary>
		/// Formats the given entry.
		/// </summary>
		/// <param name="entry">The log entry object to format.</param>
		/// <returns>The formatted string.</returns>
		public override string Format(LogEntry entry)
		{
			Arg.IsNotNull(entry, "entry");
			return entry.Message.ToString();
		}

		#endregion
	}
}