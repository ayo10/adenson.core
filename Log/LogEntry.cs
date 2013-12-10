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
		/// Gets the severity of the message.
		/// </summary>
		public Severity Severity
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the message to log.
		/// </summary>
		public string Message
		{
			get;
			set;
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
		/// Gets the class type name the logger was created with.
		/// </summary>
		public string TypeName
		{
			get;
			internal set;
		}

		#endregion
	}
}