using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Globalization;
using Adenson.Log;

namespace Adenson.Data
{
	internal sealed class SqlHelper
	{
		#region ExecuteNonQuery

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <param name="connection">A valid SqlCeConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlCeConnection connection, string commandText, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			SqlCeParameter[] commandParameters = ConvertParameters(ref commandText, parameterValues);
			try
			{
				SqlCeCommand cmd = new SqlCeCommand();
				bool mustCloseConnection = false;
				SqlHelper.PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);
				int retval = cmd.ExecuteNonQuery();
				if (mustCloseConnection) connection.Close();
				return retval;
			}
			catch (SqlCeException ex)
			{
				Log(commandText, commandParameters);
				Logger.Error(typeof(SqlHelper), ex);
				throw;
			}
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns no resultset) against the specified SqlCeTransaction
		/// using the provided parameters.
		/// </summary>
		/// <param name="transaction">A valid SqlCeTransaction</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlCeTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			
			SqlCeParameter[] commandParameters = ConvertParameters(ref commandText, parameterValues);
			try
			{
				// Create a command and prepare it for execution
				SqlCeCommand cmd = new SqlCeCommand();
				bool mustCloseConnection = false;
				PrepareCommand(cmd, (SqlCeConnection)transaction.Connection, transaction, commandText, commandParameters, out mustCloseConnection);

				// Finally, execute the command
				int retval = cmd.ExecuteNonQuery();

				// Detach the SqlCeParameters from the command object, so they can be used again
				cmd.Parameters.Clear();
				return retval;
			}
			catch (SqlCeException ex)
			{
				Log(commandText, commandParameters);
				Logger.Error(typeof(SqlHelper), ex);
				throw;
			}
		}

		#endregion ExecuteNonQuery
		#region ExecuteReader

		private enum SqlCeConnectionOwnership
		{
			/// <summary>Connection is owned and managed by SqlHelper</summary>
			Internal,
			/// <summary>Connection is owned and managed by the caller</summary>
			External
		}

		/// <summary>
		/// Create and prepare a SqlCeCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// </summary>
		/// <param name="connection">A valid SqlCeConnection, on which to execute this command</param>
		/// <param name="transaction">A valid SqlCeTransaction, or 'null'</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlCeParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
		/// <returns>SqlCeDataReader containing the results of the command</returns>
		private static SqlCeDataReader ExecuteReader(SqlCeConnection connection, SqlCeTransaction transaction, string commandText, SqlCeParameter[] commandParameters, SqlCeConnectionOwnership connectionOwnership)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			bool mustCloseConnection = false;
			// Create a command and prepare it for execution
			SqlCeCommand cmd = new SqlCeCommand();
			try
			{
				PrepareCommand(cmd, connection, transaction, commandText, commandParameters, out mustCloseConnection);

				// Create a reader
				SqlCeDataReader dataReader;

				// Call ExecuteReader with the appropriate CommandBehavior
				if (connectionOwnership == SqlCeConnectionOwnership.External) dataReader = cmd.ExecuteReader();
				else dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

				// Detach the SqlCeParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched
				// when the reader is closed, so if the parameters are detached from the command
				// then the SqlCeReader can´t set its values.
				// When this happen, the parameters can´t be used again in other command.
				bool canClear = true;
				foreach (SqlCeParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input) canClear = false;
				}

				if (canClear) cmd.Parameters.Clear();

				return dataReader;
			}
			catch (SqlCeException ex)
			{
				Log(commandText, commandParameters);
				if (mustCloseConnection) connection.Close();
				Logger.Error(typeof(SqlHelper), ex);
				throw;
			}
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset and takes no parameters) against the provided SqlCeConnection.
		/// </summary>
		/// <param name="connection">A valid SqlCeConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A SqlCeDataReader containing the resultset generated by the command</returns>
		public static SqlCeDataReader ExecuteReader(SqlCeConnection connection, string commandText)
		{
			// Pass through the call providing null for the set of SqlCeParameters
			return ExecuteReader(connection, commandText, (SqlCeParameter[])null);
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset) against the specified SqlCeConnection
		/// using the provided parameters.
		/// </summary>
		/// <param name="connection">A valid SqlCeConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="parameterValues">An array of SqlCeParamters used to execute the command</param>
		/// <returns>A SqlCeDataReader containing the resultset generated by the command</returns>
		public static SqlCeDataReader ExecuteReader(SqlCeConnection connection, string commandText, params object[] parameterValues)
		{
			SqlCeParameter[] parameters = ConvertParameters(ref commandText, parameterValues);
			return ExecuteReader(connection, commandText, parameters);
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset) against the specified SqlCeConnection
		/// using the provided parameters.
		/// </summary>
		/// <param name="connection">A valid SqlCeConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlCeParamters used to execute the command</param>
		/// <returns>A SqlCeDataReader containing the resultset generated by the command</returns>
		public static SqlCeDataReader ExecuteReader(SqlCeConnection connection, string commandText, SqlCeParameter[] commandParameters)
		{
			// Pass through the call to the private overload using a null transaction value and an externally owned connection
			return ExecuteReader(connection, null, commandText, commandParameters, SqlCeConnectionOwnership.External);
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset and takes no parameters) against the provided SqlCeTransaction.
		/// </summary>
		/// <param name="transaction">A valid SqlCeTransaction</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>A SqlCeDataReader containing the resultset generated by the command</returns>
		public static SqlCeDataReader ExecuteReader(SqlCeTransaction transaction, string commandText)
		{
			// Pass through the call providing null for the set of SqlCeParameters
			return ExecuteReader(transaction, commandText, (SqlCeParameter[])null);
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset) against the specified SqlCeTransaction
		/// using the provided parameters.
		/// </summary>
		/// <param name="transaction">A valid SqlCeTransaction</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlCeParamters used to execute the command</param>
		/// <returns>A SqlCeDataReader containing the resultset generated by the command</returns>
		public static SqlCeDataReader ExecuteReader(SqlCeTransaction transaction, string commandText, SqlCeParameter[] commandParameters)
		{
			Log(commandText, commandParameters);
			if (transaction == null) throw new ArgumentNullException("transaction");
			return ExecuteReader((SqlCeConnection)transaction.Connection, transaction, commandText, commandParameters, SqlCeConnectionOwnership.External);
		}

		#endregion ExecuteReader
		#region ExecuteDataset

		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset) against the specified SqlCeConnection
		/// using the provided parameters.
		/// </summary>
		/// <param name="connection">A valid SqlCeConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="parameterValues">An array of SqlCeParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataSet(SqlCeConnection connection, string commandText, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			SqlCeParameter[] commandParameters = ConvertParameters(ref commandText, parameterValues);

			SqlCeCommand cmd = new SqlCeCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, null, commandText, commandParameters, out mustCloseConnection);

			using (SqlCeDataAdapter da = new SqlCeDataAdapter(cmd))
			{
				DataSet ds = new DataSet();
				da.Fill(ds);
				if (mustCloseConnection) connection.Close();
				return ds;
			}
		}
		/// <summary>
		/// Execute a SqlCeCommand (that returns a resultset) against the specified SqlCeTransaction
		/// using the provided parameters.
		/// </summary>
		/// <param name="transaction">A valid SqlCeTransaction</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="parameterValues">An array of SqlCeParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataSet(SqlCeTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			SqlCeParameter[] commandParameters = ConvertParameters(ref commandText, parameterValues);

			SqlCeCommand cmd = new SqlCeCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, (SqlCeConnection)transaction.Connection, transaction, commandText, commandParameters, out mustCloseConnection);

			using (SqlCeDataAdapter da = new SqlCeDataAdapter(cmd))
			{
				DataSet ds = new DataSet();
				da.Fill(ds);
				cmd.Parameters.Clear();
				return ds;
			}
		}

		#endregion ExecuteDataset
		#region Helper Methods

		public static bool ColumnExists(SqlCeConnection connection, string tableName, string columnName)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (String.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			if (String.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");
			bool result = false;
			using (SqlCeDataReader r = SqlHelper.ExecuteReader(connection, "SELECT * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' AND COLUMN_NAME = '" + columnName + "'"))
			{
				result = r.Read();
			}
			return result;
		}
		public static void Compact(string connectionString)
		{
			try
			{
				SqlCeEngine engine = new SqlCeEngine(connectionString);
				engine.Compact(connectionString);
			}
			catch (SqlCeException ex)
			{
				Logger.Error(typeof(SqlHelper), ex);
			}
		}
		public static void CreateDatabase(string connectionString)
		{
			new SqlCeEngine(connectionString).CreateDatabase();
		}
		public static bool TableExists(SqlCeConnection connection, string tableName)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (String.IsNullOrEmpty(tableName)) throw new ArgumentNullException("tableName");
			bool result = false;
			using (SqlCeDataReader r = SqlHelper.ExecuteReader(connection, "SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName + "'"))
			{
				result = r.Read();
			}
			return result;
		}
		public static void Upgrade(string connectionString)
		{
			new SqlCeEngine(connectionString).Upgrade();
		}

		private static void AttachParameters(SqlCeCommand command, SqlCeParameter[] commandParameters)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandParameters != null)
			{
				foreach (SqlCeParameter p in commandParameters)
				{
					if (p != null)
					{
						// Check for derived output value with no value assigned
						if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value == null))
						{
							p.Value = DBNull.Value;
						}
						command.Parameters.Add(p);
					}
				}
			}
		}
		private static void PrepareCommand(SqlCeCommand command, SqlCeConnection connection, SqlCeTransaction transaction, string commandText, SqlCeParameter[] commandParameters, out bool mustCloseConnection)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			if (connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				connection.Open();
			}
			else mustCloseConnection = false;

			command.Connection = connection;
			command.CommandText = commandText;
			if (transaction != null) command.Transaction = transaction;
			if (commandParameters != null) AttachParameters(command, commandParameters);
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
								parameterValues[i] = System.Convert.ToBoolean(parameterValues[i], CultureInfo.CurrentCulture) ? 1 : 0;
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
		private static void Log(string commandText, SqlCeParameter[] parameters)
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

			Logger.Debug(typeof(SqlHelper), str);
#endif
		}

		#endregion
	}
}