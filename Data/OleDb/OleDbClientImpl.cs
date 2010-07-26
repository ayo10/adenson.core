using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace Adenson.Data.OleDb
{
	/// <summary>
	/// The SqlHelper class is intended to encapsulate high performance, scalable best practices for
	/// common uses of SqlClient
	/// </summary>
	public sealed class OleDbClientImpl : SqlHelperBase
	{
		#region Constructor

		public OleDbClientImpl(ConnectionStringSettings connectionString) : base(connectionString)
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
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OleDbTransaction oleDbTransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (oleDbTransaction != null) command.Transaction = oleDbTransaction;
			this.AssignParameters(command, commandText, parameterValues);

			return this.ExecuteDataSet(command);
		}
		public override int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(type, null, commandText, parameterValues);
		}
		public override int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OleDbTransaction sqltransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (OleDbClientImpl.IsNotEmpty(parameterValues))
			{
				OleDbParameter[] commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
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
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OleDbTransaction sqltransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (OleDbClientImpl.IsNotEmpty(parameterValues))
			{
				OleDbParameter[] commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
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
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OleDbTransaction sqltransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (OleDbClientImpl.IsNotEmpty(parameterValues))
			{
				OleDbParameter[] commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteScalar(command);
		}
		public override DbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OleDbDataAdapter((OleDbCommand)command);
		}
		public override IDbConnection CreateConnection()
		{
			return new OleDbConnection(this.ConnectionString);
		}
		public override void ClearParameterCache()
		{
			OleDbParameterCache.Clear();
		}
		public override void ClearParameterCache(string spName)
		{
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", Exceptions.ArgumentNull);
			OleDbParameterCache.Clear(spName);
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

		private void AssignParameters(OleDbCommand command, string commandText, object[] parameterValues)
		{
			if (SqlHelperBase.IsNotEmpty(parameterValues))
			{
				OleDbParameter[] commandParameters;
				if (!OleDbClientImpl.CheckParameters(parameterValues, out commandParameters))
				{
					if (command.CommandType == CommandType.StoredProcedure)
					{
						commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
						OleDbClientImpl.AssignParameterValues(commandParameters, parameterValues);
					}
					else
					{
						if (commandText.IndexOf("{0}") > 0) command.CommandText = string.Format(commandText, parameterValues);
						else commandParameters = OleDbClientImpl.GenerateParameters(commandText, parameterValues);
					}
				}
				if (commandParameters != null) command.Parameters.AddRange(commandParameters);
			}
		}

		private static OleDbCommand CheckCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command", Exceptions.ArgumentNull);
			OleDbCommand sqlcommand = command as OleDbCommand;
			if (command == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "Oledb"), "command");
			return sqlcommand;
		}
		private static OleDbTransaction CheckTransaction(IDbTransaction transaction)
		{
			OleDbTransaction oledbTransaction = null;
			if (transaction != null)
			{
				oledbTransaction = transaction as OleDbTransaction;
				if (oledbTransaction == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "Oledb"), "command");
			}
			return oledbTransaction;
		}
		private static bool CheckParameters(object[] parameterValues, out OleDbParameter[] comamndParameters)
		{
			comamndParameters = null;

			List<OleDbParameter> list = new List<OleDbParameter>();
			foreach (object obj in parameterValues)
			{
				if (obj is Parameter)
				{
					list.Add(((Parameter)obj).Convert<OleDbParameter>());
					continue;
				}

				IDbDataParameter dbparameter = obj as IDbDataParameter;
				if (dbparameter != null)
				{
					OleDbParameter sqlParameter = obj as OleDbParameter;
					if (sqlParameter == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "Oledb"));
					list.Add(sqlParameter);
				}
				else return false;
				
			}
			comamndParameters = list.ToArray();
			return true;
		}
		private static OleDbParameter[] GenerateParameters(string commandText, object[] parameterValues)
		{
			string[] splits = commandText.Split('@');
			if (splits.Length == 1 && parameterValues.Length > 0) throw new ArgumentException(Exceptions.NoCommandParameter);
			if (splits.Length - 1 != parameterValues.Length) throw new ArgumentException(Exceptions.ParameterCountMismatch);
			List<OleDbParameter> list = new List<OleDbParameter>();
			for (int i = 1; i < splits.Length; i++)
			{
				string paramName = string.Concat("@", splits[i].Split(' ')[0]);
				OleDbParameter parameter = new OleDbParameter(paramName, parameterValues[i - 1]);
				list.Add(parameter);
			}
			if (list.Count != parameterValues.Length) throw new ArgumentException(Exceptions.CommandTextParse);
			return list.ToArray();
		}
		private static void AssignParameterValues(OleDbParameter[] commandParameters, object[] parameterValues)
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