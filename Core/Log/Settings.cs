using System;

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	public sealed partial class Settings
	{
		#region Variables
		private BaseFormatter _formatter;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Settings"/> class.
		/// </summary>
		public Settings()
		{
			this.Handlers = new HandlerCollection(this);
			this.Formatter = new DefaultFormatter();
			this.Severity = Severity.Error;
			this.SecondsFormat = "N3";
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the format name.
		/// </summary>
		public BaseFormatter Formatter
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the different handlers the logger uses.
		/// </summary>
		public HandlerCollection Handlers
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the severity.
		/// </summary>
		public Severity Severity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the formatting to use for profiler seconds measurement, defaults to '0.000000'.
		/// </summary>
		public string SecondsFormat
		{
			get;
			set;
		}

		#endregion
	}
}