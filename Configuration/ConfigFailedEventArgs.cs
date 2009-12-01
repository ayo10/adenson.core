using System;

namespace Adenson.Configuration
{
	/// <summary>
	/// Simple Event class to let anyone listening the reason the configuration failed
	/// </summary>
	public sealed class ConfigFailedEventArgs : EventArgs
	{
		#region Variables
		/// <summary>
		/// Gets the exception that was thrown when he configuration failed
		/// </summary>
		public readonly Exception Exception;
		#endregion
		#region Constructor

		/// <summary>
		/// Creates a new instance of the ConfigFailedEventArgs class
		/// </summary>
		/// <param name="ex">The exception</param>
		public ConfigFailedEventArgs(Exception ex)
		{
			Exception = ex;
		}

		#endregion
	}
}