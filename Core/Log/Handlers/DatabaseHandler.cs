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
		private const string InsertStatementFullText = "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@{1}, @{2}, @{3}, @{4})";
		private const string InsertStatementFlatText = "INSERT INTO {0} ({1}) VALUES (@{1})";
		private bool simple;
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
		/// <param name="connectionStringOrKey">The connection string (or the connection configuration key).</param>
		/// <param name="tableName">The database table name.</param>
		/// <param name="messageColumn">The message column.</param>
		public DatabaseHandler(string connectionStringOrKey, string tableName, string messageColumn) : base()
		{
			this.Connection = Arg.IsNotNull(connectionStringOrKey);
			this.TableName = Arg.IsNotNull(tableName);
			this.MessageColumn = Arg.IsNotNull(messageColumn);
			simple = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
		/// </summary>
		/// <param name="connectionStringOrKey">The connection string (or the connection configuration key).</param>
		/// <param name="tableName">The database table name.</param>
		/// <param name="severityColumn">The severity column name.</param>
		/// <param name="dateColumn">The date column.</param>
		/// <param name="typeColumn">The type column.</param>
		/// <param name="messageColumn">The message column.</param>
		public DatabaseHandler(string connectionStringOrKey, string tableName, string severityColumn, string dateColumn, string typeColumn, string messageColumn) : base()
		{
			this.Connection = Arg.IsNotNull(connectionStringOrKey);
			this.TableName = Arg.IsNotNull(tableName);
			this.SeverityColumn = Arg.IsNotNull(severityColumn);
			this.DateColumn = Arg.IsNotNull(dateColumn);
			this.TypeColumn = Arg.IsNotNull(typeColumn);
			this.MessageColumn = Arg.IsNotNull(messageColumn);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the database connection (string or configuration name).
		/// </summary>
		public string Connection
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the table name.
		/// </summary>
		public string TableName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the severity column name.
		/// </summary>
		public string SeverityColumn
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the date column name.
		/// </summary>
		public string DateColumn
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the message column name.
		/// </summary>
		public string MessageColumn
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the type column name.
		/// </summary>
		public string TypeColumn
		{
			get;
			private set;
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
						if (simple)
						{
							command.CommandText = StringUtil.Format(DatabaseHandler.InsertStatementFlatText, this.TableName, this.MessageColumn);
							command.Parameters.Add(sqlHelper.CreateParameter(this.MessageColumn, entry.ToString()));
						}
						else
						{
							command.CommandText = StringUtil.Format(DatabaseHandler.InsertStatementFullText, this.TableName, this.SeverityColumn, this.TypeColumn, this.MessageColumn, this.DateColumn);
							command.Parameters.Add(sqlHelper.CreateParameter(this.SeverityColumn, entry.Severity.ToString()));
							command.Parameters.Add(sqlHelper.CreateParameter(this.TypeColumn, entry.TypeName));
							command.Parameters.Add(sqlHelper.CreateParameter(this.MessageColumn, entry.Message));
							command.Parameters.Add(sqlHelper.CreateParameter(this.DateColumn, entry.Date));
						}
						
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