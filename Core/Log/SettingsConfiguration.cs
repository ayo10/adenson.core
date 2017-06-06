using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	public sealed class SettingsConfiguration : ConfigurationSection
	{
		#region Properties

		/// <summary>
		/// Gets the format name.
		/// </summary>
		[ConfigurationProperty("formatter")]
		public string Formatter
		{
			get { return (string)this["formatter"]; }
		}

		/// <summary>
		/// Gets the different handlers the logger uses.
		/// </summary>
		[ConfigurationProperty("handlers", IsDefaultCollection = false)]
		public HandlerElementCollection Handlers
		{
			get { return (HandlerElementCollection)this["handlers"]; }
		}

		/// <summary>
		/// Gets the format name.
		/// </summary>
		[ConfigurationProperty("secondsFormat", DefaultValue = "0.00")]
		public string SecondsFormat
		{
			get { return (string)this["secondsFormat"]; }
		}

		/// <summary>
		/// Gets the severity.
		/// </summary>
		[ConfigurationProperty("severity", DefaultValue = Severity.Error)]
		public Severity Severity
		{
			get { return (Severity)this["severity"]; }
		}

		#endregion
		#region Inner Classes

		/// <summary>
		/// Represents the api config element.
		/// </summary>
		public class HandlerElement : ConfigurationElement
		{
			#region Properties

			/// <summary>
			/// Gets the handler.
			/// </summary>
			[ConfigurationProperty("handler", IsRequired = true, IsKey = true)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public HandlerType Handler
			{
				get { return (HandlerType)this["handler"]; }
				internal set { this["handler"] = value; }
			}

			/// <summary>
			/// Gets the formatter.
			/// </summary>
			[ConfigurationProperty("formatter", IsRequired = false, IsKey = false)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public string Formatter
			{
				get { return (string)this["formatter"]; }
				internal set { this["formatter"] = value; }
			}

			/// <summary>
			/// Gets the severity.
			/// </summary>
			[ConfigurationProperty("severity", IsRequired = false, IsKey = false)]
			[SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Called by the HandlerElementCollection constructor.")]
			public Severity Severity
			{
				get { return (Severity)this["severity"]; }
				internal set { this["severity"] = value; }
			}

			/// <summary>
			/// Gets the custom type.
			/// </summary>
			[ConfigurationProperty("customType", IsRequired = false, IsKey = true)]
			public string CustomType
			{
				get { return (string)this["customType"]; }
				internal set { this["customType"] = value; }
			}

			#endregion
			#region Methods

			internal string GetValue(string key, string defaultValue)
			{
				ConfigurationProperty prop = this.Properties.Cast<ConfigurationProperty>().FirstOrDefault(p => String.Equals(p.Name, key, StringComparison.CurrentCultureIgnoreCase));
				return prop == null ? defaultValue : (string)prop.DefaultValue;
			}

			/// <inheritdoc />
			protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
			{
				ConfigurationProperty p = new ConfigurationProperty(name, typeof(string), value);
				this.Properties.Add(p);
				return true;
			}

			#endregion
		}

		/// <summary>
		/// Represents a configuration element for apis.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Yes it was, dummas.")]
		public sealed class HandlerElementCollection : ConfigurationElementCollection
		{
			#region Constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="HandlerElementCollection"/> class.
			/// </summary>
			public HandlerElementCollection()
			{
				this.BaseAdd(new HandlerElement { Severity = Severity.Error, Handler = HandlerType.Trace, Formatter = "Adenson.Log.DefaultFormatter, Adenson.Core" });
			}

			#endregion
			#region Methods

			/// <summary>
			/// Creates a new element
			/// </summary>
			/// <returns>The created element</returns>
			protected override ConfigurationElement CreateNewElement()
			{
				return new HandlerElement();
			}

			/// <summary>
			/// Gets the element key for the <see cref="ConfigurationElement"/> element.
			/// </summary>
			/// <param name="element">The <see cref="ConfigurationElement"/> to return the key for.</param>
			/// <returns>An System.Object that acts as the key for the specified <see cref="ConfigurationElement"/>.</returns>
			protected override object GetElementKey(ConfigurationElement element)
			{
				HandlerElement e = (HandlerElement)element;
				return e.Handler == HandlerType.Custom ? (object)e.CustomType : (object)e.Handler;
			}

			#endregion
		}

		#endregion
	}
}