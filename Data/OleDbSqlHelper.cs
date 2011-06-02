using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace Adenson.Data
{
	/// <summary>
	/// The Sql Helper class for Oledb connections
	/// </summary>
	public sealed class OleDbSqlHelper : SqlHelperBase
	{
		#region Constructor

		/// <summary>
		/// Instantiates a new <see cref="OleDbSqlHelper"/> using <see cref="Configuration.ConnectionStrings.Default"/>
		/// </summary>
		public OleDbSqlHelper() : base()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlHelper"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string object to use to initialize the helper</param>
		/// <exception cref="ArgumentNullException">if specified connection string null</exception>
		/// <exception cref="ArgumentException">if specified connection string object has an invalid connection string</exception>
		public OleDbSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}
		/// <summary>
		/// Instantiates a new instance of <see cref="OleDbSqlHelper"/> using specified connection string setting object
		/// </summary>
		/// <param name="connectionString">The connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		public OleDbSqlHelper(string connectionString) : base(connectionString)
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
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OleDbDataAdapter((OleDbCommand)command);
		}
		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OleDbCommand"/> object</returns>
		public override IDbCommand CreateCommand()
		{
			return new OleDbCommand();
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="OleDbConnection"/> object</returns>
		public override IDbConnection CreateConnection()
		{
			return new OleDbConnection(this.ConnectionString);
		}
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <returns>New <see cref="OleDbParameter"/> object</returns>
		public override IDbDataParameter CreateParameter()
		{
			return new OleDbParameter();
		}

		#endregion
	}
}