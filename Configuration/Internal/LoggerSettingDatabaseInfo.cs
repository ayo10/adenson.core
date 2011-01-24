using System;
using System.Xml.Linq;

namespace Adenson.Configuration.Internal
{
	internal sealed class LoggerSettingDatabaseInfo : XmlSettingsBase
	{
		#region Constructor

		public LoggerSettingDatabaseInfo() : this(null)
		{
		}
		public LoggerSettingDatabaseInfo(XElement element) : base(element)
		{
			this.TableName = this.GetValue("TableName", "EventLog");
			this.SeverityColumn = this.GetValue("SeverityColumn", "Severity");
			this.DateColumn = this.GetValue("DateColumn", "Date");
			this.TypeColumn = this.GetValue("TypeColumn", "Type");
			this.MessageColumn = this.GetValue("MessageColumn", "Message");
		}

		#endregion
		#region Properties

		public string TableName
		{
			get;
			set;
		}
		public string SeverityColumn
		{
			get;
			set;
		}
		public string DateColumn
		{
			get;
			set;
		}
		public string MessageColumn
		{
			get;
			set;
		}
		public string TypeColumn
		{
			get;
			set;
		}

		#endregion
		#region Methods

		public string CreateInsertStatement()
		{
			return String.Concat(StringUtil.Format("INSERT INTO {4} ({0}, {1}, {2}, {3}) ", this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.DateColumn, this.TableName), "VALUES ('{0}', '{1}', '{2}', '{3}')");
		}

		#endregion
	}
}
