using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

namespace Adenson.Configuration
{
	/// <summary>
	/// Represents a xml element setting base.
	/// </summary>
	public abstract class XElementSettingBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="XElementSettingBase"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		protected XElementSettingBase(XElement element)
		{
			this.BaseElement = element;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the base element.
		/// </summary>
		protected XElement BaseElement
		{
			get;
			private set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Gets the value of the specify key, returning <paramref name="defaultValue"/> if it wasn't found.
		/// </summary>
		/// <typeparam name="T">The type to convert the value to .</typeparam>
		/// <param name="key">The key to look for.</param>
		/// <param name="defaultValue">The value to return if no value was found.</param>
		/// <returns>Found value if any, <paramref name="defaultValue"/> otherwise.</returns>
		protected T GetValue<T>(string key, T defaultValue)
		{
			T result = defaultValue;
			if (this.BaseElement != null)
			{
				var child = this.BaseElement.Attributes().FirstOrDefault(e => String.Equals(e.Name.LocalName, key, StringComparison.OrdinalIgnoreCase));
				if (child != null)
				{
					var value = child.Value;
					T output;
					if (TypeUtil.TryConvert<T>(value, out output))
					{
						result = (T)output;
					}
				}
			}

			return result;
		}

		#endregion
	}
}