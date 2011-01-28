using System;

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
		/// Gets the severity
		/// </summary>
		public LogSeverity Severity
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the type
		/// </summary>
		internal LogTypes LogType
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the message
		/// </summary>
		public string Message
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the source
		/// </summary>
		public string Source
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the date
		/// </summary>
		public DateTime Date
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the type
		/// </summary>
		public Type Type
		{
			get;
			set;
		}

		#endregion
	}
}