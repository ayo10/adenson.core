using System;

namespace Adenson.Log
{
	internal struct LogSeverityInternal
	{
		#region Variables
		internal static LogSeverityInternal Default = new LogSeverityInternal { Value = "PROFILE" };
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
			get { return _value == null ? this.Severity.ToString().ToUpper() : _value; }
			set { _value = value; }
		}

		#endregion
		#region Methods

		public override string ToString()
		{
			return String.Concat("[", this.Value, "]").PadRight(9);
		}

		#endregion
		#region Operators

		public static implicit operator LogSeverity(LogSeverityInternal value)
		{
			if (value.Value == LogSeverityInternal.Default.Value) return LogSeverity.Debug;
			return value.Severity;
		}
		public static implicit operator LogSeverityInternal(LogSeverity value)
		{
			return new LogSeverityInternal { Severity = value };
		}

		#endregion
	}
}