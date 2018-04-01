#if !NETSTANDARD1_0
using System;
using System.Diagnostics;

namespace Adenson.Log
{
	/// <summary>
	/// Extended <see cref="TraceFilter"/> that will causes only traces entries of type <see cref="LogEntry"/>.
	/// </summary>
	#if !DEBUG
	[DebuggerStepThrough]
	#endif
	public class LogFilter : TraceFilter
	{
		/// <inheritdoc />
		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return data1 is LogEntry;
		}
	}
}
#endif