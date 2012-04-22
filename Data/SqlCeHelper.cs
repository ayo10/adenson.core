using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using Adenson.Log;

namespace Adenson.Data
{
	/// <summary>
	/// The SqlHelper class for SQL Server CE connections
	/// </summary>
	public sealed class SqlCeHelper : SqlHelperBase
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(SqlCeHelper));
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeHelper"/> class using <see cref="Configuration.ConnectionStrings.Default"/>
		/// </summary>
		public SqlCeHelper() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeHelper"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string settings object to use</param>
		/// <exception cref="ArgumentNullException">if specified connection string null</exception>
		/// <exception cref="ArgumentException">if specified connection string object has an invalid connection string</exception>
		public SqlCeHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeHelper"/> class using specified connection string setting object
		/// </summary>
		/// <param name="connectionString">The connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		public SqlCeHelper(string connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Reclaims wasted space in the SQL Server Compact database by creating a new database file from the existing file. This method is also used to change the collating order, encryption, or password settings of the database.
		/// </summary>
		public void Compact()
		{
			try
			{
				SqlCeEngine engine = new SqlCeEngine(this.ConnectionString); // Yes, u need this here connection string
				engine.Compact(this.ConnectionString);
			}
			catch (SqlCeException ex)
			{
				logger.Error(ex);
			}
		}

		/// <summary>
		/// Creates a new database using information from the connection string.
		/// </summary>
		public override void CreateDatabase()
		{
			new SqlCeEngine(this.ConnectionString).CreateDatabase();
		}

		/// <summary>
		/// Upgrades a SQL Server Compact database from version 3.1 to 3.5. After the upgrade, the database will be encrypted if the source database was encrypted. If it was not, the upgraded database will be unencrypted.
		/// </summary>
		public void Upgrade()
		{
			new SqlCeEngine(this.ConnectionString).Upgrade();
		}

		/// <summary>
		/// Runs a check for the existence of database specified in the connectionstring (Not Supported)
		/// </summary>
		/// <returns>True if the database exists, false otherwise</returns>
		public override bool DatabaseExists()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates a new data adapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="SqlCeDataAdapter"/> object</returns>
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new SqlCeDataAdapter((SqlCeCommand)command);
		}

		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="SqlCeCommand"/> object</returns>
		public override IDbCommand CreateCommand()
		{
			return new SqlCeCommand();
		}

		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="SqlCeConnection"/> object</returns>
		public override IDbConnection CreateConnection()
		{
			return new SqlCeConnection(this.ConnectionString);
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <returns>New <see cref="SqlCeParameter"/> object</returns>
		public override IDbDataParameter CreateParameter()
		{
			return new SqlCeParameter();
		}

		#endregion
	}
}