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
			this.Open(null);
		}

		public void Open(IDbCommand command)
		{
			if (command != null)
			{
				command.Connection = this.Connection;
				command.CommandTimeout = Math.Max(command.CommandTimeout, _helper.CommandTimeout);
			}

			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
		}

		public void Close()
		{
			if (this.AllowClose && this.Connection.State != ConnectionState.Closed)
			{
				this.Connection.Close();
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
	}
}