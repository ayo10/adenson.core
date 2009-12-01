using System;

namespace Adenson.Log
{
	/// <summary>
	/// Represents a log entry
	/// </summary>
	public class LogEntry
	{
		#region Variables
		private LogType _logType;
		private LogSeverity _severity;
		private string _source, _message, _path;
		private DateTime _date;
		private Type _type;
		#endregion
		#region Properties

		/// <summary>
		/// Gets the severity
		/// </summary>
		public LogSeverity Severity
		{
			get { return _severity; }
			internal set { _severity = value; }
		}
		/// <summary>
		/// Gets the path
		/// </summary>
		public string Path
		{
			get { return _path; }
			internal set { _path = value; }
		}
		/// <summary>
		/// Gets the message
		/// </summary>
		public string Message
		{
			get { return _message; }
			internal set { _message = value; }
		}
		/// <summary>
		/// Gets the source
		/// </summary>
		public string Source
		{
			get { return _source; }
			internal set { _source = value; }
		}
		/// <summary>
		/// Gets the date
		/// </summary>
		public DateTime Date
		{
			get { return _date; }
			internal set { _date = value; }
		}
		/// <summary>
		/// Gets the type
		/// </summary>
		public Type Type
		{
			get { return _type; }
			internal set { _type = value; }
		}
		/// <summary>
		/// Gets the type
		/// </summary>
		internal LogType LogType
		{
			get { return _logType; }
			set { _logType = value; }
		}

		#endregion
	}
}