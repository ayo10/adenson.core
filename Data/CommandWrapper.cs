using System;

namespace Adenson.Data
{
	internal sealed class CommandWrapper : IDisposable
	{
		private ConnectionManager connectionManager;

		public CommandWrapper(ConnectionManager connectionManager)
		{
			// TODO: Complete member initialization
			this.connectionManager = connectionManager;
		}

		public void Dispose()
		{
			connectionManager.Close();
		}
	}
}