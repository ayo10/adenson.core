using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Adenson.Configuration.Internal
{
	/// <summary>
	/// Config values for use within the assembly
	/// </summary>
	internal static class Config
	{
		private static LoggerSettings _logSettings;
		
		public static LoggerSettings LogSettings
		{
			get
			{
				if (_logSettings == null)
				{
					var section = (XmlDocument)ConfigurationManager.GetSection("adenson/loggerSettings");
					_logSettings = new LoggerSettings((section != null) ? XDocument.Parse(section.InnerXml).Root : null);
				}
				return _logSettings;
			}
		}
	}
}