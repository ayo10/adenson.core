using System;

namespace Adenson.Log
{
	/// <summary>
	/// Writes logs to a medium.
	/// </summary>
	public abstract class BaseHandler
	{
		#region Variables
		private BaseFormatter _formatter;
		private Severity? _severity;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseHandler"/> class.
		/// </summary>
		protected BaseHandler()
		{
		}

		#endregion
		#region Properties
		
		/// <summary>
		/// Gets or sets the formatter to use to write the entry.
		/// </summary>
		public BaseFormatter Formatter
		{
			get { return _formatter ?? this.Settings.Formatter; }
			set { _formatter = value; }
		}

		/// <summary>
		/// Gets or sets the severity at which this handler writes (should be equal or greater than the Logger settings severity).
		/// </summary>
		public Severity Severity
		{
			get { return _severity ?? this.Settings.Severity; }
			set { _severity = value; }
		}

		/// <summary>
		/// Gets the parent setting.
		/// </summary>
		public Settings Settings
		{ 
			get; 
			internal set; 
		}

		#endregion
		#region Methods

		/// <summary>
		/// Writes the log to the medium the inheriter specifies.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True if the entry was written successfully, false otherwise.</returns>
		public abstract bool Write(LogEntry entry);

		#endregion
	}
}