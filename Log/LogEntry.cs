using System;
using System.Globalization;
using Adenson.Log.Config;

namespace Adenson.Log
{
	/// <summary>
	/// Represents a log entry
	/// </summary>
	public sealed class LogEntry
	{
		#region Constructor

		internal LogEntry()
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the severity of the message.
		/// </summary>
		public LogSeverity Severity
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the message to log.
		/// </summary>
		public string Message
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the date.
		/// </summary>
		public DateTime Date
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the type name.
		/// </summary>
		public string TypeName
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets additional information to add to the end of the log.
		/// </summary>
		public string Addendum
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the log type
		/// </summary>
		internal LogTypes LogTypes
		{
			get;
			set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Gets a string representation of the log entry object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.ToString(LoggerSettings.Default.Format);
		}

		/// <summary>
		/// Gets a string representation of the log entry object.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <returns>A string that represents the current object.</returns>
		public string ToString(string format)
		{
			base.ToString();
			return StringUtil.Format(format, this.Severity.ToString().ToUpper(CultureInfo.CurrentCulture), this.Date, this.TypeName, this.Message, this.Addendum);
		}

		#endregion
	}
}