using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Adenson.Configuration
{
	/// <summary>
	/// Config values for use within the assembly
	/// 
	/// </summary>
	internal static class Config
	{
		private static readonly object dummy = new object();
		private static LoggerSettings _logSettings;
		
		public static LoggerSettings LogSettings
		{
			get
			{
				if (_logSettings == null)
				{
					lock (dummy)
					{
						var section = (XmlDocument)ConfigurationManager.GetSection("adenson/loggerSettings");
						var serializer = new XmlSerializer(typeof(LoggerSettings));
						try
						{
							_logSettings = (LoggerSettings)serializer.Deserialize(new XmlNodeReader(section));
						}
						catch
						{
						}
						if (_logSettings == null) _logSettings = new LoggerSettings();
					}
				}
				return _logSettings;
			}
		}
	}
}