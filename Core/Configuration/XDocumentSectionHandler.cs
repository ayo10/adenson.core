using System.Configuration;
using System.Diagnostics;
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
			Arg.IsNotNull(section);
			return XDocument.Parse(section.OuterXml);
		}

		#endregion
	}
}