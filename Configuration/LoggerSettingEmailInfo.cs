using System;
using System.Xml.Serialization;

namespace Adenson.Configuration
{
	public sealed class LoggerSettingEmailInfo
	{
		[XmlAttribute(AttributeName = "from")]
		public string From
		{
			get;
			set;
		}
		[XmlAttribute(AttributeName = "to")]
		public string To
		{
			get;
			set;
		}
	}
}
