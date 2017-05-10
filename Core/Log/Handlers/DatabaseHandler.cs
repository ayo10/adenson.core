using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Represents sql server log settings.
	/// </summary>
	public class DatabaseHandler : BaseHandler
	{
		#region Variables
		private const string InsertStatementText = "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@{1}, @{2}, @{3}, @{4})";
		private string _insertStatement;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
		/// </summary>
		public DatabaseHandler() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		public DatabaseHandler(string connectionStringOrKey, string tableName = "EventLog", string severityColumn = "Severity", string dateColumn = "Date", string typeColumn = "Type", string messageColumn = "Message") : base()
		{
			this.Connection = Arg.IsNotNull(connectionStringOrKey);
			this.TableName = Arg.IsNotNull(tableName);
			this.SeverityColumn = Arg.IsNotNull(severityColumn);
			this.DateColumn = Arg.IsNotNull(dateColumn);
			this.TypeColumn = Arg.IsNotNull(typeColumn);
			this.MessageColumn = Arg.IsNotNull(messageColumn);
			_insertStatement = StringUtil.Format(DatabaseHandler.InsertStatementText, this.TableName, this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.DateColumn);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the database connection (string or configuration name).
		/// </summary>
		public string Connection
		{
			get;
			set;
		}

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
		[SuppressMessage("Microsoft.Security", "CA2100", Justification = "Class is a sql execution helper, executes whatever is passed to it without any validation.")]
		public override bool Write(LogEntry entry)
		{
			Arg.IsNotNull(entry);
			using (SqlHelperBase sqlHelper = SqlHelperProvider.Create(this.Connection))
			{
				try
				{
					using (IDbCommand command = sqlHelper.CreateCommand())
					{
						command.CommandText = _insertStatement;
						command.Parameters.Add(sqlHelper.CreateParameter(this.SeverityColumn, entry.Severity.ToString()));
						command.Parameters.Add(sqlHelper.CreateParameter(this.TypeColumn, entry.TypeName));
						command.Parameters.Add(sqlHelper.CreateParameter(this.MessageColumn, entry.Message));
						command.Parameters.Add(sqlHelper.CreateParameter(this.DateColumn, entry.Date));
						sqlHelper.ExecuteNonQuery(command);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Unable to log to DB");
					Debug.WriteLine(StringUtil.ToString(ex));
					return false;
				}

				return true;
			}
		}

		#endregion
	}
}