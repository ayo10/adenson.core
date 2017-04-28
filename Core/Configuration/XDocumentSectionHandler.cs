using System;
using System.Configuration;
using System.Xml.Linq;

namespace Adenson.Configuration
{
	/// <summary>
	/// Provides full xml configuration information from a configuration section.
	/// </summary>
	public sealed class XDocumentSectionHandler : IConfigurationSectionHandler
	{
		#region Methods

		object IConfigurationSectionHandler.Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			if (section == null)
			{
				throw new ArgumentNullException("section");
			}

			return XDocument.Parse(section.OuterXml);
		}

		#endregion
	}
}