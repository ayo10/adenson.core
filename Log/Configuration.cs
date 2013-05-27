using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log
{
	/// <summary>
	/// Represents the <see cref="Logger"/> default settings.
	/// </summary>
	internal sealed class Configuration : ConfigurationSection
	{
		#region Properties

		/// <summary>
		/// Gets the format name.
		/// </summary>
		[ConfigurationProperty("formatter")]
		public string Formatter
		{
			get { return (string)base["formatter"]; }
		}

		/// <summary>
		/// Gets the severity.
		/// </summary>
		[ConfigurationProperty("severity", DefaultValue = Severity.Error)]
		public Severity Severity
		{
			get { return (Severity)base["severity"]; }
		}

		/// <summary>
		/// Gets the different handlers the logger uses.
		/// </summary>
		[ConfigurationProperty("handlers", IsDefaultCollection = false)]
		public HandlerElementCollection Handlers
		{
			get { return (HandlerElementCollection)base["handlers"]; }
		}

		#endregion
		#region Inner Classes

		/// <summary>
		/// Represents the api config element.
		/// </summary>
		internal class HandlerElement : ConfigurationElement
		{
			#region Properties

			/// <summary>
			/// Gets the url.
			/// </summary>
			[ConfigurationProperty("handler", IsRequired = true, IsKey = true)]
			public HandlerType Handler
			{
				get { return (HandlerType)this["handler"]; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			[ConfigurationProperty("formatter", IsRequired = false, IsKey = false)]
			public string Formatter
			{
				get { return (string)this["formatter"]; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			[ConfigurationProperty("customType", IsRequired = false, IsKey = true)]
			public string CustomType
			{
				get
				{
					string type = (string)this["customType"];
					if (this.Handler == HandlerType.Custom && String.IsNullOrEmpty(type))
					{
						throw new ConfigurationErrorsException(Exceptions.CustomHandlerTypeTypeMustBeSpecified);
					}

					return type;
				}
			}

			#endregion
			#region Methods

			internal string GetValue(string key, string defaultValue)
			{
				ConfigurationProperty prop = this.Properties.Cast<ConfigurationProperty>().FirstOrDefault(p => String.Equals(p.Name, key, StringComparison.CurrentCultureIgnoreCase));
				return prop == null ? defaultValue : (string)prop.DefaultValue;
			}

			protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
			{
				ConfigurationProperty p = new ConfigurationProperty(name, typeof(string), value);
				this.Properties.Add(p);
				return true;
			}

			#endregion
		}

		/// <summary>
		/// Represents a configuration element for apis
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Yes it was, dummas.")]
		internal sealed class HandlerElementCollection : ConfigurationElementCollection
		{
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