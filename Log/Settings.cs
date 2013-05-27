using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Adenson.Configuration;

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
		/// Gets or sets the severity.
		/// </summary>
		[ConfigurationProperty("severity", DefaultValue = Severity.Error)]
		public Severity Severity
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

		#endregion
		#region Methods

		/// <summary>
		/// Instantiates a new instance of the <see cref="Settings"/> class from the config object.
		/// </summary>
		/// <param name="section">The configuration object.</param>
		/// <returns>Created settings object.</returns>
		public static Settings FromConfig(ConfigurationSection section)
		{
			Configuration config = (Configuration)section;
			Settings settings = new Settings();
			settings.Severity = config.Severity;
			settings.Formatter = String.IsNullOrWhiteSpace(config.Formatter) ? new DefaultFormatter() : TypeUtil.CreateInstance<BaseFormatter>(config.Formatter);
			settings.Handlers = HandlerCollection.FromConfig(settings, config.Handlers);
			return settings;
		}

		#endregion
	}
}