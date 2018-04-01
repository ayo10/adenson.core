#if !NETSTANDARD1_0
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
		#region Fields
		private SqlHelperBase sql; 
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseHandler"/> class.
		/// </summary>
		/// <param name="sql">The <see cref="SqlHelperBase"/> to use.</param>
		/// <param name="tableName">The database table name.</param>
		/// <param name="dateColumn">The date column.</param>
		/// <param name="typeColumn">The type column.</param>
		/// <param name="severityColumn">The severity column name.</param>
		/// <param name="messageColumn">The message column.</param>
		public DatabaseHandler(SqlHelperBase sql, string tableName, string dateColumn, string typeColumn, string severityColumn, string messageColumn)
		{
			this.sql = Arg.IsNotNull(sql, nameof(sql));
			this.TableName = Arg.IsNotNull(tableName, nameof(tableName));
			this.SeverityColumn = Arg.IsNotNull(severityColumn, nameof(tableName));
			this.DateColumn = Arg.IsNotNull(dateColumn, nameof(tableName));
			this.TypeColumn = Arg.IsNotNull(typeColumn, nameof(tableName));
			this.MessageColumn = Arg.IsNotNull(messageColumn, nameof(tableName));
		}

		#endregion
		#region Properties

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
			try
			{
				using (IDbCommand command = sql.CreateCommand())
				{
					command.CommandText = $@"INSERT INTO {this.TableName} ({this.DateColumn}, {this.SeverityColumn}, {this.TypeColumn}, {this.MessageColumn}) VALUES (@p0, @p1, @p2, @p3)";
					command.Parameters.Add(sql.CreateParameter("p0", entry.Date));
					command.Parameters.Add(sql.CreateParameter("p1", entry.Severity.ToString()));
					command.Parameters.Add(sql.CreateParameter("p2", entry.TypeName));
					command.Parameters.Add(sql.CreateParameter("p3", entry.Message));
					sql.ExecuteNonQuery(command);
				}

				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to log to DB");
				Debug.WriteLine(StringUtil.ToString(ex));
				return false;
			}
		}

		#endregion
	}
}
#endif