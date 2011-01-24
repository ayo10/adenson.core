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
		private static Logger logger = new Logger(typeof(SqlCeHelper));
		#endregion
		#region Constructor

		public SqlCeHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

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
		public void CreateDatabase()
		{
			new SqlCeEngine(this.ConnectionString).CreateDatabase();
		}
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
		/// <returns>New <see cref="SqlDataAdapter"/> object</returns>
		public override DbDataAdapter CreateAdapter(DbCommand command)
		{
			return new SqlCeDataAdapter((SqlCeCommand)command);
		}
		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="SqlCeCommand"/> object</returns>
		public override DbCommand CreateCommand()
		{
			return new SqlCeCommand();
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="SqlCeConnection"/> object</returns>
		public override DbConnection CreateConnection()
		{
			return new SqlCeConnection(this.ConnectionString);
		}
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		/// <returns>New <see cref="SqlCeParameter"/> object</returns>
		public override DbParameter CreateParameter()
		{
			return new SqlCeParameter();
		}

		#endregion
	}
}