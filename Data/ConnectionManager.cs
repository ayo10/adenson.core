using System;
using System.Data;
using System.Data.Common;

namespace Adenson.Data
{
	internal sealed class ConnectionManager : IDisposable
	{
		#region Variables
		private SqlHelperBase _helper;
		private bool _allowClose = true;
		#endregion
		#region Constructor

		internal ConnectionManager(SqlHelperBase helper)
		{
			_helper = helper;
			this.Connection = helper.CreateConnection();
		}

		#endregion
		#region Properties

		public IDbConnection Connection
		{
			get;
			private set;
		}

		public bool AllowClose
		{
			get { return _allowClose; }
			set { _allowClose = value; }
		}

		#endregion
		#region Methods

		public void Open()
		{
			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
		}

		public IDisposable Open(IDbCommand command)
		{
			ConnectionManagerCloser cw = new ConnectionManagerCloser(this);
			command.Connection = this.Connection;
			command.CommandTimeout = Math.Max(command.CommandTimeout, _helper.CommandTimeout);
			this.Open();
			return cw;
		}

		public void Close()
		{
			if (this.AllowClose && this.Connection.State != ConnectionState.Closed)
			{
				this.Connection.Close();
				this.Connection.Dispose();
				this.Connection = null;
			}
		}

		public void Dispose()
		{
			if (this.Connection != null)
			{
				this.Connection.Dispose();
			}
		}

		#endregion
		#region Inner Class

		private sealed class ConnectionManagerCloser : IDisposable
		{
			private ConnectionManager _connectionManager;

			public ConnectionManagerCloser(ConnectionManager connectionManager)
			{
				_connectionManager = connectionManager;
			}

			public void Dispose()
			{
				_connectionManager.Close();
			}
		}

		#endregion
	}
}