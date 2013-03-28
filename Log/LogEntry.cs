using System;
using System.Globalization;
using Adenson.Log.Config;

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
			return this.ToString(LoggerSettings.Default.Format);
		}

		public string ToString(string format)
		{
			return StringUtil.Format(format, this.Severity.ToString().ToUpper(CultureInfo.CurrentCulture), this.Date, this.TypeName, this.Message);
		}

		#endregion
	}
}