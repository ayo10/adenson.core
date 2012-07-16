using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace Adenson.Data
{
	/// <summary>
	/// Sql Helper base class
	/// </summary>
	public abstract class SqlHelperBase : IDisposable
	{
		#region Variables
		private IDbConnection _connection;
		private int _commandTimeout = 30;
		private Stack<IDbTransaction> transactions = new Stack<IDbTransaction>();
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
		/// <exception cref="ArgumentNullException">If specified connection string null</exception>
		/// <exception cref="ArgumentException">If specified connection string object has an invalid connection string</exception>
		protected SqlHelperBase(ConnectionStringSettings connectionStringSettings)
		{
			if (connectionStringSettings == null)
			{
				throw new ArgumentNullException("connectionStringSettings");
			}

			this.ConnectionString = connectionStringSettings.ConnectionString;
		}

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using specified connection string setting object
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		protected SqlHelperBase(string keyOrConnectionString)
		{
			if (StringUtil.IsNullOrWhiteSpace(keyOrConnectionString))
			{
				throw new ArgumentNullException("keyOrConnectionString");
			}

			var cs = ConfigurationManager.ConnectionStrings[keyOrConnectionString];
			this.ConnectionString = cs == null ? keyOrConnectionString : cs.ConnectionString;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the wait time (in seconds) before terminating the attempt to execute a command and generating an error (default 30).
		/// </summary>
		/// <exception cref="ArgumentException">The property value assigned is less than 0.</exception>
		public virtual int CommandTimeout
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
		public virtual string ConnectionString
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current connection object in use (if OpenConnection was invoked)
		/// </summary>
		public virtual IDbConnection Connection
		{
			get 
			{
				if (_connection == null)
				{
					_connection = this.CreateConnection();
				}

				return _connection; 
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Runs a system check for the existence of specified column in the specified table using TSQL INFORMATION_SCHEMA.
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <param name="columnName">The column name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="tableName"/> is null or empty, OR, <paramref name="columnName"/> is null or empty</exception>
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
		/// Creates a new database using information from the connection string.
		/// </summary>
		public virtual void CreateDatabase()
		{
			this.ExecuteNonQuery(CommandType.Text, StringUtil.Format("CREATE DATABASE [{0}]", this.Connection.Database));
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
		/// <exception cref="InvalidOperationException">If there are any transactions left in the stack.</exception>
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
			this.ExecuteNonQuery(CommandType.Text, StringUtil.Format("DROP DATABASE [{0}]", this.Connection.Database));
		}

		/// <summary>
		/// Starts a new transaction, and pushes it to the top of the transactions stack, causing every Execute* method invoked after to run with the topmost transaction <see cref="EndTransaction"/> is called.
		/// </summary>
		/// <returns>The current number of open transactions in the stack.</returns>
		public virtual int BeginTransaction()
		{
			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}

			transactions.Push(this.Connection.BeginTransaction());
			return transactions.Count;
		}

		/// <summary>
		/// Pops out the last (topmost) transaction from the stack and invokes Commit on it.
		/// </summary>
		/// <returns>The current number of open transactions.</returns>
		/// <exception cref="InvalidOperationException">If there are no transactions in the stack.</exception>
		public virtual int EndTransaction()
		{
			if (transactions.Count == 0)
			{
				throw new InvalidOperationException(Exceptions.NoOpenTransactions);
			}

			IDbTransaction transaction = transactions.Pop();
			transaction.Commit();
			return transactions.Count;
		}

		/// <summary>
		/// Runs a query to determine if the specified table exists using TSQL INFORMATION_SCHEMA.
		/// </summary>
		/// <param name="tableName">The table name</param>
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

		#region ExecuteDataSet

		/// <summary>
		/// Executes and returns a new DataSet from specified command text using specified transaction
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>A new DataSet object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="transaction"/> is null</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual DataSet ExecuteDataSet(string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes and returns a new DataSet from specified command text
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>A new DataSet object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(type, commandText, parameterValues));
		}

		/// <summary>
		/// Executes and returns a new DataSet from specified command
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>A new DataSet object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="command"/> is null</exception>
		public virtual DataSet ExecuteDataSet(IDbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			this.PrepCommand(command);

			IDbDataAdapter dataAdapter = this.CreateAdapter(command);
			DataSet dataset = new DataSet();
			dataset.Locale = System.Globalization.CultureInfo.CurrentCulture;
			dataAdapter.Fill(dataset);

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
		/// <returns>The result of each ExecuteDataSet run on each command</returns>
		/// <exception cref="ArgumentException">If <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commands"/> is null</exception>
		public virtual DataSet[] ExecuteDataSet(params IDbCommand[] commands)
		{
			return this.Execute<DataSet>(ExecuteType.Dataset, commands);
		}

		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>The result of each ExecuteDataSet run on each command text</returns>
		/// <exception cref="ArgumentException">If <paramref name="commandTexts"/> is empty</exception>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commandTexts"/> is null</exception>
		public virtual DataSet[] ExecuteDataSet(params string[] commandTexts)
		{
			return this.Execute<DataSet>(ExecuteType.Dataset, commandTexts);
		}

		#endregion
		#if !NET35
		#region ExecuteDynamic

		/// <summary>
		/// Executes the specified command text, runs as a System.Data.IDataReader, reads the result into a dynamic object.
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataDynamic object.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual IEnumerable<dynamic> ExecuteDynamic(string commandText, params object[] parameterValues)
		{
			return this.ExecuteDynamic(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataDynamic.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataDynamic object.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual IEnumerable<dynamic> ExecuteDynamic(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDynamic(this.CreateCommand(type, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the command and builds an System.Data.IDataDynamic.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>An System.Data.IDataDynamic object.</returns>
		public virtual IEnumerable<dynamic> ExecuteDynamic(IDbCommand command)
		{
			this.PrepCommand(command);
			List<dynamic> result = new List<dynamic>();
			using (IDataReader r = command.ExecuteReader())
			{
				dynamic d = new ExpandoObject();
				while (r.Read())
				{
					for (var i = 0; i < r.FieldCount; i++)
					{
						d[r.GetName(i)] = r.GetValue(i);
					}

					result.Add(d);
				}
			}

			return result;
		}

		#endregion
		#endif
		#region ExecuteNonQuery

		/// <summary>
		/// Executes the specified command text and returns the number of rows affected.
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual int ExecuteNonQuery(string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the specified command text and returns the number of rows affected.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteNonQuery(this.CreateCommand(type, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the command and returns the number of rows affected.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="command"/> is null</exception>
		public virtual int ExecuteNonQuery(IDbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			this.PrepCommand(command);
			return command.ExecuteNonQuery();
		}

		/// <summary>
		/// Executes the commands in a batched mode with a transaction
		/// </summary>
		/// <param name="commands">1 or more commands</param>
		/// <returns>The result of each ExecuteNonQuery run on each command text</returns>
		/// <exception cref="ArgumentException">If <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commands"/> is null</exception>
		public virtual int[] ExecuteNonQuery(params IDbCommand[] commands)
		{
			return this.Execute<int>(ExecuteType.NonQuery, commands);
		}

		/// <summary>
		/// Executes the command texts in a batched mode with a transaction.
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>The result of each ExecuteNonQuery run on each command text</returns>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commandTexts"/> is null</exception>
		/// <exception cref="ArgumentException">If <paramref name="commandTexts"/> is empty</exception>
		public virtual int[] ExecuteNonQuery(params string[] commandTexts)
		{
			return this.Execute<int>(ExecuteType.NonQuery, commandTexts);
		}

		/// <summary>
		/// Reads the specified stream split into strings (delimiting using <see cref="Environment.NewLine"/>), and runs them in batched mode.
		/// </summary>
		/// <param name="stream">The stream containing the commmand text to run.</param>
		/// <returns>The result of each ExecuteNonQuery run on each command text.</returns>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commandTexts"/> is null.</exception>
		public virtual int[] ExecuteNonQuery(StreamReader stream)
		{
			return this.ExecuteNonQuery(stream, Environment.NewLine);
		}

		/// <summary>
		/// Reads the specified stream split into strings (using specified delimiter), and runs them in batched mode.
		/// </summary>
		/// <param name="stream">The stream containing the commmand text to run.</param>
		/// <param name="delimiter">The delimiter to use.</param>
		/// <returns>The result of each ExecuteNonQuery run on each command text.</returns>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commandTexts"/> is null.</exception>
		public virtual int[] ExecuteNonQuery(StreamReader stream, string delimiter)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			string[] scripts = stream.ReadToEnd().Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
			return this.ExecuteNonQuery(scripts);
		}

		#endregion
		#region ExecuteReader

		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual IDataReader ExecuteReader(string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteReader(this.CreateCommand(type, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the command and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(IDbCommand command)
		{
			this.PrepCommand(command);
			return command.ExecuteReader();
		}

		#endregion
		#region ExecuteScalar

		/// <summary>
		/// Executes the specified command text returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual object ExecuteScalar(string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the specified command text using specified transaction returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="transaction">The transaction to use</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="transaction"/> is null</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual object ExecuteScalar(IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}

			return this.ExecuteScalar(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the specified command text returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="type">The command type</param>
		/// <param name="commandText">The command to execute</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteScalar(this.CreateCommand(type, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the command returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="command"/> is null</exception>
		public virtual object ExecuteScalar(IDbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			this.PrepCommand(command);
			return command.ExecuteScalar();
		}

		/// <summary>
		/// Executes the commands in a batched mode with a transaction
		/// </summary>
		/// <param name="commands">1 or more commands</param>
		/// <returns>The result of each ExecuteScalar run on each command text</returns>
		/// <exception cref="ArgumentException">If <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commands"/> is null</exception>
		public virtual object[] ExecuteScalar(params IDbCommand[] commands)
		{
			return this.Execute<object>(ExecuteType.Scalar, commands);
		}

		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>The result of each ExecuteScalar run on each command text</returns>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commandTexts"/> is null</exception>
		/// <exception cref="ArgumentException">If <paramref name="commandTexts"/> is empty</exception>
		public virtual object[] ExecuteScalar(params string[] commandTexts)
		{
			return this.Execute<object>(ExecuteType.Scalar, commandTexts);
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
		/// <param name="commandText">The command text</param>
		/// <param name="parameterValues">The parameter values, which can be an array of <see cref="Parameter"/>, <see cref="IDataParameter"/>.</param>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty, OR, parameterValues is not empty but any item in it is null</exception>
		protected virtual IDbCommand CreateCommand(CommandType type, string commandText, params object[] parameterValues)
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

			foreach (var p in parameters)
			{
				command.Parameters.Add(p);
			}

			if (transactions.Count > 0)
			{
				command.Transaction = transactions.Peek();
			}

			return command;
		}

		/// <summary>
		/// Disposes the connection in use, there by freeing resources in use.
		/// </summary>
		/// <param name="disposing">If the object is being disposed or not.</param>
		/// <exception cref="InvalidOperationException">If there are open transactions.</exception>
		protected virtual void Dispose(bool disposing)
		{
			List<string> errors = new List<string>();

			if (transactions.Count > 0)
			{
				throw new InvalidOperationException(Exceptions.CannotDisposeWithArgOpenTransactions);
			}

			if (_connection != null)
			{
				_connection.Dispose();
			}
		}
		
		/// <summary>
		/// Prepares the command for execution. (Sets connection, timeout and opens connection if needed);
		/// </summary>
		/// <param name="command">The command to prep.</param>
		protected virtual void PrepCommand(IDbCommand command)
		{
			if (command != null)
			{
				command.Connection = this.Connection;
				command.CommandTimeout = Math.Max(command.CommandTimeout, this.CommandTimeout);
			}

			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
		}

		private T[] Execute<T>(ExecuteType type, params object[] commands)
		{
			if (commands.Length == 0)
			{
				throw new ArgumentException(Exceptions.ArgumentsEmpty, "commands");
			}

			if (commands.Any(c => c == null || (c is string && StringUtil.IsNullOrWhiteSpace((string)c))))
			{
				throw new ArgumentNullException("commands", Exceptions.ArgumentInListNull);
			}

			List<T> list = new List<T>();
			IDbCommand command = null;

			try
			{
				foreach (object c in commands)
				{
					command = c as IDbCommand;
					if (command == null)
					{
						command = this.CreateCommand(CommandType.Text, (string)c);
					}

					switch (type)
					{
						case ExecuteType.Dataset:
							list.Add((T)(object)this.ExecuteDataSet(command));
							break;
						case ExecuteType.NonQuery:
							list.Add((T)(object)this.ExecuteNonQuery(command));
							break;
						case ExecuteType.Scalar:
							list.Add((T)this.ExecuteScalar(command));
							break;
						default:
							throw new NotSupportedException();
					}
				}
			}
			catch
			{
				if (command != null)
				{
					Log.Logger.GetLogger(this.GetType()).Debug(Exceptions.ErrantCommandArg, command.CommandText);
				}

				if (transactions.Count > 0)
				{
					transactions.Peek().Rollback();
				}

				throw;
			}

			return list.ToArray();
		}

		#endregion
		#region ExecuteType Enum

		private enum ExecuteType
		{
			Dataset,
			NonQuery,
			Scalar
		}

		#endregion
	}
}