using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

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
		private bool _useTransactionAlways = true;
		private int _commandTimeout = 30;
		#endregion
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using <see cref="Configuration.ConnectionStrings.Default"/>
		/// </summary>
		protected SqlHelperBase() : this(Configuration.ConnectionStrings.Default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using specified connection string setting object
		/// </summary>
		/// <param name="connectionStringSettings">The connection string settings object to use</param>
		/// <exception cref="ArgumentNullException">if specified connection string null</exception>
		/// <exception cref="ArgumentException">if specified connection string object has an invalid connection string</exception>
		protected SqlHelperBase(ConnectionStringSettings connectionStringSettings)
		{
			if (connectionStringSettings == null)
			{
				throw new ArgumentNullException("connectionStringSettings");
			}

			this.ConnectionString = connectionStringSettings.ConnectionString;
			this.UseTransactionAlways = true;
		}

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using specified connection string setting object
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use</param>
		/// <exception cref="ArgumentException">if specified connection string is invalid</exception>
		protected SqlHelperBase(string keyOrConnectionString)
		{
			if (StringUtil.IsNullOrWhiteSpace(keyOrConnectionString))
			{
				throw new ArgumentNullException("keyOrConnectionString");
			}
			
			var cs = ConfigurationManager.ConnectionStrings[keyOrConnectionString];
			this.ConnectionString = cs == null ? keyOrConnectionString : cs.ConnectionString;
			this.UseTransactionAlways = true;
		}
		
		#endregion
		#region Properties
		
		/// <summary>
		/// Gets or sets the wait time (in seconds) before terminating the attempt to execute a command and generating an error (default 30).
		/// </summary>
		/// <exception cref="ArgumentException">The property value assigned is less than 0.</exception>
		public int CommandTimeout
		{
			get 
			{ 
				return _commandTimeout; 
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The value must be greater than 0.", "value");
				}

				_commandTimeout = value;
			}
		}
		
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

		/// <summary>
		/// Gets or sets a value indicating whether to use transactions if a method that doesn't take a <see cref="IDbTransaction"/> object is called, defaults to true.
		/// </summary>
		public bool UseTransactionAlways
		{
			get { return _useTransactionAlways; }
			set { _useTransactionAlways = value; }
		}

		/// <summary>
		/// Gets the connection manager
		/// </summary>
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
		/// Runs a system check for the existence of specified column in the specified table using TSQL INFORMATION_SCHEMA.
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <param name="columnName">the column name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="tableName"/> is null or empty, OR, <paramref name="columnName"/> is null or empty</exception>
		public virtual bool ColumnExists(string tableName, string columnName)
		{
			if (StringUtil.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException("tableName");
			}

			if (StringUtil.IsNullOrWhiteSpace(columnName))
			{
				throw new ArgumentNullException("columnName");
			}

			using (IDataReader r = this.ExecuteReader(CommandType.Text, String.Format("SELECT * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'", tableName, columnName)))
			{
				return r.Read();
			}
		}

		/// <summary>
		/// Closes the connection opened by OpenConnection, then, lets CreateConnection know to always create new connections henceforth.
		/// </summary>
		/// <exception cref="InvalidOperationException">if the method was called out of sequence, i.e., OpenConnection was never called, or called once and CloseConnection called multiple times.</exception>
		public virtual void CloseConnection()
		{
			this.Manager.AllowClose = true;
			this.Manager.Close();
		}

		/// <summary>
		/// Creates a new database using information from the connection string.
		/// </summary>
		public virtual void CreateDatabase()
		{
			this.ExecuteNonQuery(CommandType.Text, StringUtil.Format("CREATE DATABASE [{0}]", this.Manager.Connection.Database));
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		/// <returns>New <see cref="IDataParameter"/> object</returns>
		public IDataParameter CreateParameter(string name, object value)
		{
			var parameter = this.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
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
		/// Drops the database using information from the connection string.
		/// </summary>
		public virtual void DropDatabase()
		{
			this.ExecuteNonQuery(CommandType.Text, StringUtil.Format("DROP DATABASE [{0}]", this.Manager.Connection.Database));
		}

		/// <summary>
		/// Uses CreateConnection() to get a new connection, and until CloseConnection is closed, uses that connection object
		/// </summary>
		/// <returns>The IDbConnection connection that will be used until CloseConnection is called.</returns>
		/// <exception cref="InvalidOperationException">if the method has been called already.</exception>
		public virtual IDbConnection OpenConnection()
		{
			if (!this.Manager.AllowClose)
			{
				throw new InvalidOperationException("OpenConnection has already been closed, call CloseConnection first");
			}

			this.Manager.AllowClose = false;
			this.Manager.Open();
			return this.Manager.Connection;
		}

		/// <summary>
		/// Runs a query to determine if the specified table exists using TSQL INFORMATION_SCHEMA.
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public virtual bool TableExists(string tableName)
		{
			if (StringUtil.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException("tableName");
			}

			using (IDataReader r = this.ExecuteReader(CommandType.Text, String.Format("SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", tableName)))
			{
				return r.Read();
			}
		}

		#region Execute Methods

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
		public virtual DataSet ExecuteDataSet(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		
		/// <summary>
		/// Executes and returns a new DataSet from specified command
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>a new DataSet object</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="command"/> is null</exception>
		public virtual DataSet ExecuteDataSet(IDbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			command.Connection = this.Manager.Connection;
			command.CommandTimeout = Math.Max(command.CommandTimeout, this.CommandTimeout);
			this.Manager.Open();
			
			IDbDataAdapter dataAdapter = this.CreateAdapter(command);
			DataSet dataset = new DataSet();
			dataset.Locale = System.Globalization.CultureInfo.CurrentCulture;
			dataAdapter.Fill(dataset);
			
			this.Manager.Close();
			
			IDisposable d = dataAdapter as IDisposable;
			if (d != null)
			{
				d.Dispose();
			}
			
			return dataset;
		}
		
		/// <summary>
		/// Executes the commands in a batched mode with a transaction
		/// </summary>
		/// <param name="commands">1 or more commands</param>
		/// <returns>the result of each ExecuteDataSet run on each command</returns>
		/// <exception cref="ArgumentException">if <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commands"/> is null</exception>
		public virtual DataSet[] ExecuteDataSet(params IDbCommand[] commands)
		{
			if (commands.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commands");
			}

			if (commands.Any(c => c == null))
			{
				throw new ArgumentNullException("commands", Exceptions.ArgumentInListNull);
			}

			List<DataSet> list = new List<DataSet>();
			IDbTransaction transaction = this.UseTransactionAlways ? this.OpenConnection().BeginTransaction() : null;
			try
			{
				foreach (IDbCommand command in commands)
				{
					if (transaction != null)
					{
						command.Transaction = transaction;
					}

					list.Add(this.ExecuteDataSet(command));
				}

				if (transaction != null)
				{
					transaction.Commit();
				}
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}

				throw;
			}
			finally
			{
				this.CloseConnection();
			}

			return list.ToArray();
		}
		
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteDataSet run on each command text</returns>
		/// <exception cref="ArgumentException">if <paramref name="commandTexts"/> is empty</exception>
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commandTexts"/> is null</exception>
		public virtual DataSet[] ExecuteDataSet(params string[] commandTexts)
		{
			if (commandTexts.Any(t => String.IsNullOrEmpty(t)))
			{
				throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			}

			if (commandTexts.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commandTexts");
			}

			List<DataSet> list = new List<DataSet>();
			IDbTransaction transaction = this.UseTransactionAlways ? this.OpenConnection().BeginTransaction() : null;
			try
			{
				foreach (string commandText in commandTexts)
				{
					list.Add(this.ExecuteDataSet(CommandType.Text, transaction, commandText));
				}

				if (transaction != null)
				{
					transaction.Commit();
				}
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}

				throw;
			}
			finally
			{
				this.CloseConnection();
			}

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
		public virtual int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		
		/// <summary>
		/// Executes the command and returns the number of rows affected.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="command"/> is null</exception>
		public virtual int ExecuteNonQuery(IDbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			command.Connection = this.Manager.Connection;
			command.CommandTimeout = Math.Max(command.CommandTimeout, this.CommandTimeout);
			this.Manager.Open();
			int result = command.ExecuteNonQuery();
			this.Manager.Close();
			return result;
		}
		
		/// <summary>
		/// Executes the commands in a batched mode with a transaction
		/// </summary>
		/// <param name="commands">1 or more commands</param>
		/// <returns>the result of each ExecuteNonQuery run on each command text</returns>
		/// <exception cref="ArgumentException">if <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commands"/> is null</exception>
		public virtual int[] ExecuteNonQuery(params IDbCommand[] commands)
		{
			if (commands.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commands");
			}

			if (commands.Any(c => c == null))
			{
				throw new ArgumentNullException("commands", Exceptions.ArgumentInListNull);
			}

			List<int> list = new List<int>();
			IDbTransaction transaction = this.UseTransactionAlways ? this.OpenConnection().BeginTransaction() : null;
			try
			{
				foreach (IDbCommand command in commands)
				{
					if (transaction != null)
					{
						command.Transaction = transaction;
					}

					list.Add(this.ExecuteNonQuery(command));
				}

				if (transaction != null)
				{
					transaction.Commit();
				}
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}

				list.Clear();
				throw;
			}
			finally
			{
				this.CloseConnection();
			}

			return list.ToArray();
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
			if (commandTexts.Any(t => String.IsNullOrEmpty(t)))
			{
				throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			}

			if (commandTexts.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commandTexts");
			}

			List<int> list = new List<int>();
			IDbTransaction transaction = this.UseTransactionAlways ? this.OpenConnection().BeginTransaction() : null;
			try
			{
				foreach (string commandText in commandTexts)
				{
					list.Add(this.ExecuteNonQuery(CommandType.Text, transaction, commandText));
				}

				if (transaction != null)
				{
					transaction.Commit();
				}
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}

				throw;
			}
			finally
			{
				this.CloseConnection();
			}

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
		public virtual IDataReader ExecuteReader(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(this.CreateCommand(type, transaction, commandText, parameterValues));
		}
		
		/// <summary>
		/// Executes the command and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(IDbCommand command)
		{
			command.Connection = this.Manager.Connection;
			command.CommandTimeout = Math.Max(command.CommandTimeout, this.CommandTimeout);
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
		public virtual object ExecuteScalar(IDbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			command.Connection = this.Manager.Connection;
			command.CommandTimeout = Math.Max(command.CommandTimeout, this.CommandTimeout);
			this.Manager.Open();
			object result = command.ExecuteScalar();
			this.Manager.Close();
			return result;
		}
		
		/// <summary>
		/// Executes the commands in a batched mode with a transaction
		/// </summary>
		/// <param name="commands">1 or more commands</param>
		/// <returns>the result of each ExecuteScalar run on each command text</returns>
		/// <exception cref="ArgumentException">if <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">if any of the items in <paramref name="commands"/> is null</exception>
		public virtual object[] ExecuteScalar(params IDbCommand[] commands)
		{
			if (commands.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commands");
			}

			if (commands.Any(c => c == null))
			{
				throw new ArgumentNullException("commands", Exceptions.ArgumentInListNull);
			}

			List<object> list = new List<object>();
			IDbTransaction transaction = this.UseTransactionAlways ? this.OpenConnection().BeginTransaction() : null;
			try
			{
				foreach (IDbCommand command in commands)
				{
					if (transaction != null)
					{
						command.Transaction = transaction;
					}

					list.Add(this.ExecuteScalar(command));
				}

				if (transaction != null)
				{
					transaction.Commit();
				}
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}

				throw;
			}
			finally
			{
				this.CloseConnection();
			}

			return list.ToArray();
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
			if (commandTexts.Any(t => String.IsNullOrEmpty(t)))
			{
				throw new ArgumentNullException("commandTexts", Exceptions.ArgumentInListNull);
			}

			if (commandTexts.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commandTexts");
			}

			List<object> list = new List<object>();
			IDbTransaction transaction = this.UseTransactionAlways ? this.OpenConnection().BeginTransaction() : null;
			try
			{
				foreach (string commandText in commandTexts)
				{
					list.Add(this.ExecuteScalar(CommandType.Text, transaction, commandText));
				}

				if (transaction != null)
				{
					transaction.Commit();
				}
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}

				throw;
			}
			finally
			{
				this.CloseConnection();
			}

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
		public virtual object ExecuteScalar(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(this.CreateCommand(type, transaction, commandText, parameterValues));
		}

		#endregion
		#region Abstract Methods

		/// <summary>
		/// Creates a new data adapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter</param>
		/// <returns>New <see cref="IDbDataAdapter"/> adapter</returns>
		public abstract IDbDataAdapter CreateAdapter(IDbCommand command);
		
		/// <summary>
		/// Creates a new command object for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		public abstract IDbCommand CreateCommand();
		
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New <see cref="IDbConnection"/> object</returns>
		public abstract IDbConnection CreateConnection();
		
		/// <summary>
		/// Creates a new data parametr for use in running commands
		/// </summary>
		/// <returns>New <see cref="IDataParameter"/> object</returns>
		public abstract IDbDataParameter CreateParameter();
		
		/// <summary>
		/// Runs a check for the existence of database specified in the connectionstring
		/// </summary>
		/// <returns>True if the database exists, false otherwise</returns>
		public abstract bool DatabaseExists();

		#endregion

		/// <summary>
		/// Creates a new command object of specified type using specified <paramref name="parameterValues"/>
		/// </summary>
		/// <param name="type">The type of command</param>
		/// <param name="transaction">The transaction to use</param>
		/// <param name="commandText">The command text</param>
		/// <param name="parameterValues">The parameter values, which can be an array of <see cref="Parameter"/>, <see cref="IDataParameter"/>.</param>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		/// <exception cref="ArgumentNullException">if <paramref name="commandText"/> is null or empty, OR, parameterValues is not empty but any item in it is null</exception>
		protected IDbCommand CreateCommand(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (StringUtil.IsNullOrWhiteSpace(commandText))
			{
				throw new ArgumentNullException("commandText");
			}

			if (!parameterValues.IsNullOrEmpty() && parameterValues.Any(p => p == null))
			{
				throw new ArgumentNullException("parameterValues");
			}

			List<IDataParameter> parameters = new List<IDataParameter>();
			if (parameterValues.Any(p => p is Parameter) || parameterValues.Any(p => p is IDataParameter))
			{
				foreach (var item in parameterValues)
				{
					IDataParameter dataParameter = item as IDataParameter;
					var parameter = item as Parameter;
					if (parameter != null)
					{
						dataParameter = this.CreateParameter(parameter.Name, parameter.Value);
					}

					if (dataParameter != null)
					{
						parameters.Add(dataParameter);
					}
				}
			}
			else if (parameterValues.All(p => p is KeyValuePair<string, object>))
			{
				foreach (var item in parameterValues)
				{
					var kv = (KeyValuePair<string, object>)item;
					IDataParameter dataParameter = this.CreateParameter(kv.Key, kv.Value);
					parameters.Add(dataParameter);
				}
			}
			else if (parameterValues.Length == 1 && parameterValues[0] is IDictionary)
			{
				IDictionary dic = parameterValues[0] as IDictionary;
				foreach (var k in dic.Keys)
				{
					IDataParameter dataParameter = this.CreateParameter(k.ToString(), dic[k]);
					parameters.Add(dataParameter);
				}
			}
			else if (commandText.Contains("{0}"))
			{
				if (parameterValues.Any(p => p is Parameter) || !parameterValues.Any(p => p is IDataParameter))
				{
					throw new ArgumentException(Exceptions.UnableToParseCommandText);
				}
				else
				{
					// Cheap way of throwing a FormatException, if the commandText is invalid.-or- The index of a parameterValues item is less than zero, or greater than or equal to the length of the args array.
					String.Format(commandText, parameterValues);
					
					for (var i = 0; i < parameterValues.Length; i++)
					{
						string name = "@param" + i;
						commandText = commandText.Replace("{" + i + "}", name);
						var parameter = this.CreateParameter();
						parameter.ParameterName = name;
						parameter.Value = parameterValues[i];
					}
				}
			}
			else if (commandText.Contains("@"))
			{
				var splits = commandText.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
				if ((splits.Length - 1) == parameterValues.Length)
				{
					for (var i = 0; i < parameterValues.Length; i++)
					{
						IDataParameter dataParameter = this.CreateParameter();
						dataParameter.ParameterName = "@" + splits[i + 1].Split(' ')[0];
						dataParameter.Value = parameterValues[i];
						parameters.Add(dataParameter);
					}
				}
			}
			else if (type == CommandType.StoredProcedure)
			{
				for (var i = 0; i < parameterValues.Length; i++)
				{
					IDataParameter dataParameter = this.CreateParameter();
					dataParameter.ParameterName = "@param" + i;
					dataParameter.Value = parameterValues[i];
					parameters.Add(dataParameter);
				}
			}

			if (parameterValues.Length != parameters.Count)
			{
				throw new ArgumentException(Exceptions.UnableToParseCommandText);
			}

			IDbCommand command = this.CreateCommand();
			command.CommandText = commandText;
			command.CommandType = type;
			if (transaction != null)
			{
				command.Transaction = transaction;
			}

			foreach (var p in parameters)
			{
				command.Parameters.Add(p);
			}

			return command;
		}

		/// <summary>
		/// Disposes the helper class
		/// </summary>
		/// <param name="disposing">If the object is being disposed or not.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (_connectionManager != null)
			{
				_connectionManager.Dispose();
			}
		}

		#endregion
	}
}