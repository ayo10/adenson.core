using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Adenson.Configuration;
using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Represents database log settings.
	/// </summary>
	public sealed class DatabaseHandler : BaseHandler
	{
		#region Variables
		private const string InsertStatementText = "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@{1}, @{2}, @{3}, @{4})";
		private string insertStatement;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		internal DatabaseHandler(SettingsConfiguration.HandlerElement element) : base()
		{
			this.TableName = element.GetValue("tableName", "EventLog");
			this.SeverityColumn = element.GetValue("severityColumn", "Severity");
			this.DateColumn = element.GetValue("dateColumn", "Date");
			this.TypeColumn = element.GetValue("typeColumn", "Type");
			this.MessageColumn = element.GetValue("messageColumn", "Message");
			
			insertStatement = StringUtil.Format(DatabaseHandler.InsertStatementText, this.TableName, this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.DateColumn);
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

		#endregion
		#region Methods

		/// <summary>
		/// Writes the log entry into the database as configured.
		/// </summary>
		/// <param name="entry">The entry to log.</param>
		/// <returns>True if record was successfully inserted, false otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not fail regardless of any exception.")]
		public override bool Write(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			SqlHelperBase sqlHelper = SqlHelperProvider.Create(ConnectionStrings.Get("Logger", true));
			if (sqlHelper == null)
			{
				return false;
			}

			try
			{
				IDbCommand command = sqlHelper.CreateCommand();
				command.CommandText = insertStatement;
				command.Parameters.Add(sqlHelper.CreateParameter(this.SeverityColumn, entry.Severity.ToString()));
				command.Parameters.Add(sqlHelper.CreateParameter(this.TypeColumn, entry.TypeName));
				command.Parameters.Add(sqlHelper.CreateParameter(this.MessageColumn, entry.Message));
				command.Parameters.Add(sqlHelper.CreateParameter(this.DateColumn, entry.Date));
				sqlHelper.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to log to DB");
				Debug.WriteLine(StringUtil.ToString(ex, false));
				return false;
			}

			return true;
		}

		#endregion
	}
}