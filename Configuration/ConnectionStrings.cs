using System;
using System.Configuration;

namespace Adenson.Configuration
{
	/// <summary>
	/// ConnectionStrings provides a simple repository of database connections
	/// strings, read at initialization time from an XML file.
	/// Repository for strings.  Its all static, so as to save loading work.
	/// </summary>
	public static class ConnectionStrings
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		public const string DefaultKey = "default";
		#endregion
		#region Properties

		/// <summary>
		/// Gets the default connection string
		/// </summary>
		public static ConnectionStringSettings Default
		{
			get { return Get(ConnectionStrings.DefaultKey, false); }
		}

		#endregion
		#region Methods

		/// <summary>
		/// We'll be nice and provide an ro indexer - oh wait, we can't.
		/// Well, this GetCS thing will have to do instead.
		/// </summary>
		/// <param name="key">The lookup key for the connection string!</param>
		/// <returns>A connection string</returns>
		/// <remarks>calls GetCS(key, true)</remarks>
		public static ConnectionStringSettings Get(string key)
		{
			return Get(key, true);
		}
		/// <summary>
		/// Gets a connection string using the key
		/// </summary>
		/// <param name="key">Key to use to do a lookup</param>
		/// <param name="useDefaultIfNull">If true, returns ConnectionStrings.Default if String.IsNullOrEmpty(result) == true</param>
		/// <returns>A connection string</returns>
		/// <exception cref="ArgumentNullException">if key is null or empty</exception>
		/// <exception cref="ArgumentOutOfRangeException">if no value for key could be found (will happen even if useDefaultIfNull is true if the default key does not exist)</exception>
		public static ConnectionStringSettings Get(string key, bool useDefaultIfNull)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
			ConnectionStringSettings result = ConnectionStrings.GetValue(key);
			if (result == null && useDefaultIfNull) result = ConnectionStrings.Default;
			if (result == null) throw new ConfigurationErrorsException(String.Format(Exceptions.ConnectionStringWithKeyArgNotFound, key, ConnectionStrings.DefaultKey));
			return result;
		}
		/// <summary>
		/// Calls ConnectionStrings.GetCS, catching ArgumentOutOfRangeException that would have been thrown.
		/// </summary>
		/// <param name="key">The lookup key for the connection string</param>
		/// <param name="connectionSetting">The connection setting if any</param>
		/// <returns>A connection string</returns>
		/// <remarks>calls GetCS(key, true)</remarks>
		public static bool TryGet(string key, out ConnectionStringSettings connectionSetting)
		{
			return ConnectionStrings.TryGet(key, false, out connectionSetting);
		}
		/// <summary>
		/// Calls ConnectionStrings.GetCS, catching ArgumentOutOfRangeException that would have been thrown.
		/// </summary>
		/// <param name="key">Key to use to do a lookup</param>
		/// <param name="useDefaultIfNull">If true, returns ConnectionStrings.Default if String.IsNullOrEmpty(result) == true, null otherwise</param>
		/// <param name="connectionSetting">The connection setting if any</param>
		/// <returns>A connection string</returns>
		public static bool TryGet(string key, bool useDefaultIfNull, out ConnectionStringSettings connectionSetting)
		{
			try
			{
				connectionSetting = ConnectionStrings.Get(key, useDefaultIfNull);
				return true;
			}
			catch
			{
			}
			connectionSetting = null;
			return false;
		}

		private static ConnectionStringSettings GetValue(string key)
		{
			return ConfigurationManager.ConnectionStrings[key];
		}

		#endregion
	}
}