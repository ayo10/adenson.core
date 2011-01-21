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
	/// The SqlHelper class is intended to encapsulate high performance, scalable best practices for
	/// common uses of SqlClient
	/// </summary>
	public sealed class SqlClientHelper : SqlHelperBase
	{
		#region Constructor

		public SqlClientHelper(ConnectionStringSettings connectionString) : base(connectionString)
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
			SqlClientHelper.CheckArgument(commandText, "commandText");
			SqlTransaction sqltransaction = SqlClientHelper.CheckTransaction(transaction);

			SqlCommand command = new SqlCommand(commandText);
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
			SqlClientHelper.CheckArgument(commandText, "commandText");
			SqlTransaction sqltransaction = SqlClientHelper.CheckTransaction(transaction);

			SqlCommand command = new SqlCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			this.AssignParameters(command, commandText, parameterValues);
			return this.ExecuteNonQuery(command);
		}
		public override int[] ExecuteNonQueryBatched(params string[] commandTexts)
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
						if (ins == String.Empty) continue;
						if (String.Equals(ins, "GO", StringComparison.CurrentCultureIgnoreCase))
						{
							if (last != String.Empty) sqls.Add(last.Trim());
							last = String.Empty;
						}
						else last += ins + Environment.NewLine;
					}
					return base.ExecuteNonQueryBatched(sqls.ToArray());
				}
			}
			return base.ExecuteNonQueryBatched(commandTexts);
		}
		public override IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(type, null, commandText, parameterValues);
		}
		public override IDataReader ExecuteReader(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			SqlClientHelper.CheckArgument(commandText, "commandText");
			SqlTransaction sqltransaction = SqlClientHelper.CheckTransaction(transaction);

			SqlCommand command = new SqlCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			this.AssignParameters(command, commandText, parameterValues);

			return this.ExecuteReader(command);
		}
		public override object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(type, null, commandText, parameterValues);
		}
		public override object ExecuteScalar(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			SqlClientHelper.CheckArgument(commandText, "commandText");
			SqlTransaction sqltransaction = SqlClientHelper.CheckTransaction(transaction);

			SqlCommand command = new SqlCommand(commandText);
			command.CommandType = type;
			if (sqltransaction != null) command.Transaction = sqltransaction;
			this.AssignParameters(command, commandText, parameterValues);

			return this.ExecuteScalar(command);
		}
		public override DbDataAdapter CreateAdapter(IDbCommand command)
		{
			return new SqlDataAdapter((SqlCommand)command);
		}
		public override IDbConnection CreateConnection()
		{
			return new SqlConnection(this.ConnectionString);
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

		private void AssignParameters(SqlCommand command, string commandText, object[] parameterValues)
		{
			if (!parameterValues.IsEmpty())
			{
				SqlParameter[] commandParameters;
				if (!SqlClientHelper.CheckParameters(parameterValues, out commandParameters))
				{
					if (command.CommandType == CommandType.StoredProcedure)
					{
						commandParameters = SqlClientParameterCache.GetSpParameterSet((SqlConnection)this.Manager.Connection, commandText);
						SqlClientHelper.AssignParameterValues(commandParameters, parameterValues);
					}
					else
					{
						if (commandText.IndexOf("{0}") > 0) command.CommandText = String.Format(commandText, parameterValues);
						else commandParameters = SqlClientHelper.GenerateParameters(commandText, parameterValues);
					}
				}
				if (commandParameters != null) command.Parameters.AddRange(commandParameters);
			}
		}

		private static void CheckArgument(string argument, string paramName)
		{
			if (String.IsNullOrEmpty(argument)) throw new ArgumentNullException(paramName, Exceptions.ArgumentNullOrEmpty);
		}
		private static SqlCommand CheckCommand(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command", Exceptions.ArgumentNull);
			SqlCommand sqlcommand = command as SqlCommand;
			if (command == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "SqlClient"), "command");
			return sqlcommand;
		}
		private static SqlTransaction CheckTransaction(IDbTransaction transaction)
		{
			SqlTransaction sqltransaction = null;
			if (transaction != null)
			{
				sqltransaction = transaction as SqlTransaction;
				if (sqltransaction == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "SqlClient"), "command");
			}
			return sqltransaction;
		}
		private static bool CheckParameters(object[] parameterValues, out SqlParameter[] comamndParameters)
		{
			comamndParameters = null;

			List<SqlParameter> list = new List<SqlParameter>();
			foreach (object obj in parameterValues)
			{
				if (obj is Parameter)
				{
					list.Add(((Parameter)obj).Convert<SqlParameter>());
					continue;
				}

				IDbDataParameter dbparameter = obj as IDbDataParameter;
				if (dbparameter != null)
				{
					SqlParameter sqlParameter = obj as SqlParameter;
					if (sqlParameter == null) throw new ArgumentException(String.Format(Exceptions.SqlImplWrongType, "SqlClient"), "command");
					list.Add(sqlParameter);
				}
				else return false;
				
			}
			comamndParameters = list.ToArray();
			return true;
		}
		private static SqlParameter[] GenerateParameters(string commandText, object[] parameterValues)
		{
			//insert into table (col1, col2) values (@col1, @col2)
			//delete from table where col1 = @col1
			//update table set col1 = @col1, col2 = @col2
			//select * from table where col1 = @col1
			string[] splits = commandText.Split('@');
			if (splits.Length == 1 && parameterValues.Length > 0) throw new ArgumentException(Exceptions.NoCommandParameter);
			if (splits.Length - 1 != parameterValues.Length) throw new ArgumentException(Exceptions.ParameterCountMismatch);
			List<SqlParameter> list = new List<SqlParameter>();
			for (int i = 1; i < splits.Length; i++)
			{
				string paramName = String.Concat("@", splits[i].Split(' ', ',', ')')[0]);
				SqlParameter parameter = new SqlParameter(paramName, parameterValues[i - 1]);
				list.Add(parameter);
			}
			if (list.Count != parameterValues.Length) throw new ArgumentException(Exceptions.CommandTextParse);
			return list.ToArray();
		}
		private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
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