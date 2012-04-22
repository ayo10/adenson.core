using System;
using System.Data;
using System.Data.Common;

namespace Adenson.Data
{
	internal sealed class ConnectionManager : IDisposable
	{
		#region Variables
		private bool _allowClose = true;
		#endregion
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
	}
}