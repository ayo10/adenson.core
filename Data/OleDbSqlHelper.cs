using System;
using System.Configuration;
using System.Data.Common;
using System.Data.OleDb;

namespace Adenson.Data
{
	/// <summary>
	/// The SqlHelper class for Oledb connections
	/// </summary>
	public sealed class OleDbSqlHelper : SqlHelperBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlHelper"/> class.
		/// </summary>
		/// <param name="connectionString"></param>
		public OleDbSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Runs a query to see if the specified column in the specified table (Not supported)
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <param name="columnName">the column name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Runs a query to see if the specified table exists (Not supported)
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool CheckTableExists(string tableName)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Creates a new DbDataAdapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="OleDbDataAdapter"/> object</returns>
		public override DbDataAdapter CreateAdapter(DbCommand command)
		{
			return new OleDbDataAdapter((OleDbCommand)command);
		}
		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OdbcCommand"/> object</returns>
		public override DbCommand CreateCommand()
		{
			return new OleDbCommand();
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="OleDbConnection"/> object</returns>
		public override DbConnection CreateConnection()
		{
			return new OleDbConnection(this.ConnectionString);
		}
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		/// <returns>New <see cref="OleDbParameter"/> object</returns>
		public override DbParameter CreateParameter()
		{
			return new OleDbParameter();
		}

		#endregion
	}
}