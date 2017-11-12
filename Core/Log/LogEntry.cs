using System;

namespace Adenson.Log
{
	/// <summary>
	/// Represents a log entry
	/// </summary>
	public class LogEntry
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="LogEntry"/> class.
		/// </summary>
		public LogEntry()
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the severity of the message.
		/// </summary>
		public Severity Severity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the message to log.
		/// </summary>
		public object Message
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		public DateTime Date
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		public string Caller
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the class type name the logger was created with.
		/// </summary>
		public string TypeName
		{
			get;
			set;
		}

		#endregion
	}
}