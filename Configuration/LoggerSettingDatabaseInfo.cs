using System;
using System.Xml.Serialization;

namespace Adenson.Configuration
{
	public sealed class LoggerSettingDatabaseInfo
	{
		#region Constructor

		public LoggerSettingDatabaseInfo()
		{
			this.TableName = "EventLog";
			this.SeverityColumn = "Severity";
			this.DateColumn = "Date";
			this.TypeColumn = "Type";
			this.MessageColumn = "Message";
			this.PathColumn = "Path";
		}

		#endregion
		#region Properties

		[XmlAttribute(AttributeName = "tableName")]
		public string TableName
		{
			get;
			set;
		}
		[XmlAttribute(AttributeName = "severityColumn")]
		public string SeverityColumn
		{
			get;
			set;
		}
		[XmlAttribute(AttributeName = "dateColumn")]
		public string DateColumn
		{
			get;
			set;
		}
		[XmlAttribute(AttributeName = "messageColumn")]
		public string MessageColumn
		{
			get;
			set;
		}
		[XmlAttribute(AttributeName = "typeColumn")]
		public string TypeColumn
		{
			get;
			set;
		}
		[XmlAttribute(AttributeName = "pathColumn")]
		public string PathColumn
		{
			get;
			set;
		}

		#endregion
		#region Methods

		public string CreateInsertStatement()
		{
			return String.Concat(StringUtil.Format("INSERT INTO [dbo].[EVENT_LOG] ({0}, {1}, {2}, {3}, {4}) ", this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.PathColumn, this.DateColumn), "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')");
		}

		#endregion
	}
}
