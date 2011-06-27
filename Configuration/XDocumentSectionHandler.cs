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
			return XDocument.Parse(section.OuterXml);
		}

		#endregion
	}

	[Obsolete("Use XDocumentSectionHandler")]
	public class XmlSectionHandler : IConfigurationSectionHandler
	{
		#region Methods

		object IConfigurationSectionHandler.Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			return XDocument.Parse(section.OuterXml);
		}

		#endregion
	}
}