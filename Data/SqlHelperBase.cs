using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using Adenson.Log;

namespace Adenson.Data
{
	/// <summary>
	/// Sql Helper base class
	/// </summary>
	public abstract class SqlHelperBase : IDisposable
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(SqlHelperBase));
		private static string[] crudites = new string[] { "alter ", "create ", "select ", "update ", "delete " };
		private bool mustCloseConnection = true;
		private ConnectionManager _connection;
		#endregion
		#region Constructors

		/// <summary>
		/// When implemented, should instantiate a new instance of the sql helper using specified connection as a
		/// connection string if isConnectionString is true, else uses ConnectionStrings.GetCS to retrieve it
		/// </summary>
		/// <param name="connectionString">The connection string settings object to use</param>
		protected SqlHelperBase(ConnectionStringSettings connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException("connectionString");
			this.ConnectionString = connectionString.ConnectionString;
		}
		
		#endregion
		#region Properties
		
		/// <summary>
		/// Gets the connection string to use
		/// </summary>
		public string ConnectionString
		{
			get;
			private set;
		}
		/// <summary>
		/// Gets the current connection object in use (if OpenConnection was invoked)
		/// </summary>
		public IDbConnection CurrentConnection
		{
			get { return this.Manager.Connection; }
		}

		internal ConnectionManager Manager
		{
			get
			{
				if (_connection == null || _connection.Connection == null)
				{
					_connection = new ConnectionManager(this.CreateConnection());
					_connection.AllowClose = mustCloseConnection;
				}
				return _connection;
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Closes the connection opened by OpenConnection, then, lets CreateConnection know to always create new connections henceforth.
		/// </summary>
		/// <exception cref="InvalidOperationException">if the method was called out of sequence, i.e., OpenConnection was never called, or called once and CloseConnection called multiple times.</exception>
		public virtual void CloseConnection()
		{
			if (this.Manager.AllowClose) throw new InvalidOperationException("OpenConnection must be called before CloseConnection.");
			this.Manager.AllowClose = false;
			this.Manager.AllowClose = true;
			this.Manager.Close();
		}
		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified stored procedure
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		public virtual DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified stored procedure using specified transaction
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		public virtual DataSet ExecuteDataSet(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified command
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>a new DataSet object</returns>
		public virtual DataSet ExecuteDataSet(DbCommand command)
		{
			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			using (DbDataAdapter dataAdapter = this.CreateAdapter(command))
			{
				DataSet ds = new DataSet();
				ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
				dataAdapter.Fill(ds);
				this.Manager.Close();
				return ds;
			}
		}
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteDataSet run on each command text</returns>
		public virtual DataSet[] ExecuteDataSet(params string[] commandTexts)
		{
			if (commandTexts.Length == 0) return null;

			DbTransaction transaction = this.OpenConnection().BeginTransaction();
			List<DataSet> list = new List<DataSet>();
			if (commandTexts == null || commandTexts.Length == 0) return list.ToArray();
			foreach (string commandText in commandTexts)
			{
				if (StringUtil.IsNullOrWhiteSpace(commandText)) throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			}

			try
			{
				foreach (string commandText in commandTexts)
				{
					list.Add(this.ExecuteDataSet(CommandType.Text, transaction, commandText));
				}
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				list.Clear();
				throw;
			}
			this.CloseConnection();
			return list.ToArray();
		}
		/// <summary>
		/// Executes the specified command text and returns the number of rows affected.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		public virtual int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the specified command text using specified transaction and returns the number of rows affected.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		public virtual int ExecuteNonQuery(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		/// <summary>
		/// Executes the specified command text after applying specified parameter values
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination thereof)</param>
		/// <returns>The number of rows affected.</returns>
		public virtual int ExecuteNonQuery(string commandText, params object[] parameterValues)
		{
			CommandType type = IsCrud(commandText) ? CommandType.Text : CommandType.StoredProcedure;
			return this.ExecuteNonQuery(type, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the command and returns the number of rows affected.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The number of rows affected.</returns>
		public virtual int ExecuteNonQuery(DbCommand command)
		{
			try
			{
				command.Connection = this.Manager.Connection;
				this.Manager.Open();
				int result = command.ExecuteNonQuery();
				this.Manager.Close();
				return result;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw;
			}
		}
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteNonQuery run on each command text</returns>
		public virtual int[] ExecuteNonQuery(params string[] commandTexts)
		{
			DbTransaction transaction = this.OpenConnection().BeginTransaction();
			List<int> list = new List<int>();
			if (commandTexts == null || commandTexts.Length == 0) return list.ToArray();
			foreach (string commandText in commandTexts)
			{
				if (StringUtil.IsNullOrWhiteSpace(commandText)) throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			}

			try
			{
				foreach (string commandText in commandTexts)
				{
					list.Add(this.ExecuteNonQuery(CommandType.Text, transaction, commandText));
				}
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				list.Clear();
				throw;
			}
			this.CloseConnection();
			return list.ToArray();
		}
		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the specified command text using the specified transaction and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(string commandText, params object[] parameterValues)
		{
			CommandType type = IsCrud(commandText) ? CommandType.Text : CommandType.StoredProcedure;
			return this.ExecuteReader(type, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the command and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(DbCommand command)
		{
			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			IDataReader result = command.ExecuteReader(this.Manager.AllowClose ? CommandBehavior.CloseConnection : CommandBehavior.Default);
			return result;
		}
		/// <summary>
		/// Executes the command returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public virtual object ExecuteScalar(DbCommand command)
		{
			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			object result = command.ExecuteScalar();
			this.Manager.Close();
			return result;
		}
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteScalar run on each command text</returns>
		public virtual object[] ExecuteScalar(params string[] commandTexts)
		{
			DbTransaction transaction = this.OpenConnection().BeginTransaction();
			List<object> list = new List<object>();
			if (commandTexts == null || commandTexts.Length == 0) return list.ToArray();
			foreach (string commandText in commandTexts)
			{
				if (StringUtil.IsNullOrWhiteSpace(commandText)) throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			}

			try
			{
				foreach (string commandText in commandTexts)
				{
					list.Add(this.ExecuteScalar(CommandType.Text, transaction, commandText));
				}
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				list.Clear();
				throw;
			}
			this.CloseConnection();
			return list.ToArray();
		}
		/// <summary>
		/// Executes the specified command text returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public virtual object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes the specified command text using specified transaction returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public virtual object ExecuteScalar(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		/// <summary>
		/// Uses CreateConnection() to get a new connection, and until CloseConnection is closed, uses that connection object
		/// </summary>
		/// <returns>The IDbConnection connection that will be used until CloseConnection is called.</returns>
		/// <exception cref="InvalidOperationException">if the method has been called already.</exception>
		public virtual DbConnection OpenConnection()
		{
			if (!this.Manager.AllowClose) throw new InvalidOperationException("OpenConnection has already been closed, call CloseConnection first");
			this.Manager.AllowClose = false;
			this.Manager.Open();
			return this.Manager.Connection;
		}

		/// <summary>
		/// Creates a new data adapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="IDbDataAdapter"/> adapter</returns>
		public abstract DbDataAdapter CreateAdapter(DbCommand command);
		/// <summary>
		/// Creates a new command object for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		public abstract DbCommand CreateCommand();
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="IDbConnection"/> object</returns>
		public abstract DbConnection CreateConnection();
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <returns>New <see cref="IDataParameter"/> object</returns>
		public abstract DbParameter CreateParameter();
		/// <summary>
		/// Runs a system check for the existence of specified column in the specified table
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <param name="columnName">the column name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public abstract bool CheckColumnExists(string tableName, string columnName);
		/// <summary>
		/// Runs a system check for the existence of specified table
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public abstract bool CheckTableExists(string tableName);

		/// <summary>
		/// Creates a new command object of specified type
		/// </summary>
		/// <param name="type">The type of command</param>
		/// <param name="transaction">The transaction to use</param>
		/// <param name="commandText">The command text</param>
		/// <param name="parameterValues"></param>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		protected DbCommand CreateCommand(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (StringUtil.IsNullOrWhiteSpace(commandText)) throw new ArgumentNullException("commandText");
			if (!parameterValues.IsEmpty() && parameterValues.Any(p => p == null)) throw new ArgumentNullException("parameterValues");

			DbCommand command = this.CreateCommand();
			command.CommandText = commandText;
			command.CommandType = type;
			if (transaction != null) command.Transaction = transaction;
			if (!parameterValues.IsEmpty())
			{
				if (commandText.IndexOf("{0}", StringComparison.CurrentCulture) > 0) command.CommandText = StringUtil.Format(commandText, parameterValues);
				else
				{
					foreach (object obj in parameterValues)
					{
						var parameter = obj as Parameter;
						IDataParameter dbParameter = obj as IDataParameter;
						if (parameter != null)
						{
							dbParameter = this.CreateParameter();
							dbParameter.ParameterName = parameter.Name;
							dbParameter.Value = parameter.Value;
						}
						if (dbParameter != null) command.Parameters.Add(dbParameter);
					}
				}
			}
			return command;
		}
		/// <summary>
		/// Disposes the helper class
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (_connection != null) _connection.Dispose();
		}

		/// <summary>
		/// Gets if the specified command text is a CRUD command
		/// </summary>
		/// <param name="commandText">The text to check</param>
		/// <returns></returns>
		protected static bool IsCrud(string commandText)
		{
			return crudites.Any(s => commandText.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1);
		}

		#endregion
	}
}