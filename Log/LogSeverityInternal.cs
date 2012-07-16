using System;

namespace Adenson.Log
{
	internal struct LogSeverityInternal
	{
		#region Variables
		internal static LogSeverityInternal Profiler = new LogSeverityInternal { Value = "PROFILER" };
		private string _value;
		#endregion
		#region Properties

		public LogSeverity Severity
		{
			get;
			set;
		}

		public string Value
		{
			get { return _value == null ? this.Severity.ToString().ToUpper(System.Globalization.CultureInfo.CurrentCulture) : _value; }
			set { _value = value; }
		}

		#endregion
		#region Operators

		public static implicit operator LogSeverity(LogSeverityInternal value)
		{
			if (value.Value == LogSeverityInternal.Profiler.Value)
			{
				return LogSeverity.Debug;
			}

			return value.Severity;
		}

		public static implicit operator LogSeverityInternal(LogSeverity value)
		{
			return new LogSeverityInternal { Severity = value };
		}

		#endregion
		#region Methods

		public override string ToString()
		{
			return String.Concat("[", this.Value, "]").PadRight(9);
		}

		#endregion
	}
}