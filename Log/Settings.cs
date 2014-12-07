using System;
using System.Collections.Generic;
using System.Configuration;

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	public sealed class Settings
	{
		#region Variables
		private BaseFormatter _formatter;
		#endregion
		#region Constructor

		private Settings()
		{
			this.Formatter = new DefaultFormatter();
			this.Severity = Severity.Error;
			this.Handlers = new HandlerCollection(this);
			this.Handlers.Add(new TraceHandler());
			this.SecondsFormat = "N3";
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the format name.
		/// </summary>
		public BaseFormatter Formatter
		{
			get
			{
				return _formatter; 
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_formatter = value;
			}
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
		#region Methods

		/// <summary>
		/// Instantiates a new instance of the <see cref="Settings"/> class from the config object. If config is null, creates a default settings object.
		/// </summary>
		/// <param name="section">The configuration object.</param>
		/// <returns>Created settings object.</returns>
		public static Settings FromConfig(ConfigurationSection section)
		{
			SettingsConfiguration config = section as SettingsConfiguration;
			Settings settings = new Settings();
			if (config != null)
			{
				settings.Severity = config.Severity;
				settings.SecondsFormat = config.SecondsFormat;
				settings.Formatter = String.IsNullOrWhiteSpace(config.Formatter) ? new DefaultFormatter() : TypeUtil.CreateInstance<BaseFormatter>(config.Formatter);
				settings.Handlers = HandlerCollection.FromConfig(settings, config.Handlers);
			}

			return settings;
		}

		#endregion
	}
}