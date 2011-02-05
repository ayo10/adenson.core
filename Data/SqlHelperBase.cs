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
		private bool mustCloseConnection = true;
		private ConnectionManager _connectionManager;
		private string _connectionString;
		#endregion
		#region Constructors

		/// <summary>
		/// Instantiates a new helper using <see cref="Configuration.ConnectionStrings.Default"/>
		/// </summary>
		protected SqlHelperBase() : this(Configuration.ConnectionStrings.Default)
		{
		}
		/// <summary>
		/// Instantiates a new instance of the sql helper using specified connection string setting object
		/// </summary>
		/// <param name="connectionString">The connection string settings object to use</param>
		/// <exception cref="ArgumentNullException">if specified connection string null</exception>
		/// <exception cref="ArgumentException">if specified connection string object has an invalid connection string</exception>
		protected SqlHelperBase(ConnectionStringSettings connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException("connectionString");
			this.ConnectionString = connectionString.ConnectionString;
		}
		/// <summary>
		/// Instantiates a new instance of the sql helper using specified connection string setting object
		/// </summary>
		/// <param name="connectionString">The connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		protected SqlHelperBase(string connectionString)
		{
			if (StringUtil.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
			this.ConnectionString = connectionString;
		}
		
		#endregion
		#region Properties
		
		/// <summary>
		/// Gets the connection string to use
		/// </summary>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		public string ConnectionString
		{
			get { return _connectionString; }
			private set
			{
				DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
				builder.ConnectionString = value;
				_connectionString = value;
			}
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
				if (_connectionManager == null || _connectionManager.Connection == null)
				{
					_connectionManager = new ConnectionManager(this.CreateConnection());
					_connectionManager.AllowClose = mustCloseConnection;
				}
				return _connectionManager;
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
		/// Executes and returns a new DataSet from specified command text
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(type, null, commandText, parameterValues);
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified command text using specified transaction
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual DataSet ExecuteDataSet(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified command
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>a new DataSet object</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="command"/> is null</exception>
		public virtual DataSet ExecuteDataSet(DbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

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
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commandTexts"/> is null</exception>
		/// <exception cref="ArgumentException">if <paramref name="commandTexts"/> is empty</exception>
		public virtual DataSet[] ExecuteDataSet(params string[] commandTexts)
		{
			if (commandTexts.Any(t => String.IsNullOrEmpty(t))) throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			if (commandTexts.Length == 0) throw new ArgumentException(Exceptions.ArgumentsEmpty, "commandTexts");

			List<DataSet> list = new List<DataSet>();
			DbTransaction transaction = this.OpenConnection().BeginTransaction();
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
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
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
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual int ExecuteNonQuery(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		/// <summary>
		/// Executes the command and returns the number of rows affected.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="command"/> is null</exception>
		public virtual int ExecuteNonQuery(DbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			int result = command.ExecuteNonQuery();
			this.Manager.Close();
			return result;
		}
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteNonQuery run on each command text</returns>
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commandTexts"/> is null</exception>
		/// <exception cref="ArgumentException">if <paramref name="commandTexts"/> is empty</exception>
		public virtual int[] ExecuteNonQuery(params string[] commandTexts)
		{
			if (commandTexts.Any(t => String.IsNullOrEmpty(t))) throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			if (commandTexts.Length == 0) throw new ArgumentException(Exceptions.ArgumentsEmpty, "commandTexts");

			List<int> list = new List<int>();
			DbTransaction transaction = this.OpenConnection().BeginTransaction();
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
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual IDataReader ExecuteReader(CommandType type, DbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(this.CreateCommand(type, transaction, commandText, parameterValues));
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
		/// <exception cref="ArgumentNullException">if <paramref name="command"/> is null</exception>
		public virtual object ExecuteScalar(DbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

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
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commandTexts"/> is null</exception>
		/// <exception cref="ArgumentException">if <paramref name="commandTexts"/> is empty</exception>
		public virtual object[] ExecuteScalar(params string[] commandTexts)
		{
			if (commandTexts.Any(t => String.IsNullOrEmpty(t))) throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			if (commandTexts.Length == 0) throw new ArgumentException(Exceptions.ArgumentsEmpty, "commandTexts");

			List<object> list = new List<object>();
			DbTransaction transaction = this.OpenConnection().BeginTransaction();
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
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
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
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
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
		/// <exception cref="ArgumentNullException">if <paramref name="tableName"/> is null or empty, OR, <paramref name="columnName"/> is null or empty</exception>
		public abstract bool CheckColumnExists(string tableName, string columnName);
		/// <summary>
		/// Runs a system check for the existence of specified table
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="tableName"/> is null or empty</exception>
		public abstract bool CheckTableExists(string tableName);

		/// <summary>
		/// Creates a new command object of specified type
		/// </summary>
		/// <param name="type">The type of command</param>
		/// <param name="transaction">The transaction to use</param>
		/// <param name="commandText">The command text</param>
		/// <param name="parameterValues"></param>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, parameterValues is not empty but any item in it is null</exception>
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
			if (_connectionManager != null) _connectionManager.Dispose();
		}

		#endregion
	}
}