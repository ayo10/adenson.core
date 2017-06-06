using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Adenson.Log
{
	/// <summary>
	/// Sends logs to the event log.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public sealed class EventLogHandler : BaseHandler
	{
		#region Constants
		internal const int DefaultEventId = 1026;
		private static bool createEventSourceLogged;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogHandler"/> class.
		/// </summary>
		public EventLogHandler() : this("Application", EventLogHandler.DefaultEventId)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogHandler"/> class.
		/// </summary>
		/// <param name="source">The event log source.</param>
		/// <param name="eventId">The event id to use.</param>
		public EventLogHandler(string source, int eventId) : base()
		{
			this.Source = Arg.IsNotNull(source);
			this.EventId = Arg.IsValid(eventId, v => v >= 0 && eventId <= UInt16.MaxValue, "Event Ids must be between 0 and 65535");
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the event log source to use.
		/// </summary>
		public string Source
		{ 
			get; 
			private set; 
		}

		/// <summary>
		/// Gets the event id to use.
		/// </summary>
		public int EventId
		{
			get;
			private set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Writes the log to the event log.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True if the log was written successfully, false otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not fail regardless of any exception.")]
		public override bool Write(LogEntry entry)
		{
			Arg.IsNotNull(entry, "entry");
			string source = this.Source;
			try
			{
				if (!EventLog.SourceExists(source))
				{
					EventLog.CreateEventSource(source, entry.TypeName);
				}
			}
			catch (SecurityException e)
			{
				if (!createEventSourceLogged)
				{
					Trace.TraceError(StringUtil.Format(SR.EventLogWarning, this.Source, e.Message));
					createEventSourceLogged = true;
				}

				source = ".NET Runtime";
			}

			EventLogEntryType eventLogType;
			switch (entry.Severity)
			{
				case Severity.Critical:
				case Severity.Error:
					eventLogType = EventLogEntryType.Error;
					break;
				case Severity.Warn:
					eventLogType = EventLogEntryType.Warning;
					break;
				default:
					eventLogType = EventLogEntryType.Information;
					break;
			}

			try
			{
				EventLog.WriteEntry(source, this.Formatter.Format(entry), eventLogType, this.EventId);
			}
			catch (Exception ex)
			{
				Trace.TraceError(StringUtil.ToString(ex));
				return false;
			}

			return true;
		}

		#endregion
	}
}