﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Adenson.Configuration;
using Adenson.Data;

namespace Adenson.Log.Config
{
	internal sealed class LoggerSettingDatabaseInfo : XElementSettingBase
	{
		#region Variables
		private const string InsertStatementText = "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES ({5})";
		private const string InsertValuesText = "'{0}', '{1}', '{2}', '{3}'";
		#endregion
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
			this.InsertStatement = StringUtil.Format(LoggerSettingDatabaseInfo.InsertStatementText, this.TableName, this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.DateColumn, LoggerSettingDatabaseInfo.InsertValuesText);
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

		public string InsertStatement
		{
			get;
			private set;
		}

		#endregion
		#region Methods

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal bool Save(LogEntry entry)
		{
			var sqlHelper = SqlHelperProvider.Create(ConnectionStrings.Get("Logger", true));
			if (sqlHelper == null) return false;

			IDbCommand command = sqlHelper.CreateCommand();
			command.CommandText = this.InsertStatement;
			command.Parameters.Add(sqlHelper.CreateParameter(this.SeverityColumn, entry.Severity.ToString()));
			command.Parameters.Add(sqlHelper.CreateParameter(this.TypeColumn, entry.TypeName));
			command.Parameters.Add(sqlHelper.CreateParameter(this.MessageColumn, entry.Message));
			command.Parameters.Add(sqlHelper.CreateParameter(this.DateColumn, entry.Date));

			try
			{
				sqlHelper.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(Logger.ConvertToString(ex));
				return false;
			}

			return true;
		}

		#endregion
	}
}