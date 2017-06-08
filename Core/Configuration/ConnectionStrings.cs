using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

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
		/// The default key constant ('default').
		/// </summary>
		public const string DefaultKey = "default";
		#endregion
		#region Properties

		/// <summary>
		/// Gets the default connection string.
		/// </summary>
		public static ConnectionStringSettings Default
		{
			get { return Get(ConnectionStrings.DefaultKey, false); }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Finds the closest connection string to the specified, usually called if <paramref name="connectionString"/> is sanitized, i.e. doesn't have a password.
		/// </summary>
		/// <param name="connectionString">The connection string to compare with.</param>
		/// <returns>Enumration of matching connection strings.</returns>
		public static IEnumerable<ConnectionStringSettings> Closest(string connectionString)
		{
			Arg.IsNotNull(connectionString, "connectionString");

			var val = connectionString.ToUpper().ToDictionary(";");
			foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
			{
				var csd = cs.ConnectionString.ToUpper().ToDictionary(";");
				var matches = val.Keys.Intersect(csd.Keys);
				if (matches.Any() && matches.All(k => String.Equals(val[k], csd[k], StringComparison.CurrentCultureIgnoreCase)))
				{
					yield return cs;
				}
			}
		}

		/// <summary>
		/// We'll be nice and provide an ro indexer - oh wait, we can't.
		/// Well, this GetCS thing will have to do instead.
		/// </summary>
		/// <param name="key">The lookup key for the connection string!.</param>
		/// <returns>A connection string</returns>
		/// <remarks>Calls GetCS(key, true)</remarks>
		public static ConnectionStringSettings Get(string key)
		{
			return Get(key, true);
		}

		/// <summary>
		/// Gets a connection string using the key.
		/// </summary>
		/// <param name="key">Key to use to do a lookup.</param>
		/// <param name="useDefaultIfNull">If true, returns ConnectionStrings.Default if String.IsNullOrEmpty(result) == true.</param>
		/// <returns>A connection string</returns>
		/// <exception cref="ArgumentNullException">If key is null or empty</exception>
		/// <exception cref="ArgumentOutOfRangeException">If no value for key could be found (will happen even if useDefaultIfNull is true if the default key does not exist)</exception>
		public static ConnectionStringSettings Get(string key, bool useDefaultIfNull)
		{
			Arg.IsNotNull(key, "key");

			ConnectionStringSettings result = ConnectionStrings.GetValue(key);
			if (result == null)
			{
				if (useDefaultIfNull)
				{
					result = ConnectionStrings.Default;
				}
			}

			if (result == null)
			{
				throw new ConfigurationErrorsException(StringUtil.Format(Exceptions.ConnectionStringWithKeyArgNotFound, key, ConnectionStrings.DefaultKey));
			}

			return result;
		}

		private static ConnectionStringSettings GetValue(string key)
		{
			return ConfigurationManager.ConnectionStrings[key];
		}

		#endregion
	}
}