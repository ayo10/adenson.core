using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Adenson.Data
{
	/// <summary>
	/// The SqlHelper class for SQL Server Client connections
	/// </summary>
	public sealed class SqlClientHelper : SqlHelperBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class using <see cref="Configuration.ConnectionStrings.Default"/>
		/// </summary>
		public SqlClientHelper() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string settings object to use</param>
		/// <exception cref="ArgumentNullException">if specified connection string null</exception>
		/// <exception cref="ArgumentException">if specified connection string object has an invalid connection string</exception>
		public SqlClientHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class using specified connection string setting object
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		public SqlClientHelper(string keyOrConnectionString)
			: base(keyOrConnectionString)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Creates a new data adapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="SqlDataAdapter"/> object</returns>
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new SqlDataAdapter((SqlCommand)command);
		}

		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="SqlCommand"/> object</returns>
		public override IDbCommand CreateCommand()
		{
			return new SqlCommand();
		}

		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="SqlConnection"/> object</returns>
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
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, null, StringUtil.Format("CREATE DATABASE [{0}]", database));
				cmd.Connection = connection;
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands
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
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, null, StringUtil.Format("SELECT * FROM sys.databases WHERE Name = '{0}'", database));
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
		public override void DropDatabase()
		{
			var ssb = new SqlConnectionStringBuilder(this.ConnectionString);
			var database = ssb.InitialCatalog;
			ssb.InitialCatalog = "master";
			using (var connection = new SqlConnection(ssb.ToString()))
			{
				connection.Open();
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, null, StringUtil.Format("EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{0}'\r\nDROP DATABASE [{0}]", database));
				cmd.Connection = connection;
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteNonQuery run on each command text</returns>
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commandTexts"/> is null</exception>
		/// <exception cref="ArgumentException">if <paramref name="commandTexts"/> is empty</exception>
		public override int[] ExecuteNonQuery(params string[] commandTexts)
		{
			if (commandTexts.Length == 1)
			{
				string str = commandTexts[0];
				string[] splits = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if (splits.Any(s => String.Equals(s, "GO", StringComparison.CurrentCultureIgnoreCase)))
				{
					// doing the following below for some odd reason changes the order of strings
					////splits = str.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

					List<string> sqls = new List<string>();
					string last = String.Empty;
					foreach (string ins in splits.Select(s => s.Trim()))
					{
						if (String.IsNullOrEmpty(ins))
						{
							continue;
						}

						if (String.Equals(ins, "GO", StringComparison.CurrentCultureIgnoreCase))
						{
							if (!String.IsNullOrEmpty(last))
							{
								sqls.Add(last.Trim());
							}

							last = String.Empty;
						}
						else
						{
							last += ins + Environment.NewLine;
						}
					}

					if (!String.IsNullOrEmpty(last))
					{
						sqls.Add(last.Trim());
					}

					return base.ExecuteNonQuery(sqls.ToArray());
				}
			}

			return base.ExecuteNonQuery(commandTexts);
		}

		#endregion
	}
}