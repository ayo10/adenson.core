using System;
using Adenson.Log;

namespace Adenson.CoreTest.Log
{
	public class TestFormatter : BaseFormatter
	{
		public override string Format(LogEntry entry)
		{
			return String.Empty;
		}
	}
}