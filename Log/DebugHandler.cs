#define DEBUG
using System;
using System.Diagnostics;

namespace Adenson.Log
{
	/// <summary>
	/// Debug writer handler.
	/// </summary>
	public sealed class DebugHandler : BaseHandler
	{
		/// <summary>
		/// Writes the log to the diagnostics debug (using <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True, regardless.</returns>
		public override bool Write(LogEntry entry)
		{
			Debug.WriteLine(this.Formatter.Format(entry));
			return true;
		}
	}
}