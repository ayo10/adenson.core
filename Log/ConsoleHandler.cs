using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adenson.Log
{
	/// <summary>
	/// Console writer handler.
	/// </summary>
	public sealed class ConsoleHandler : BaseHandler
	{
		/// <summary>
		/// Writes the log to the console (using <see cref="Console2"/>.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True, regardless.</returns>
		public override bool Write(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			string formatted = this.Formatter.Format(entry);

			switch (entry.Severity)
			{
				case Severity.Critical:
					Console2.WriteCritical(formatted);
					break;
				case Severity.Debug:
					Console2.WriteDebug(formatted);
					break;
				case Severity.Error:
					Console2.WriteError(formatted);
					break;
				case Severity.Info:
					Console2.WriteInfo(formatted);
					break;
				case Severity.Warn:
					Console2.WriteWarning(formatted);
					break;
			}

			return true;
		}
	}
}
