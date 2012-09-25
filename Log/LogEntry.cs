using System;
using System.Globalization;

namespace Adenson.Log
{
	/// <summary>
	/// Represents a log entry
	/// </summary>
	internal sealed class LogEntry
	{
		#region Constructor

		internal LogEntry()
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the severity
		/// </summary>
		public LogSeverity Severity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the message
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the source
		/// </summary>
		public string Source
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date
		/// </summary>
		public DateTime Date
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type name
		/// </summary>
		public string TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the log type
		/// </summary>
		internal LogTypes LogType
		{
			get;
			set;
		}

		#endregion
		#region Methods

		public override string ToString()
		{
			var format = "{0}\t{1}\t[{2}]\t {3}";
			var date = this.Date.ToString("H:mm:ss.fff", CultureInfo.CurrentCulture);
			return StringUtil.Format(format, this.Severity.ToString(), date, this.TypeName, this.Message);
		}

		#endregion
	}
}