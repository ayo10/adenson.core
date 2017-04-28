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
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogHandler"/> class.
		/// </summary>
		public EventLogHandler() : this("Application")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogHandler"/> class.
		/// </summary>
		/// <param name="source">The event log source.</param>
		public EventLogHandler(string source) : base()
		{
			this.Source = Arg.IsNotNull(source);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogHandler"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		internal EventLogHandler(SettingsConfiguration.HandlerElement element) : this(element.GetValue("source", "Application"))
		{
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

			try
			{
				if (!EventLog.SourceExists(this.Source))
				{
					EventLog.CreateEventSource(this.Source, entry.TypeName);
				}
			}
			catch (SecurityException e)
			{
				Debug.WriteLine(StringUtil.Format(SR.EventLogWarning, this.Source, e.Message));
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

			try
			{
				EventLog.WriteEntry(this.Source, this.Formatter.Format(entry), eventLogType);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(StringUtil.ToString(ex));
				return false;
			}

			return true;
		}

		#endregion
	}
}