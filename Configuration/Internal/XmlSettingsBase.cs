using System;
using System.Linq;
using System.Xml.Linq;

namespace Adenson.Configuration.Internal
{
	internal abstract class XmlSettingsBase
	{
		#region Constructor

		public XmlSettingsBase(XElement element)
		{
			this.BaseElement = element;
		}

		#endregion
		#region Properties

		public XElement BaseElement
		{
			get;
			set;
		}

		#endregion
		#region Methods

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
					if (TypeUtil.TryConvert<T>(value, out output)) result = (T)output;
				}
			}
			return result;

		}

		#endregion
	}
}