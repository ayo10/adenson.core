using System;
using System.Data;

namespace Adenson.Data
{
	public sealed class ConnectionManager
	{
		#region Constructor

		internal ConnectionManager(IDbConnection connection)
		{
			this.Connection = connection;
		}

		#endregion
		#region Properties

		public IDbConnection Connection
		{
			get;
			private set;
		}
		public bool AllowClose = true; 

		#endregion
		#region Methods

		public void Open()
		{
			if (this.Connection.State != ConnectionState.Open) this.Connection.Open();
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
			if (this.Connection != null) this.Connection.Dispose();
		}

		#endregion
	}
}