using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace Adenson.Data.Odbc
{
	/// <summary>
	/// The OdbcHelper class is intended to encapsulate high performance, scalable best practices for
	/// common uses of OdbcClient
	/// </summary>
	public sealed class OdbcClientImpl : SqlHelperBase
	{
		#region Constructor

		public OdbcClientImpl(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		public override DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(type, null, commandText, parameterValues);
		}
		public override DataSet ExecuteDataSet(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			OdbcTransaction sqltransaction = OdbcClientImpl.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			this.AssignParameters(command, commandText, parameterValues);

			return this.ExecuteDataSet(command);
		}
		public override int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(type, null, commandText, parameterValues);
		}
		public override int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNull);
			OdbcTransaction sqltransaction = OdbcClientImpl.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (OdbcClientImpl.IsNotEmpty(parameterValues))
			{
				OdbcParameter[] commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteNonQuery(command);
		}
		public override IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(type, null, commandText, parameterValues);
		}
		public override IDataReader ExecuteReader(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNull);
			OdbcTransaction sqltransaction = OdbcClientImpl.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (OdbcClientImpl.IsNotEmpty(parameterValues))
			{
				OdbcParameter[] commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteReader(command);
		}
		public override object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(type, null, commandText, parameterValues);
		}
		public override object ExecuteScalar(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNull);
			OdbcTransaction sqltransaction = OdbcClientImpl.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (OdbcClientImpl.IsNotEmpty(parameterValues))
			{
				OdbcParameter[] commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteScalar(command);
		}
		public override DbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OdbcDataAdapter((OdbcCommand)command);
		}
		public override IDbConnection CreateConnection()
		{
			return new OdbcConnection(this.ConnectionString);
		}
		public override void ClearParameterCache()
		{
			OdbcParameterCache.Clear();
		}
		public override void ClearParameterCache(string spName)
		{
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNull);
			OdbcParameterCache.Clear(spName);
		}
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			throw new NotImplementedException();
		}
		public override bool CheckTableExists(string tableName)
		{
			bool result = false;
			using (IDataReader reader = this.ExecuteReader(CommandType.Text, "select * from dbo.sysobjects where id = object_id(N'{0}') and OBJECTPROPERTY(id, N'IsUserTable') = 1", tableName))
			{
				result = reader.Read();
			}
			return result;
		}

		private void AssignParameters(OdbcCommand command, string commandText, object[] parameterValues)
		{
			if (SqlHelperBase.IsNotEmpty(parameterValues))
			{
				OdbcParameter[] commandParameters;
				if (!OdbcClientImpl.CheckParameters(parameterValues, out commandParameters))
				{
					if (command.CommandType == CommandType.StoredProcedure)
					{
						commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
						OdbcClientImpl.AssignParameterValues(commandParameters, parameterValues);
					}
					else
					{
						if (commandText.IndexOf("{0}") > 0) command.CommandText = string.Format(commandText, parameterValues);
						else commandParameters = OdbcClientImpl.GenerateParameters(commandText, parameterValues);
					}
				}
				if (commandParameters != null) command.Parameters.AddRange(commandParameters);
			}
		}

		private static OdbcCommand CheckCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command", ExceptionMessages.ArgumentNull);
			OdbcCommand sqlcommand = command as OdbcCommand;
			if (command == null) throw new ArgumentException(String.Format(ExceptionMessages.SqlImplWrongType, "Odbc"), "command");
			return sqlcommand;
		}
		private static OdbcTransaction CheckTransaction(IDbTransaction transaction)
		{
			OdbcTransaction sqltransaction = null;
			if (transaction != null)
			{
				sqltransaction = transaction as OdbcTransaction;
				if (sqltransaction == null) throw new ArgumentException(String.Format(ExceptionMessages.SqlImplWrongType, "Odbc"), "command");
			}
			return sqltransaction;
		}
		private static bool CheckParameters(object[] parameterValues, out OdbcParameter[] comamndParameters)
		{
			comamndParameters = null;

			List<OdbcParameter> list = new List<OdbcParameter>();
			foreach (object obj in parameterValues)
			{
				if (obj is Parameter)
				{
					list.Add(((Parameter)obj).Convert<OdbcParameter>());
					continue;
				}

				IDbDataParameter dbparameter = obj as IDbDataParameter;
				if (dbparameter != null)
				{
					OdbcParameter sqlParameter = obj as OdbcParameter;
					if (sqlParameter == null) throw new ArgumentException(String.Format(ExceptionMessages.SqlImplWrongType, "Odbc"));
					list.Add(sqlParameter);
				}
				else return false;
				
			}
			comamndParameters = list.ToArray();
			return true;
		}
		private static OdbcParameter[] GenerateParameters(string commandText, object[] parameterValues)
		{
			string[] splits = commandText.Split('@');
			if (splits.Length == 1 && parameterValues.Length > 0) throw new ArgumentException(ExceptionMessages.NoCommandParameter);
			if (splits.Length - 1 != parameterValues.Length) throw new ArgumentException(ExceptionMessages.ParameterCountMismatch);
			List<OdbcParameter> list = new List<OdbcParameter>();
			for (int i = 1; i < splits.Length; i++)
			{
				string paramName = string.Concat("@", splits[i].Split(' ')[0]);
				OdbcParameter parameter = new OdbcParameter(paramName, parameterValues[i - 1]);
				list.Add(parameter);
			}
			if (list.Count != parameterValues.Length) throw new ArgumentException(ExceptionMessages.CommandTextParse);
			return list.ToArray();
		}
		private static void AssignParameterValues(OdbcParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters == null) || (parameterValues == null)) return;

			if (commandParameters.Length != parameterValues.Length)
			{
				object[] newParamVals = new object[Math.Max(commandParameters.Length, parameterValues.Length)];
				for (int i = 0; i < parameterValues.Length; i++) newParamVals[i] = parameterValues[i];
				for (int i = parameterValues.Length; i < commandParameters.Length; i++) newParamVals[i] = DBNull.Value;
				parameterValues = newParamVals;
			}

			for (int i = 0; i < commandParameters.Length; i++)
			{
				if (parameterValues[i] is IDbDataParameter)
				{
					IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
					if (paramInstance.Value == null) commandParameters[i].Value = DBNull.Value;
					else commandParameters[i].Value = paramInstance.Value;
				}
				else if (parameterValues[i] == null) commandParameters[i].Value = DBNull.Value;
				else commandParameters[i].Value = parameterValues[i];
			}
		}

		#endregion
	}
}