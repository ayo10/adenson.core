using System;
using System.Globalization;

namespace Adenson.Log
{
	internal struct LogSeverityInternal
	{
		internal static LogSeverityInternal Measure = new LogSeverityInternal { Value = "MEASURE" };
		private string _value;
		public LogSeverity Severity
		{
			get;
			set;
		}
		public string Value
		{
			get { return _value == null ? this.Severity.ToString().ToUpper(CultureInfo.CurrentCulture) : _value; }
			set { _value = value; }
		}

		public override string ToString()
		{
			return String.Concat("[", this.Value, "]").PadRight(9);
		}
		public static implicit operator LogSeverity(LogSeverityInternal value)
		{
			if (value.Value == "MEASURE") return LogSeverity.Debug;
			return value.Severity;
		}
		public static implicit operator LogSeverityInternal(LogSeverity value)
		{
			return new LogSeverityInternal { Severity = value };
		}
	}
}