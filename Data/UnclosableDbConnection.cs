using System;
using System.Data;

namespace Adenson.Data
{
	internal sealed class UnclosableDbConnection : IDbConnection
	{
		public UnclosableDbConnection(IDbConnection conn)
		{
			if (conn == this)
			{
				throw new ArgumentNullException("Setting something to itself, hmm, not cool");
			}

			this.Connection = conn;
		}

		public IDbConnection Connection
		{
			get;
			private set;
		}

		public string ConnectionString
		{
			get { return this.Connection.ConnectionString; }
			set { this.Connection.ConnectionString = value; }
		}

		public int ConnectionTimeout
		{
			get { return this.Connection.ConnectionTimeout; }
		}

		public string Database
		{
			get { return this.Connection.Database; }
		}

		public ConnectionState State
		{
			get { return this.Connection.State; }
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return this.Connection.BeginTransaction(il);
		}

		public IDbTransaction BeginTransaction()
		{
			return this.Connection.BeginTransaction();
		}

		public void ChangeDatabase(string databaseName)
		{
			this.Connection.ChangeDatabase(databaseName);
		}

		public void Close()
		{
		}

		public IDbCommand CreateCommand()
		{
			return this.Connection.CreateCommand();
		}

		public void Open()
		{
			this.Connection.Open();
		}

		public void Dispose()
		{
		}
	}
}
