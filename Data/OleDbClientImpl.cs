using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace Adenson.Data.OleDb
{
	/// <summary>
	/// The SqlHelper class for Oledb connections
	/// </summary>
	public sealed class OleDbClientImpl : SqlHelperBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbClientImpl"/> class.
		/// </summary>
		/// <param name="connectionString"></param>
		public OleDbClientImpl(ConnectionStringSettings connectionString) : base(connectionString)
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
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);
			OleDbTransaction oleDbTransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (oleDbTransaction != null) command.Transaction = oleDbTransaction;
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
			OleDbTransaction sqltransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (!parameterValues.IsEmpty())
			{
				OleDbParameter[] commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
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
			OleDbTransaction sqltransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (!parameterValues.IsEmpty())
			{
				OleDbParameter[] commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
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
			OleDbTransaction sqltransaction = OleDbClientImpl.CheckTransaction(transaction);

			OleDbCommand command = new OleDbCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			if (!parameterValues.IsEmpty())
			{
				OleDbParameter[] commandParameters = OleDbParameterCache.GetSpParameterSet((OleDbConnection)this.Manager.Connection, commandText);
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
			return new OleDbDataAdapter((OleDbCommand)command);
		}
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New IDbConnection object</returns>
		public override IDbConnection CreateConnection()
		{
			return new OleDbConnection(this.ConnectionString);
		}
		/// <summary>
		/// Runs a system check for the existence of specified column in the specified table (Not supported)
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <param name="columnName">the column name</param>
		/// <returns>
		/// True if the table exists, false otherwise
		/// </returns>
		public override bool CheckColumnExists(string tableName, string columnName)
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Runs a system check for the existence of specified table (Not supported)
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>
		/// True if the table exists, false otherwise
		/// </returns>
		public override bool CheckTableExists(string tableName)
		{
			throw new NotSupportedException();
		}

		private void AssignParameters(OleDbCommand command, string commandText, object[] parameterValues)
		{
			if (!parameterValues.IsEmpty())
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
						if (commandText.IndexOf("{0}") > 0) command.CommandText = String.Format(commandText, parameterValues);
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