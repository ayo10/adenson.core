#if !NETSTANDARD1_0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;

namespace Adenson.Data
{
	/// <summary>
	/// Sql Helper base class
	/// </summary>
	public abstract partial class SqlHelperBase : IDisposable
	{
		#region Variables
		private IDbConnection _connection;
		private int _commandTimeout = 30;
		private Stack<IDbTransaction> transactions = new Stack<IDbTransaction>();
		#endregion
		#region Constructors

#if NET40 || NET45

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlHelperBase"/> class using.<see cref="System.Configuration.ConfigurationManager.ConnectionStrings"/> with key 'default'.
		/// </summary>
		protected SqlHelperBase() : this("default")
		{
			this.CloseConnection = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlHelperBase"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="connectionString">Either the connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		protected SqlHelperBase(string connectionString)
		{
			Arg.IsNotEmpty(connectionString);
			var cs = System.Configuration.ConfigurationManager.ConnectionStrings[connectionString];
			this.ConnectionString = cs == null ? connectionString : cs.ConnectionString;
			this.CloseConnection = true;
		}
#else
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlHelperBase"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="connectionString">The connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid.</exception>
		protected SqlHelperBase(string connectionString)
		{
			this.ConnectionString = Arg.IsNotEmpty(connectionString);
			this.CloseConnection = true;
		}
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlHelperBase"/> class using specified connection object.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="close">If to close the connection when this object is disposed.</param>
		/// <exception cref="ArgumentException">If specified connection is null.</exception>
		protected SqlHelperBase(IDbConnection connection, bool close)
		{
			_connection = Arg.IsNotNull(connection);
			this.ConnectionString = connection.ConnectionString;
			this.CloseConnection = close;
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
		/// Gets the connection string to use.
		/// </summary>
		public virtual string ConnectionString
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current connection object in use (if OpenConnection was invoked).
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

		/// <summary>
		/// Gets or sets a value indicating whether to close (and dispose) the connection when this object is disposed.
		/// </summary>
		protected bool CloseConnection
		{
			get;
			set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Runs a system check for the existence of specified column in the specified table using TSQL INFORMATION_SCHEMA.
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <param name="columnName">The column name.</param>
		/// <returns>True if the table exists, false otherwise</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="tableName"/> is null or empty, OR, <paramref name="columnName"/> is null or empty</exception>
		public virtual bool ColumnExists(string tableName, string columnName)
		{
			Arg.IsNotEmpty(tableName);
			Arg.IsNotEmpty(columnName);
			using (IDataReader r = this.ExecuteReader(CommandType.Text, "EXEC sp_executesql N'SELECT * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @p0 AND COLUMN_NAME = @p1", new Parameter("p0", tableName), new Parameter("p1", columnName)))
			{
				return r.Read();
			}
		}

		/// <summary>
		/// Creates a new database using information from the connection string.
		/// </summary>
		public virtual void CreateDatabase()
		{
			this.ExecuteNonQuery(CommandType.Text, $"CREATE DATABASE [{this.Connection.Database}]");
		}

		/// <summary>
		/// Creates a new data parametr for use in running commands.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		/// <returns>New <see cref="IDataParameter"/> object</returns>
		public IDataParameter CreateParameter(string name, object value)
		{
			var parameter = this.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value == null ? DBNull.Value : value;
			return parameter;
		}

		/// <summary>
		/// Disposes the object.
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
			this.ExecuteNonQuery(CommandType.Text, $"DROP DATABASE [{this.Connection.Database}]");
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
				throw new InvalidOperationException("There are no current open transactions.");
			}

			IDbTransaction transaction = transactions.Pop();
			transaction.Commit();
			return transactions.Count;
		}

		/// <summary>
		/// Runs a query to determine if the specified table exists using TSQL INFORMATION_SCHEMA.
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public virtual bool TableExists(string tableName)
		{
			Arg.IsNotEmpty(tableName);
			using (IDataReader r = this.ExecuteReader(CommandType.Text, "EXEC sp_executesql N'SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @p0", new Parameter("p0", tableName)))
			{
				return r.Read();
			}
		}
		
		#region ExecuteDynamic

		/// <summary>
		/// Executes the specified command text, runs as a System.Data.IDataReader, reads the result into a dynamic object.
		/// </summary>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="type">The command type.</param>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="command">The command.</param>
		/// <returns>An System.Data.IDataDynamic object.</returns>
		public virtual IEnumerable<dynamic> ExecuteDynamic(IDbCommand command)
		{
			Arg.IsNotNull(command);
			this.PrepCommand(command);
			List<dynamic> result = new List<dynamic>();
			using (IDataReader r = command.ExecuteReader())
			{
				while (r.Read())
				{
					dynamic dyna = new ExpandoObject();
					IDictionary<string, object> dynadic = dyna as IDictionary<string, object>;

					for (var i = 0; i < r.FieldCount; i++)
					{
						object value = r.GetValue(i);
						dynadic["_Item" + i] = dynadic[r.GetName(i)] = value == DBNull.Value ? null : value;
					}

					result.Add(dyna);
				}
			}

			return result;
		}

		#endregion
		#region ExecuteNonQuery

		/// <summary>
		/// Executes the specified command text and returns the number of rows affected.
		/// </summary>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="type">The command type.</param>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="command">The command.</param>
		/// <returns>The number of rows affected.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="command"/> is null</exception>
		public virtual int ExecuteNonQuery(IDbCommand command)
		{
			Arg.IsNotNull(command);
			this.PrepCommand(command);
			return command.ExecuteNonQuery();
		}

		#endregion
		#region ExecuteReader

		/// <summary>
		/// Executes the specified command text and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="type">The command type.</param>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="command">The command.</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(IDbCommand command)
		{
			Arg.IsNotNull(command);
			this.PrepCommand(command);
			return command.ExecuteReader();
		}

		#endregion
		#region ExecuteScalar

		/// <summary>
		/// Executes the specified command text returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="transaction">The transaction to use.</param>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="transaction"/> is null</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual object ExecuteScalar(IDbTransaction transaction, string commandText, params object[] parameterValues)
		{
			Arg.IsNotNull(transaction);
			return this.ExecuteScalar(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes the specified command text returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="type">The command type.</param>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
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
		/// <param name="command">The command.</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="command"/> is null</exception>
		public virtual object ExecuteScalar(IDbCommand command)
		{
			Arg.IsNotNull(command);
			this.PrepCommand(command);
			return command.ExecuteScalar();
		}

		#endregion
		#region Abstract 

		/// <summary>
		/// Creates a new command object for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		public abstract IDbCommand CreateCommand();

		/// <summary>
		/// Creates a new database connection for use by the helper methods.
		/// </summary>
		/// <returns>New <see cref="IDbConnection"/> object</returns>
		public abstract IDbConnection CreateConnection();

		/// <summary>
		/// Creates a new data parametr for use in running commands.
		/// </summary>
		/// <returns>New <see cref="IDataParameter"/> object</returns>
		public abstract IDbDataParameter CreateParameter();

		/// <summary>
		/// Runs a check for the existence of database specified in the connectionstring.
		/// </summary>
		/// <returns>True if the database exists, false otherwise</returns>
		public abstract bool DatabaseExists();

		#endregion

		/// <summary>
		/// Creates a new command object of specified type using specified <paramref name="parameterValues"/>.
		/// </summary>
		/// <param name="type">The type of command.</param>
		/// <param name="commandText">The command text.</param>
		/// <param name="parameterValues">The parameter values, which can be an array of <see cref="Parameter"/>, <see cref="IDataParameter"/>.</param>
		/// <returns>New <see cref="IDbCommand"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty, OR, parameterValues is not empty but any item in it is null</exception>
		[SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Fine as is.")]
		[SuppressMessage("Microsoft.Security", "CA2100", Justification = "Class is a sql execution helper, executes whatever is passed to it without any validation.")]
		protected virtual IDbCommand CreateCommand(CommandType type, string commandText, params object[] parameterValues)
		{
			Arg.IsNotEmpty(commandText, nameof(commandText));
			Arg.IsNotNull(parameterValues, nameof(parameterValues));
			Arg.IsNotAllNull(parameterValues, nameof(parameterValues));

			List<IDataParameter> parameters = new List<IDataParameter>();
			Exception exception = null;
			if (parameterValues.Any(p => p is Parameter) || parameterValues.Any(p => p is IDataParameter))
			{
				foreach (var item in parameterValues)
				{
					IDataParameter dataParameter = item as IDataParameter;
					var parameter = item as Parameter;
					if (dataParameter == null && parameter != null)
					{
						dataParameter = this.CreateParameter(parameter.Name, parameter.Value);
						if (parameter.DbType != null)
						{
							dataParameter.DbType = parameter.DbType.Value;
						}
					}

					parameters.Add(dataParameter);
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
			else if (parameterValues.Length == 1 && parameterValues.First() is IDictionary)
			{
				IDictionary dic = parameterValues[0] as IDictionary;
				foreach (var k in dic.Keys)
				{
					IDataParameter dataParameter = this.CreateParameter((string)k, dic[k]);
					parameters.Add(dataParameter);
				}
			}
			else if (type == CommandType.StoredProcedure)
			{
				for (var i = 0; i < parameterValues.Length; i++)
				{
					parameters.Add(this.CreateParameter("@param" + i, parameterValues[i]));
				}
			}

			if (exception != null || (parameterValues.Length != parameters.Count || parameters.Any(p => p == null)))
			{
				throw new ArgumentException(
					@"The specified command text argument could not be parsed.\r\n"
					+ "Use sql parameterized strings(e.g.'UPDATE [Table] SET [Column1] = @parameter1, [Column2] = @parameter2') and parameter arguments that MUST be one of 'Adenson.Data.Parameter', 'System.Data.IDataParameter', System.Collections.Generic.KeyValuePair<string, object> or System.Collections.IDictionary.",
					exception);
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
			if (_connection != null && this.CloseConnection)
			{
				_connection.Dispose();
			}
		}

		/// <summary>
		/// Prepares the command for execution. (Sets connection, timeout and opens connection if needed);.
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

		#endregion
	}
}
#endif