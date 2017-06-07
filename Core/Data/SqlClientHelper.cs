using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Adenson.Data
{
	/// <summary>
	/// The SqlHelper class for SQL Server Client connections
	/// </summary>
	[Obsolete("SqlClientHelper got renamed to SqlServerHelper. Use that instead.", false)]
	public class SqlClientHelper : SqlServerHelper
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class using <see cref="Configuration.ConnectionStrings.Default"/>.
		/// </summary>
		public SqlClientHelper() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string settings object to use.</param>
		/// <exception cref="ArgumentNullException">If specified connection string null</exception>
		/// <exception cref="ArgumentException">If specified connection string object has an invalid connection string</exception>
		public SqlClientHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid.</exception>
		public SqlClientHelper(string keyOrConnectionString) : base(keyOrConnectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class using specified connection object (which will never be closed or disposed of in this class).
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="close">If to close the connection when this object is disposed.</param>
		/// <exception cref="ArgumentException">If specified connection is null.</exception>
		public SqlClientHelper(SqlConnection connection, bool close) : base(connection, close)
		{
		}

		#endregion
	}
}