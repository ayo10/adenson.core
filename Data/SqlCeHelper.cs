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
		/// Instantiates a new <see cref="SqlCeHelper"/> using <see cref="Configuration.ConnectionStrings.Default"/>
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
		/// Instantiates a new instance of <see cref="SqlCeHelper"/> using specified connection string setting object
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
				SqlCeEngine engine = new SqlCeEngine(this.ConnectionString);//yes, u need this here connection string
				engine.Compact(this.ConnectionString);
			}
			catch (SqlCeException ex)
			{
				logger.Error(ex);
			}
		}
		/// <summary>
		/// Creates a new database.
		/// </summary>
		public void CreateDatabase()
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
		/// Runs a query to determine if the specified column exists in the specified table
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <param name="columnName">the column name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			if (StringUtil.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException("tableName");
			if (StringUtil.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException("columnName");
			bool result = false;
			using (IDataReader r = this.ExecuteReader(CommandType.Text, "SELECT * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' AND COLUMN_NAME = '" + columnName + "'"))
			{
				result = r.Read();
			}
			return result;
		}
		/// <summary>
		/// Runs a query to determine if the specified table exists
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool CheckTableExists(string tableName)
		{
			if (StringUtil.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException("tableName");
			bool result = false;
			using (IDataReader r = this.ExecuteReader(CommandType.Text, "SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName + "'"))
			{
				result = r.Read();
			}
			return result;
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