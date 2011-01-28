using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
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
		/// Instantiates a new <see cref="SqlClientHelper"/> using <see cref="Configuration.ConnectionStrings.Default"/>
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
		/// Instantiates a new instance of <see cref="SqlClientHelper"/> using specified connection string setting object
		/// </summary>
		/// <param name="connectionString">The connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		public SqlClientHelper(string connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Runs a query to determine if the specified column exists in the specified table
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <param name="columnName">the column name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Runs a query to determine if the specified table exists
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public override bool CheckTableExists(string tableName)
		{
			bool result = false;
			using (IDataReader reader = this.ExecuteReader(CommandType.Text, "select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1", tableName))
			{
				result = reader.Read();
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
			return new SqlDataAdapter((SqlCommand)command);
		}
		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="SqlCommand"/> object</returns>
		public override DbCommand CreateCommand()
		{
			return new SqlCommand();
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="SqlConnection"/> object</returns>
		public override DbConnection CreateConnection()
		{
			return new SqlConnection(this.ConnectionString);
		}
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <returns>New <see cref="SqlParameter"/> object</returns>
		public override DbParameter CreateParameter()
		{
			return new SqlParameter();
		}
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteNonQuery run on each command text</returns>
		public override int[] ExecuteNonQuery(params string[] commandTexts)
		{
			if (commandTexts.Length == 1)
			{
				string str = commandTexts[0];
				string[] splits = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if (splits.Any(s => String.Equals(s, "GO", StringComparison.CurrentCultureIgnoreCase)))
				{
					//doing below for some odd reason changes the order of strings
					//splits = str.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

					List<string> sqls = new List<string>();
					string last = String.Empty;
					foreach (string ins in splits.Select(s => s.Trim()))
					{
						if (String.IsNullOrEmpty(ins)) continue;
						if (String.Equals(ins, "GO", StringComparison.CurrentCultureIgnoreCase))
						{
							if (!String.IsNullOrEmpty(last)) sqls.Add(last.Trim());
							last = String.Empty;
						}
						else last += ins + Environment.NewLine;
					}
					return base.ExecuteNonQuery(sqls.ToArray());
				}
			}
			return base.ExecuteNonQuery(commandTexts);
		}

		#endregion
	}
}