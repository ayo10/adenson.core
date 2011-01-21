using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using Adenson.Log;

namespace Adenson.Data
{
	/// <summary>
	/// The SqlHelper class for SQL Server connections
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
		/// <param name="tableName"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			if (String.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (String.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");
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
		/// <param name="tableName"></param>
		/// <returns></returns>
		public override bool CheckTableExists(string tableName)
		{
			if (String.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			bool result = false;
			using (IDataReader r = this.ExecuteReader(CommandType.Text, "SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName + "'"))
			{
				result = r.Read();
			}
			return result;
		}
		public override DbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new SqlCeDataAdapter((SqlCeCommand)command);
		}
		public override IDbConnection CreateConnection()
		{
			return new SqlCeConnection(this.ConnectionString);
		}
		public override DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(type, null, commandText, parameterValues);
		}
		public override DataSet ExecuteDataSet(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (type != CommandType.Text) throw new NotSupportedException(Exceptions.SqlCommandTypeTextNotSupported);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);

			SqlCeParameter[] commandParameters = SqlCeHelper.ConvertParameters(ref commandText, parameterValues);
			SqlCeCommand command = new SqlCeCommand(commandText);
			command.CommandType = type;
			if (transaction != null) command.Transaction = (SqlCeTransaction)transaction;
			SqlCeHelper.AttachParameters(command, commandParameters);

			return this.ExecuteDataSet(command);
		}
		public override int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(type, null, commandText, parameterValues);
		}
		public override int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (type != CommandType.Text) throw new NotSupportedException(Exceptions.SqlCommandTypeTextNotSupported);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);

			SqlCeParameter[] commandParameters = SqlCeHelper.ConvertParameters(ref commandText, parameterValues);
			SqlCeCommand command = new SqlCeCommand(commandText);
			command.CommandType = type;
			if (transaction != null) command.Transaction = (SqlCeTransaction)transaction;
			AttachParameters(command, commandParameters);

			return this.ExecuteNonQuery(command);
		}
		public override IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(type, null, commandText, parameterValues);
		}
		public override IDataReader ExecuteReader(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (type != CommandType.Text) throw new NotSupportedException(Exceptions.SqlCommandTypeTextNotSupported);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);

			SqlCeParameter[] commandParameters = SqlCeHelper.ConvertParameters(ref commandText, parameterValues);
			SqlCeCommand command = new SqlCeCommand(commandText);
			command.CommandType = type;
			if (transaction != null) command.Transaction = (SqlCeTransaction)transaction;
			AttachParameters(command, commandParameters);

			return this.ExecuteReader(command);
		}
		public override object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(type, null, commandText, parameterValues);
		}
		public override object ExecuteScalar(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			SqlCeParameter[] commandParameters = SqlCeHelper.ConvertParameters(ref commandText, parameterValues);
			SqlCeCommand command = new SqlCeCommand(commandText);
			command.CommandType = type;
			if (transaction != null) command.Transaction = (SqlCeTransaction)transaction;
			AttachParameters(command, commandParameters);

			return this.ExecuteScalar(command);
		}

		private static void AttachParameters(SqlCeCommand command, SqlCeParameter[] commandParameters)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandParameters == null) return;
			foreach (SqlCeParameter p in commandParameters.Where(p => p != null))
			{
				// Check for derived output value with no value assigned
				if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value == null)) p.Value = DBNull.Value;
				command.Parameters.Add(p);
			}
		}
		private static SqlCeParameter[] ConvertParameters(ref string commandText, object[] parameterValues)
		{
			string[] splits = commandText.Split('?');
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(splits[0].TrimEnd('\''));
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				SqlCeParameter[] commandParameters = new SqlCeParameter[parameterValues.Length];
				for (int i = 0; i < parameterValues.Length; i++)
				{
					if (parameterValues[i] is SqlCeParameter) commandParameters[i] = (SqlCeParameter)parameterValues[i];
					else
					{
						string paramName = "@Parameter" + i;
						sb.Append(paramName);
						sb.Append(splits[i + 1].TrimStart('\''));
						commandParameters[i] = new SqlCeParameter();
						commandParameters[i].ParameterName = paramName;
						switch (parameterValues[i].GetType().Name)
						{
							case "System.Boolean":
								parameterValues[i] = Convert.ToBoolean(parameterValues[i]) ? 1 : 0;
								commandParameters[i].SqlDbType = SqlDbType.Bit;
								break;
							case "System.DateTime": commandParameters[i].SqlDbType = SqlDbType.DateTime; break;
							case "System.String": commandParameters[i].SqlDbType = SqlDbType.NVarChar; break;
						}
						commandParameters[i].Value = parameterValues[i];
					}
				}
				commandText = sb.ToString();
				return commandParameters;
			}
			return null;
		}
		private static void WriteLog(string commandText, SqlCeParameter[] parameters)
		{
			#if DEBUG
			string str = commandText;
			if (parameters != null)
			{
				str += " [";
				foreach (SqlCeParameter param in parameters)
				{
					str += param.ParameterName + "=" + param.Value + ",";
				}
				str = str.TrimEnd(',') + "]";
			}

			Logger.Debug(typeof(SqlCeHelper), str);
			#endif
		}

		#endregion
	}
}
