using System;
using System.Collections.Generic;
using Adenson.Log;

namespace Adenson.CoreTest.Log
{
	public class TestHandler : BaseHandler
	{
		public TestHandler()
		{
			this.Entries = new List<LogEntry>();
		}

		public List<LogEntry> Entries
		{
			get;
			private set;
		}

		public override bool Write(LogEntry entry)
		{
			this.Entries.Add(entry);
			Console.WriteLine(this.Formatter.Format(entry));
			return true;
		}
	}
}