using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Adenson.Configuration;
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
		private ConnectionManager _connection;
		private static Logger logger = Logger.GetLogger(typeof(SqlHelperBase));
		#endregion
		#region Constructors

		/// <summary>
		/// When implemented, should instantiate a new instance of the sql helper using 
		/// ConnectionStrings.GetCS(connectionKey) to retrieve the connection string
		/// </summary>
		/// <param name="connectionKey">The connection key to use</param>
		public SqlHelperBase(string connectionKey)
		{
			if (String.IsNullOrEmpty(connectionKey)) throw new ArgumentNullException("connectionKey");
			ConnectionStringSettings connectionString;
			if (!ConnectionStrings.TryGet(connectionKey, false, out connectionString)) throw new ArgumentNullException("ConnectionString", "Unable to determine connection string");
			this.ConnectionString = connectionString.ConnectionString;
		}
		/// <summary>
		/// When implemented, should instantiate a new instance of the sql helper using specified connection as a
		/// connection string if isConnectionString is true, else uses ConnectionStrings.GetCS to retrieve it
		/// </summary>
		/// <param name="connectionString"></param>
		public SqlHelperBase(ConnectionStringSettings connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException("connectionString");
			if (String.IsNullOrEmpty(connectionString.ConnectionString)) throw new ArgumentNullException("connectionString.ConnectionString");
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
		public System.IO.TextWriter Log
		{ 
			get; 
			set;
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
		/// Executes and returns a new DataSet from specified command
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>a new DataSet object</returns>
		public virtual DataSet ExecuteDataSet(IDbCommand command)
		{
			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			this.WriteLog(command);
			using (DbDataAdapter dataAdapter = this.CreateAdapter(command))
			{
				DataSet ds = new DataSet();
				dataAdapter.Fill(ds);
				this.Manager.Close();
				return ds;
			}
		}
		/// <summary>
		/// Executes the command and returns the number of rows affected.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The number of rows affected.</returns>
		public virtual int ExecuteNonQuery(IDbCommand command)
		{
			this.WriteLog(command);
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
		/// Executes the command returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public virtual object ExecuteScalar(IDbCommand command)
		{
			this.WriteLog(command);
			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			object result = command.ExecuteScalar();
			this.Manager.Close();
			return result;
		}
		/// <summary>
		/// Executes the command and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public virtual IDataReader ExecuteReader(IDbCommand command)
		{
			this.WriteLog(command);
			command.Connection = this.Manager.Connection;
			this.Manager.Open();
			IDataReader result = command.ExecuteReader(this.Manager.AllowClose ? CommandBehavior.CloseConnection : CommandBehavior.Default);
			return result;
		}
		/// <summary>
		/// Executes and returns a new DataSet from specified stored procedure
		/// </summary>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		public abstract DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes and returns a new DataSet from specified stored procedure using specified transaction
		/// </summary>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>a new DataSet object</returns>
		public abstract DataSet ExecuteDataSet(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteDataSet run on each command text</returns>
		public virtual DataSet[] ExecuteDataSetBatched(params string[] commandTexts)
		{
			IDbTransaction transaction = this.OpenConnection().BeginTransaction();
			List<DataSet> list = new List<DataSet>();
			if (commandTexts == null || commandTexts.Length == 0) return list.ToArray();
			foreach (string commandText in commandTexts)
			{
				if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandTexts", ExceptionMessages.ArgumentInListNull);
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
		/// Executes the stored procedure and returns the number of rows affected.
		/// </summary>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		public abstract int ExecuteNonQuery(CommandType type, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the stored procedure using specified transaction and returns the number of rows affected.
		/// </summary>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The number of rows affected.</returns>
		public abstract int ExecuteNonQuery(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteNonQuery run on each command text</returns>
		public virtual int[] ExecuteNonQueryBatched(params string[] commandTexts)
		{
			IDbTransaction transaction = this.OpenConnection().BeginTransaction();
			List<int> list = new List<int>();
			if (commandTexts == null || commandTexts.Length == 0) return list.ToArray();
			foreach (string commandText in commandTexts)
			{
				if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandTexts", ExceptionMessages.ArgumentInListNull);
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
		/// Executes the stored procedure and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public abstract IDataReader ExecuteReader(CommandType type, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the stored procedure using the specified transaction and builds an System.Data.IDataReader.
		/// </summary>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>An System.Data.IDataReader object.</returns>
		public abstract IDataReader ExecuteReader(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the stored procedure returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public abstract object ExecuteScalar(CommandType type, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the stored procedure using specified transaction returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.
		/// </summary>
		/// <param name="transaction">The transaction</param>
		/// <param name="commandText">The stored procedure name</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all)</param>
		/// <returns>The first column of the first row in the resultset.</returns>
		public abstract object ExecuteScalar(CommandType type, IDbTransaction transaction, string commandText, params object[] parameterValues);
		/// <summary>
		/// Executes the command texts in a batched mode with a transaction
		/// </summary>
		/// <param name="commandTexts">1 or more command texts</param>
		/// <returns>the result of each ExecuteScalar run on each command text</returns>
		public virtual object[] ExecuteScalarBatched(params string[] commandTexts)
		{
			IDbTransaction transaction = this.OpenConnection().BeginTransaction();
			List<object> list = new List<object>();
			if (commandTexts == null || commandTexts.Length == 0) return list.ToArray();
			foreach (string commandText in commandTexts)
			{
				if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandTexts", ExceptionMessages.ArgumentInListNull);
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
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public abstract DbDataAdapter CreateAdapter(IDbCommand command);
		/// <summary>
		/// Creates a new database connection for use by the helper methods
		/// </summary>
		/// <returns>New IDbConnection object</returns>
		public abstract IDbConnection CreateConnection();
		/// <summary>
		/// Uses CreateConnection() to get a new connection, and until CloseConnection is closed, uses that connection object
		/// </summary>
		/// <returns>The IDbConnection connection that will be used until CloseConnection is called.</returns>
		/// <exception cref="InvalidOperationException">if the method has been called already.</exception>
		public abstract IDbConnection OpenConnection();
		/// <summary>
		/// Closes the connection opened by OpenConnection, then, lets CreateConnection know to always create new connections henceforth.
		/// </summary>
		/// <exception cref="InvalidOperationException">if the method was called out of sequence, i.e., OpenConnection was never called, or called once and CloseConnection called multiple times.</exception>
		public abstract void CloseConnection();
		/// <summary>
		/// Clears any cached stored procedure parameters
		/// </summary>
		public abstract void ClearParameterCache();
		/// <summary>
		/// Clears parameters for the specfied stored procedure;
		/// </summary>
		public abstract void ClearParameterCache(string spName);
		/// <summary>
		/// Runs a system check for the existence of specified table
		/// </summary>
		/// <param name="tableName">the table name</param>
		/// <returns>True if the table exists, false otherwise</returns>
		public abstract bool CheckTableExists(string tableName);
		/// <summary>
		/// Disposes the object
		/// </summary>
		public abstract void Dispose();

		private void WriteLog(IDbCommand command)
		{
			if (this.Log != null)
			{
				throw new NotImplementedException();
			}
		}

		protected static bool IsNotEmpty(object[] parameterValues)
		{
			return (parameterValues != null) && (parameterValues.Length > 0);
		}

		#endregion

	}
}