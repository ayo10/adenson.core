using System;
using System.Globalization;
using System.Linq;

namespace Adenson.Log
{
	/// <summary>
	/// The built-in default <see cref="BaseFormatter"/> object.
	/// </summary>
	public class DefaultFormatter : BaseFormatter
	{
		#region Variables
		private static string format = DefaultFormatter.GetDefaultFormat();
		#endregion
		#region Methods

		/// <summary>
		/// Formats the given entry.
		/// </summary>
		/// <param name="entry">The log entry object to format.</param>
		/// <returns>The formatted string.</returns>
		public override string Format(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			return StringUtil.Format(format, "[" + entry.Severity.ToString().ToUpper(CultureInfo.CurrentCulture) + "]", entry.Date, entry.TypeName, this.ToString(entry.Message));
		}

		private static string GetDefaultFormat()
		{
			return "{Date:H:mm:ss.fff} {Severity,-8} {TypeName,-10} {Message}"
									.Replace("{Severity", "{0")
									.Replace("{Date", "{1")
									.Replace("{TypeName", "{2")
									.Replace("{Message", "{3");
		}

		#endregion
	}
}