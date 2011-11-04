using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;

namespace Adenson.Data
{
	/// <summary>
	/// The Sql Helper class for ODBC connections
	/// </summary>
	public sealed class OdbcSqlHelper : SqlHelperBase
	{
		#region Constructor
		
		/// <summary>
		/// Instantiates a new <see cref="OdbcSqlHelper"/> using <see cref="Configuration.ConnectionStrings.Default"/>
		/// </summary>
		public OdbcSqlHelper() : base()
		{
		}
		/// <summary>
		/// Initializes a new instance of <see cref="OdbcSqlHelper"/>.
		/// </summary>
		/// <param name="connectionString">The connection string object to use to initialize the helper</param>
		/// <exception cref="ArgumentNullException">if specified connection string null</exception>
		/// <exception cref="ArgumentException">if specified connection string object has an invalid connection string</exception>
		public OdbcSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}
		/// <summary>
		/// Instantiates a new instance of <see cref="OdbcSqlHelper"/> using specified connection string setting object
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		public OdbcSqlHelper(string keyOrConnectionString) : base(keyOrConnectionString)
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
		public override bool ColumnExists(string tableName, string columnName)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Creates a new DbDataAdapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="OdbcDataAdapter"/> object</returns>
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OdbcDataAdapter((OdbcCommand)command);
		}
		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OdbcCommand"/> object</returns>
		public override IDbCommand CreateCommand()
		{
			return new OdbcCommand();
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="OdbcConnection"/> object</returns>
		public override IDbConnection CreateConnection()
		{
			return new OdbcConnection(this.ConnectionString);
		}
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <returns>New <see cref="OdbcParameter"/> object</returns>
		public override IDbDataParameter CreateParameter()
		{
			return new OdbcParameter();
		}
		/// <summary>
		/// Runs a check for the existence of database specified in the connectionstring
		/// </summary>
		/// <returns>True if the database exists, false otherwise</returns>
		public override bool DatabaseExists()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Runs a query to see if the specified table exists (Not supported)
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool TableExists(string tableName)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}