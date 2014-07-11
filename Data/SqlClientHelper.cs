using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

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
		/// <exception cref="ArgumentNullException">If specified connection string null</exception>
		/// <exception cref="ArgumentException">If specified connection string object has an invalid connection string</exception>
		public SqlClientHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientHelper"/> class using specified connection string setting object
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid.</exception>
		public SqlClientHelper(string keyOrConnectionString) : base(keyOrConnectionString)
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
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, StringUtil.Format("CREATE DATABASE [{0}]", database));
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
				SqlCommand cmd = (SqlCommand)this.CreateCommand(CommandType.Text, StringUtil.Format("SELECT * FROM sys.databases WHERE Name = '{0}'", database));
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
				Action<string> a = sql =>
				{
					try
					{
						new SqlCommand(sql, connection).ExecuteNonQuery();
					}
				catch
					{
					}
				};

				a.Invoke(StringUtil.Format("ALTER DATABASE [{0}] SET single_user with rollback immediate", database));
				a.Invoke(StringUtil.Format("EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{0}'", database));
				new SqlCommand(StringUtil.Format("DROP DATABASE [{0}]", database), connection).ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Reads the specified stream split into strings (delimiting using <see cref="Environment.NewLine"/> and "GO"), and runs them in batched mode.
		/// </summary>
		/// <param name="stream">The stream containing the commmand text to run.</param>
		/// <returns>The result of each ExecuteNonQuery run on each command text.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
		public override int[] ExecuteNonQueries(StreamReader stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			string text = stream.ReadToEnd();
			string[] splits = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if (splits.Any(s => String.Equals(s, "GO", StringComparison.CurrentCultureIgnoreCase)))
			{
				// doing the following below for some odd reason changes the order of strings
				// splits = str.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
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

				return base.ExecuteNonQueries(sqls.ToArray());
			}

			return new int[] { this.ExecuteNonQuery(text) };
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