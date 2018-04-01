using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Adenson.Log;

namespace Adenson.Data
{
	/// <summary>
	/// The SqlHelper class for SQL Server connections
	/// </summary>
	public class SqlServerHelper : SqlHelperBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerHelper"/> class using <see cref="Configuration.ConnectionStrings.Default"/>.
		/// </summary>
		public SqlServerHelper() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerHelper"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid.</exception>
		public SqlServerHelper(string keyOrConnectionString) : base(keyOrConnectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerHelper"/> class using specified connection object (which will never be closed or disposed of in this class).
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="close">If to close the connection when this object is disposed.</param>
		/// <exception cref="ArgumentException">If specified connection is null.</exception>
		public SqlServerHelper(SqlConnection connection, bool close) : base(connection, close)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="SqlCommand"/> object</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public override IDbCommand CreateCommand()
		{
			return new SqlCommand();
		}

		/// <summary>
		/// Creates a new database connection for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="SqlConnection"/> object</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public override IDbConnection CreateConnection()
		{
			return new SqlConnection(this.ConnectionString);
		}

		/// <summary>
		/// Creates a new database using information from the connection stringg (after switching the database to master).
		/// </summary>
		public override void CreateDatabase()
		{
			var ssb = new SqlConnectionStringBuilder(this.ConnectionString);
			var database = ssb.InitialCatalog;
			ssb.InitialCatalog = "master";
			using (var connection = new SqlConnection(ssb.ToString()))
			{
				connection.Open();
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, $"CREATE DATABASE [{database}]");
				cmd.Connection = connection;
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands.
		/// </summary>
		/// <returns>New <see cref="SqlParameter"/> object</returns>
		public override IDbDataParameter CreateParameter()
		{
			return new SqlParameter();
		}

		/// <summary>
		/// Runs a check for the existence of database specified in the connection string (after switching the database to master).
		/// </summary>
		/// <returns>True if the database exists, false otherwise</returns>
		public override bool DatabaseExists()
		{
			var ssb = new SqlConnectionStringBuilder(this.ConnectionString);
			var database = ssb.InitialCatalog;
			ssb.InitialCatalog = "master";
			using (var connection = new SqlConnection(ssb.ToString()))
			{
				connection.Open();
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, $"SELECT * FROM sys.databases WHERE Name = '{database}'");
				cmd.Connection = connection;
				using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					return reader.Read();
				}
			}
		}

		/// <summary>
		/// Drops the database using information from the connection string.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031", Justification = "The try/catch with no specificity is there to catch exceptions that whether they work or not, is irrelevant.")]
		[SuppressMessage("Microsoft.Security", "CA2100", Justification = "All scripts executed are internal to this method.")]
		public override void DropDatabase()
		{
			if (!this.DatabaseExists())
			{
				return;
			}

			var ssb = new SqlConnectionStringBuilder(this.ConnectionString);
			var database = ssb.InitialCatalog;
			ssb.InitialCatalog = "master";
			using (SqlConnection connection = new SqlConnection(ssb.ToString()))
			{
				connection.Open();
				Action<string, bool> tryExecute = (sql, thrw) =>
				{
					try
					{
						Logger.Debug(this.GetType(), $"Executing: {sql}");
						using (var dc = new SqlCommand(sql, connection))
						{
							dc.ExecuteNonQuery();
						}
					}
					catch (Exception ex)
					{
						Logger.Error(this.GetType(), ex);
						if (thrw)
						{
							throw;
						}
					}
				};

				tryExecute.Invoke($"ALTER DATABASE [{database}] SET single_user with rollback immediate", false);
				tryExecute.Invoke($"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{database}'", false);
				tryExecute.Invoke($"DROP DATABASE [{database}]", true);
			}
		}

		/// <summary>
		/// Calls Dispose(), then empties the connection pool.
		/// </summary>
		/// <param name="disposing">If the object is being disposed or not.</param>
		/// <exception cref="InvalidOperationException">If there are open transactions.</exception>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			SqlConnection.ClearAllPools();
		}

		#endregion
	}
}