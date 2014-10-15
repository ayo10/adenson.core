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
		/// Initializes a new instance of the <see cref="OdbcSqlHelper"/> class using <see cref="Configuration.ConnectionStrings.Default"/>.
		/// </summary>
		public OdbcSqlHelper() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OdbcSqlHelper"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string object to use to initialize the helper.</param>
		/// <exception cref="ArgumentNullException">If specified connection string null</exception>
		/// <exception cref="ArgumentException">If specified connection string object has an invalid connection string</exception>
		public OdbcSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OdbcSqlHelper"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		public OdbcSqlHelper(string keyOrConnectionString) : base(keyOrConnectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OdbcSqlHelper"/> class using specified connection object (which will never be closed or disposed of in this class).
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="close">If to close the connection when this object is disposed.</param>
		/// <exception cref="ArgumentException">If specified connection is null.</exception>
		public OdbcSqlHelper(OdbcConnection connection, bool close) : base(connection, close)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Runs a query to see if the specified column in the specified table (Not supported).
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <param name="columnName">The column name.</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool ColumnExists(string tableName, string columnName)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates a new <see cref="OdbcDataAdapter"/> object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter.</param>
		/// <returns>New <see cref="OdbcDataAdapter"/> object</returns>
		/// <exception cref="InvalidCastException">If <paramref name="command"/> is not an instance of <see cref="OdbcCommand"/>.</exception>
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OdbcDataAdapter((OdbcCommand)command);
		}

		/// <summary>
		/// Creates a new <see cref="OdbcCommand"/> object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OdbcCommand"/> object</returns>
		public override IDbCommand CreateCommand()
		{
			return new OdbcCommand();
		}

		/// <summary>
		/// Creates a new <see cref="OdbcConnection"/> object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OdbcConnection"/> object</returns>
		public override IDbConnection CreateConnection()
		{
			return new OdbcConnection(this.ConnectionString);
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands.
		/// </summary>
		/// <returns>New <see cref="OdbcParameter"/> object</returns>
		public override IDbDataParameter CreateParameter()
		{
			return new OdbcParameter();
		}

		/// <summary>
		/// Runs a check for the existence of database specified in the connectionstring.
		/// </summary>
		/// <returns>True if the database exists, false otherwise</returns>
		public override bool DatabaseExists()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Runs a query to see if the specified table exists (Not supported).
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool TableExists(string tableName)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}