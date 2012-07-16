using System;
using System.Diagnostics;

namespace Adenson.Log
{
	internal class LogFilter : TraceFilter
	{
		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return data1 is LogEntry;
		}
	}
}