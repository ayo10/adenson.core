using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using Adenson.Log;

namespace Adenson.Data.SqlCe
{
	public sealed class SqlCeImpl : SqlHelperBase
	{
		#region Variables
		private static Logger logger = new Logger(typeof(SqlCeImpl));
		#endregion
		#region Constructor

		public SqlCeImpl(string connectionKey) : base(connectionKey)
		{
		}
		public SqlCeImpl(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		public void Compact(string connectionString)
		{
			try
			{
				SqlCeEngine engine = new SqlCeEngine(connectionString);
				engine.Compact(connectionString);
			}
			catch (SqlCeException ex)
			{
				logger.Error(ex);
			}
		}
		public void CreateDatabase(string connectionString)
		{
			new SqlCeEngine(connectionString).CreateDatabase();
		}
		public void Upgrade(string connectionString)
		{
			new SqlCeEngine(connectionString).Upgrade();
		}

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
			if (type != CommandType.Text) throw new NotSupportedException(ExceptionMessages.SqlCommandTypeTextNotSupported);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNull);

			SqlCeParameter[] commandParameters = SqlCeImpl.ConvertParameters(ref commandText, parameterValues);
			SqlCeCommand command = new SqlCeCommand(commandText);
			command.CommandType = type;
			if (transaction != null) command.Transaction = (SqlCeTransaction)transaction;
			SqlCeImpl.AttachParameters(command, commandParameters);

			return this.ExecuteDataSet(command);
		}
		public override int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(type, null, commandText, parameterValues);
		}
		public override int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (type != CommandType.Text) throw new NotSupportedException(ExceptionMessages.SqlCommandTypeTextNotSupported);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNull);

			SqlCeParameter[] commandParameters = SqlCeImpl.ConvertParameters(ref commandText, parameterValues);
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
			if (type != CommandType.Text) throw new NotSupportedException(ExceptionMessages.SqlCommandTypeTextNotSupported);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNull);

			SqlCeParameter[] commandParameters = SqlCeImpl.ConvertParameters(ref commandText, parameterValues);
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
			SqlCeParameter[] commandParameters = SqlCeImpl.ConvertParameters(ref commandText, parameterValues);
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

			Logger.Debug(typeof(SqlCeImpl), str);
			#endif
		}

		#endregion
	}
}
