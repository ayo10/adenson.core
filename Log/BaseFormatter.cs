using System;

namespace Adenson.Log
{
	/// <summary>
	/// Base class for providing suppport for formatting <see cref="LogEntry"/> objects.
	/// </summary>
	public abstract class BaseFormatter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseFormatter"/> class.
		/// </summary>
		protected BaseFormatter()
		{
		}

		/// <summary>
		/// Formats the given entry.
		/// </summary>
		/// <param name="entry">The log entry object to format.</param>
		/// <returns>The formatted string.</returns>
		public abstract string Format(LogEntry entry);
	}
}