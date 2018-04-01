#if !NETSTANDARD1_0
using System;
using System.Diagnostics;

namespace Adenson.Log
{
	/// <summary>
	/// Trace writer handler.
	/// </summary>
	public sealed class TraceHandler : BaseHandler
	{
		/// <summary>
		/// Writes the log to the diagnostics trace (using <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True, regardless.</returns>
		public override bool Write(LogEntry entry)
		{
			Trace.WriteLine(this.Formatter.Format(entry));
			return true;
		}
	}
}
#endif