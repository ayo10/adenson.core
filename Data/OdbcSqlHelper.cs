using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace Adenson.Data
{
	/// <summary>
	/// The OdbcHelper class is intended to encapsulate high performance, scalable best practices for
	/// common uses of OdbcClient
	/// </summary>
	public sealed class OdbcSqlHelper : SqlHelperBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="OdbcSqlHelper"/> class.
		/// </summary>
		/// <param name="connectionString"></param>
		public OdbcSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Creates a new DbDataAdapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="OdbcDataAdapter"/> object</returns>
		public override DbDataAdapter CreateAdapter(DbCommand command)
		{
			return new OdbcDataAdapter((OdbcCommand)command);
		}
		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OdbcCommand"/> object</returns>
		public override DbCommand CreateCommand()
		{
			return new OdbcCommand();
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="OdbcConnection"/> object</returns>
		public override DbConnection CreateConnection()
		{
			return new OdbcConnection(this.ConnectionString);
		}
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		/// <returns>New <see cref="OdbcParameter"/> object</returns>
		public override DbParameter CreateParameter()
		{
			return new OdbcParameter();
		}
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

		#endregion
	}
}