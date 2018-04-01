#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_3 && !NETSTANDARD1_0
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Adenson.Log
{
	/// <summary>
	/// Sends logs to the event log.
	/// </summary>
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
					Trace.TraceError($@"An error occured when trying to create a Event Log with source '{this.Source}'. Defaulting to '.NET Runtime'. Error is '{e.Message}'." +
						$"See https://msdn.microsoft.com/en-us/library/6s7642se.aspx for a solution, the short of it being you will need to give the user under" +
						$"which the program is running rights to be able to read and write to the" +
						$"registry key HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Eventlog\\Security." +
						$"{Environment.NewLine}Or add to the registry, the following:" +
						$"{Environment.NewLine}[HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\EventLog\\Application\\{this.Source}]" +
						$"\"EventMessageFile\"=hex(2):25,00,53,00,79,00,73,00,74,00,65,00,6d,00,52,00,6f,\\" +
						$"  00,6f,00,74,00,25,00,5c,00,53,00,79,00,73,00,74,00,65,00,6d,00,33,00,32,00,\\" +
						$"  5c,00,45,00,76,00,65,00,6e,00,74,00,43,00,72,00,65,00,61,00,74,00,65,00,2e,\\" +
						$"  00,65,00,78,00,65,00,00,00,00,00" +
						$"{Environment.NewLine}Or, execute from the command line (as an administrator, of course). It will add a new error" +
						$"entry to the event log, but it will also set up the necessary registry entries." +
						$"{Environment.NewLine}EventCreate /t Error /d test /so {this.Source} /l Application /id 1");
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
#endif