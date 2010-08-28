using System;

namespace Adenson.Log
{
	internal sealed class Settings
	{
		public string EmailErrorTo;
		public string EmailErrorFrom = "errors@adenson.com";
		public string Source;
		public ushort BatchLogSize = 0;
		public LogSeverity SeverityLevel = LogSeverity.Error;
		public LogType LogType = LogType.None;
		public string DateTimeFormat = "HH:mm:ss:fff";
	}
}