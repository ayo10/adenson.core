using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;

namespace Adenson.Data
{
	/// <summary>
	/// The Sql Helper class for Oledb connections
	/// </summary>
	public sealed class OleDbSqlHelper : SqlHelperBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlHelper"/> class using <see cref="Configuration.ConnectionStrings.Default"/>.
		/// </summary>
		public OleDbSqlHelper() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlHelper"/> class class.
		/// </summary>
		/// <param name="connectionString">The connection string object to use to initialize the helper.</param>
		/// <exception cref="ArgumentNullException">If specified connection string null</exception>
		/// <exception cref="ArgumentException">If specified connection string object has an invalid connection string</exception>
		public OleDbSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlHelper"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		public OleDbSqlHelper(string keyOrConnectionString) : base(keyOrConnectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlHelper"/> class using specified connection object (which will never be closed or disposed of in this class).
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="close">If to close the connection when this object is disposed.</param>
		/// <exception cref="ArgumentException">If specified connection is null.</exception>
		public OleDbSqlHelper(OleDbConnection connection, bool close) : base(connection, close)
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
		/// Runs a check for the existence of database specified in the connectionstring (Not supported).
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

		/// <summary>
		/// Creates a new DbDataAdapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter.</param>
		/// <returns>New <see cref="OleDbDataAdapter"/> object</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OleDbDataAdapter((OleDbCommand)command);
		}

		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OleDbCommand"/> object</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public override IDbCommand CreateCommand()
		{
			return new OleDbCommand();
		}

		/// <summary>
		/// Creates a new database connection for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="OleDbConnection"/> object</returns>
		public override IDbConnection CreateConnection()
		{
			return new OleDbConnection(this.ConnectionString);
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands.
		/// </summary>
		/// <returns>New <see cref="OleDbParameter"/> object</returns>
		public override IDbDataParameter CreateParameter()
		{
			return new OleDbParameter();
		}

		#endregion
	}
}