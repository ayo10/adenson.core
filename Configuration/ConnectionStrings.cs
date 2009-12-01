using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using Adenson.Log;

namespace Adenson.Configuration
{
	/// <summary>
	/// ConnectionStrings provides a simple repository of database connections
	/// strings, read at initialization time from an XML file.
	/// Repository for strings.  Its all static, so as to save loading work.
	/// </summary>
	public static class ConnectionStrings
	{
		#region Properties

		/// <summary>
		/// Gets the default connection string
		/// </summary>
		public static string Default
		{
			get { return Get("default", false); }
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
		public static string Get(string key)
		{
			return Get(key, true);
		}
		/// <summary>
		/// Gets a connection string using the key
		/// </summary>
		/// <param name="key">Key to use to do a lookup</param>
		/// <param name="useDefaultIfNull">If true, returns ConnectionStrings.Default if string.IsNullOrEmpty(result) == true</param>
		/// <returns>A connection string</returns>
		/// <exception cref="ArgumentNullException">if key is null or empty</exception>
		/// <exception cref="ArgumentOutOfRangeException">if no value for key could be found (will happen even if useDefaultIfNull is true if the default key does not exist)</exception>
		public static string Get(string key, bool useDefaultIfNull)
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
			string result = ConnectionStrings.GetValue(key);
			if (string.IsNullOrEmpty(result) && useDefaultIfNull) result = ConnectionStrings.Default;
			if (result == null) throw new ArgumentOutOfRangeException(key, String.Format("No Connection String Found either for {0} or {1}", key, "default"));
			return result;
		}
		/// <summary>
		/// Calls ConnectionStrings.GetCS, catching ArgumentOutOfRangeException that would have been thrown.
		/// </summary>
		/// <param name="key">The lookup key for the connection string!</param>
		/// <returns>A connection string</returns>
		/// <remarks>calls GetCS(key, true)</remarks>
		public static string TryGet(string key)
		{
			return ConnectionStrings.TryGet(key, false);
		}
		/// <summary>
		/// Calls ConnectionStrings.GetCS, catching ArgumentOutOfRangeException that would have been thrown.
		/// </summary>
		/// <param name="key">Key to use to do a lookup</param>
		/// <param name="useDefaultIfNull">If true, returns ConnectionStrings.Default if string.IsNullOrEmpty(result) == true, null otherwise</param>
		/// <returns>A connection string</returns>
		public static string TryGet(string key, bool useDefaultIfNull)
		{
			try
			{
				return ConnectionStrings.Get(key, useDefaultIfNull);
			}
			catch
			{
			}
			return null;
		}

		private static string GetValue(string key)
		{
			string result = null;
			ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings[key];
			if (cs != null) result = cs.ConnectionString;
			return result;
		}

		#endregion
	}
}