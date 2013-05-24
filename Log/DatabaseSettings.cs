using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Adenson.Configuration;
using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Represents database log settings.
	/// </summary>
	public sealed class DatabaseSettings : XElementSettingBase
	{
		#region Variables
		private const string InsertStatementText = "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES ({5})";
		private const string InsertValuesText = "'{0}', '{1}', '{2}', '{3}'";
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseSettings"/> class.
		/// </summary>
		internal DatabaseSettings() : this(null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseSettings"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		internal DatabaseSettings(XElement element) : base(element)
		{
			this.TableName = this.GetValue("TableName", "EventLog");
			this.SeverityColumn = this.GetValue("SeverityColumn", "Severity");
			this.DateColumn = this.GetValue("DateColumn", "Date");
			this.TypeColumn = this.GetValue("TypeColumn", "Type");
			this.MessageColumn = this.GetValue("MessageColumn", "Message");
			this.InsertStatement = StringUtil.Format(DatabaseSettings.InsertStatementText, this.TableName, this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.DateColumn, DatabaseSettings.InsertValuesText);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the table name.
		/// </summary>
		public string TableName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the severity column name.
		/// </summary>
		public string SeverityColumn
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date column name.
		/// </summary>
		public string DateColumn
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the message column name.
		/// </summary>
		public string MessageColumn
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type column name.
		/// </summary>
		public string TypeColumn
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the insert statement.
		/// </summary>
		public string InsertStatement
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the addendum column name.
		/// </summary>
		public string AddendumColumn
		{
			get;
			set;
		}

		#endregion
		#region Methods

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not fail regardless of any exception.")]
		internal bool Save(LogEntry entry)
		{
			var sqlHelper = SqlHelperProvider.Create(ConnectionStrings.Get("Logger", true));
			if (sqlHelper == null)
			{
				return false;
			}

			IDbCommand command = sqlHelper.CreateCommand();
			command.CommandText = this.InsertStatement;
			command.Parameters.Add(sqlHelper.CreateParameter(this.SeverityColumn, entry.Severity.ToString()));
			command.Parameters.Add(sqlHelper.CreateParameter(this.TypeColumn, entry.TypeName));
			command.Parameters.Add(sqlHelper.CreateParameter(this.MessageColumn, entry.Message));
			command.Parameters.Add(sqlHelper.CreateParameter(this.DateColumn, entry.Date));
			command.Parameters.Add(sqlHelper.CreateParameter(this.AddendumColumn, entry.Addendum));

			try
			{
				sqlHelper.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Unable to log to DB");
				System.Diagnostics.Debug.WriteLine(StringUtil.ToString(ex, false));
				return false;
			}

			return true;
		}

		#endregion
	}
}