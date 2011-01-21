using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace Adenson.Data
{
	/// <summary>
	/// The OdbcHelper class is intended to encapsulate high performance, scalable best practices for
	/// common uses of OdbcClient
	/// </summary>
	public sealed class OdbcSqlHelper : SqlHelperBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="OdbcSqlHelper"/> class.
		/// </summary>
		/// <param name="connectionString"></param>
		public OdbcSqlHelper(ConnectionStringSettings connectionString) : base(connectionString)
		{
		}

		#endregion
		#region Methods

		/// <summary>
		/// Executes and returns a new DataSet from specified stored procedure
		/// </summary>
		/// <param name="type"></param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		public override DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified stored procedure using specified transaction
		/// </summary>
		/// <param name="type"></param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		public override DataSet ExecuteDataSet(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			OdbcTransaction sqltransaction = OdbcSqlHelper.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			this.AssignParameters(command, commandText, parameterValues);

			return this.ExecuteDataSet(command);
		}
		/// <summary>
		/// Executes the specified command text and returns the number of rows affected.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		public override int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the specified command text using specified transaction and returns the number of rows affected.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		public override int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OdbcTransaction sqltransaction = OdbcSqlHelper.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (!parameterValues.IsEmpty())
			{
				OdbcParameter[] commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteNonQuery(command);
		}
		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public override IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the specified command text using the specified transaction and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public override IDataReader ExecuteReader(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OdbcTransaction sqltransaction = OdbcSqlHelper.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (!parameterValues.IsEmpty())
			{
				OdbcParameter[] commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteReader(command);
		}
		/// <summary>
		/// Executes the specified command text returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public override object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the specified command text using specified transaction returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>
		/// The first column of the first row in the resultset.
		/// </returns>
		public override object ExecuteScalar(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OdbcTransaction sqltransaction = OdbcSqlHelper.CheckTransaction(transaction);

			OdbcCommand command = new OdbcCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (!parameterValues.IsEmpty())
			{
				OdbcParameter[] commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
				AssignParameterValues(commandParameters, parameterValues);
				command.Parameters.AddRange(commandParameters);
			}
			return this.ExecuteScalar(command);
		}
		/// <summary>
		/// Creates a new DbDataAdapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New DbDataAdapter adapter</returns>
		public override DbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new OdbcDataAdapter((OdbcCommand)command);
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New IDbConnection object</returns>
		public override IDbConnection CreateConnection()
		{
			return new OdbcConnection(this.ConnectionString);
		}
		/// <summary>
		/// Runs a query to see if the specified column exists or not. (This method is not supported)
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Runs a query to see if specified table exists or not. (This method is not supported)
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public override bool CheckTableExists(string tableName)
		{
			throw new NotSupportedException();
		}

		private void AssignParameters(OdbcCommand command, string commandText, object[] parameterValues)
		{
			if (parameterValues.IsEmpty()) return;
			OdbcParameter[] commandParameters;
			if (!OdbcSqlHelper.CheckParameters(parameterValues, out commandParameters))
			{
				if (command.CommandType == CommandType.StoredProcedure)
				{
					commandParameters = OdbcParameterCache.GetSpParameterSet((OdbcConnection)this.Manager.Connection, commandText);
					OdbcSqlHelper.AssignParameterValues(commandParameters, parameterValues);
				}
				else
				{
					if (commandText.IndexOf("{0}") > 0) command.CommandText = String.Format(commandText, parameterValues);
					else commandParameters = OdbcSqlHelper.GenerateParameters(commandText, parameterValues);
				}
			}
			if (commandParameters != null) command.Parameters.AddRange(commandParameters);
		}

		private static OdbcTransaction CheckTransaction(IDbTransaction transaction)
		{
			OdbcTransaction sqltransaction = null;
			if (transaction != null)
			{
				sqltransaction = transaction as OdbcTransaction;
				if (sqltransaction == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "Odbc"), "sqltransaction");
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
					if (sqlParameter == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "Odbc"));
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
			if (splits.Length == 1 && parameterValues.Length > 0) throw new ArgumentException(Exceptions.NoCommandParameter);
			if (splits.Length - 1 != parameterValues.Length) throw new ArgumentException(Exceptions.ParameterCountMismatch);
			List<OdbcParameter> list = new List<OdbcParameter>();
			for (int i = 1; i < splits.Length; i++)
			{
				string paramName = string.Concat("@", splits[i].Split(' ')[0]);
				OdbcParameter parameter = new OdbcParameter(paramName, parameterValues[i - 1]);
				list.Add(parameter);
			}
			if (list.Count != parameterValues.Length) throw new ArgumentException(Exceptions.CommandTextParse);
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