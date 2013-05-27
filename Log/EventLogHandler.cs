using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;

namespace Adenson.Log
{
	/// <summary>
	/// Sends logs to the event log.
	/// </summary>
	public sealed class EventLogHandler : BaseHandler
	{
		#region Constructor

		internal EventLogHandler(SettingsConfiguration.HandlerElement element) : base()
		{
			this.Source = element.GetValue("source", "Application");
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the event log source to use (defaults to 'Application').
		/// </summary>
		public string Source
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
		public override bool Write(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			try
			{
				if (!EventLog.SourceExists(this.Source))
				{
					EventLog.CreateEventSource(this.Source, entry.TypeName);
				}
			}
			catch (SecurityException e)
			{
				Debug.WriteLine(SR.EventLogWarning, this.Source, e.Message);
				return false;
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

			EventLog.WriteEntry(this.Source, this.Formatter.Format(entry), eventLogType);
			return true;
		}

		#endregion
	}
}