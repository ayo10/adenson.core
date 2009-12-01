using System;
using System.Xml;

namespace Adenson.Xml
{
	/// <summary>
	/// Collection of commonly needed Xml utilities
	/// </summary>
	public static class XmlUtils
	{
		/// <summary>
		/// Creates a new XmlElement, sets an element using key and value, appends it to the document and returns it
		/// </summary>
		/// <param name="document">The document</param>
		/// <param name="key">The new element key</param>
		/// <param name="value">The element InnerText</param>
		/// <returns>Newly created and appended element</returns>
		public static XmlElement AppendElement(XmlDocument document, string key, string value)
		{
			XmlElement element = document.CreateElement(key);
			element.InnerText = value;
			document.AppendChild(element);
			return element;
		}
		/// <summary>
		/// Creates a new XmlElement, sets an element (from element.OwnerDocument) using key and value, appends it to the element and returns it
		/// </summary>
		/// <param name="element">The element</param>
		/// <param name="key">The new element key</param>
		/// <param name="value">The element InnerText</param>
		/// <returns>Newly created and appended element</returns>
		public static XmlElement AppendElement(XmlElement element, string key, string value)
		{
			XmlElement childElement = element.OwnerDocument.CreateElement(key);
			childElement.InnerText = value;
			element.AppendChild(childElement);
			return childElement;
		}
	}
}