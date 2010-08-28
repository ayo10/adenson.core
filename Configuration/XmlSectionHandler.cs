using System;
using System.Configuration;
using System.Xml;

namespace Adenson.Configuration
{
	/// <summary>
	/// Provides full xml configuration information from a configuration section.
	/// </summary>
	public class XmlSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates a new configuration handler and adds it to the section-handler collection based on the specified parameters.
		/// </summary>
		/// <param name="parent">The parent object.</param>
		/// <param name="configContext">The configuration context object.</param>
		/// <param name="section">The section XML node.</param>
		/// <returns>An <see cref="XmlDocument"/> object loaded with section.InnerXml</returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			var xml = new XmlDocument();
			xml.LoadXml(section.OuterXml);
			return xml;
		}
	}
}