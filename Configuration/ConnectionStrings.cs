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
			if (result == null) throw new ConfigurationErrorsException(StringUtil.Format(Exceptions.ConnectionStringWithKeyArgNotFound, key, ConnectionStrings.DefaultKey));
			return result;
		}

		private static ConnectionStringSettings GetValue(string key)
		{
			return ConfigurationManager.ConnectionStrings[key];
		}

		#endregion
	}
}