using System;

namespace Adenson.Log
{
	/// <summary>
	/// The built-in default <see cref="BaseFormatter"/> object.
	/// </summary>
	public class DefaultFormatter : BaseFormatter
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
			return $"{entry.Date:H:mm:ss.fff} {entry.Severity.ToString().Substring(0, 1)} {entry.TypeName} {this.ToString(entry.Message)}";
		}

		#endregion
	}
}