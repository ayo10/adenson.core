using System;
using System.Data;
using System.Data.Odbc;

namespace Adenson.Data.Odbc
{
	/// <summary>
	/// OdbcHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public static class OdbcParameterCache
	{
		#region Variables
		private static System.Collections.Hashtable paramCache = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		#endregion
		#region Methods

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OdbcConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of OdbcParamters to be cached</param>
		public static void CacheParameterSet(string connectionString, string commandText, params OdbcParameter[] commandParameters)
		{
			if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", Exceptions.ArgumentNull);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);

			string hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}
		/// <summary>
		/// Retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OdbcConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An array of OdbcParamters</returns>
		public static OdbcParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", Exceptions.ArgumentNull);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", Exceptions.ArgumentNull);

			string hashKey = connectionString + ":" + commandText;

			OdbcParameter[] cachedParameters = paramCache[hashKey] as OdbcParameter[];
			if (cachedParameters == null) return null;
			else return CloneParameters(cachedParameters);
		}
		/// <summary>
		/// Retrieves the set of OdbcParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OdbcConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OdbcParameters</returns>
		public static OdbcParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}
		/// <summary>
		/// Retrieves the set of OdbcParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OdbcConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OdbcParameters</returns>
		public static OdbcParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", Exceptions.ArgumentNull);
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", Exceptions.ArgumentNull);

			using (OdbcConnection connection = new OdbcConnection(connectionString))
			{
				return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
			}
		}
		/// <summary>
		/// Clears the parameter cache
		/// </summary>
		public static void Clear()
		{
			paramCache.Clear();
		}
		/// <summary>
		/// Clears the parameter cache
		/// </summary>
		public static void Clear(string spName)
		{
			System.Collections.Generic.List<string> keys = new System.Collections.Generic.List<string>();
			foreach (string key in paramCache.Keys)
			{
				if (key.EndsWith(spName, StringComparison.CurrentCultureIgnoreCase) && !keys.Contains(key)) keys.Add(key);
			}
			foreach (string key in keys) paramCache.Remove(key);
		}

		/// <summary>
		/// Retrieves the set of OdbcParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid OdbcConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OdbcParameters</returns>
		internal static OdbcParameter[] GetSpParameterSet(OdbcConnection connection, string spName)
		{
			return GetSpParameterSet(connection, spName, false);
		}
		/// <summary>
		/// Retrieves the set of OdbcParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid OdbcConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OdbcParameters</returns>
		internal static OdbcParameter[] GetSpParameterSet(OdbcConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", Exceptions.ArgumentNull);
			using (OdbcConnection clonedConnection = (OdbcConnection)((ICloneable)connection).Clone())
			{
				return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of OdbcParameters appropriate for the stored procedure
		/// </summary>
		/// <param name="connection">A valid OdbcConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OdbcParameters</returns>
		private static OdbcParameter[] GetSpParameterSetInternal(OdbcConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", Exceptions.ArgumentNull);
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", Exceptions.ArgumentNull);

			string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : String.Empty);

			OdbcParameter[] cachedParameters;

			cachedParameters = paramCache[hashKey] as OdbcParameter[];
			if (cachedParameters == null)
			{
				OdbcParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				paramCache[hashKey] = spParameters;
				cachedParameters = spParameters;
			}

			return CloneParameters(cachedParameters);
		}
		/// <summary>
		/// Resolve at run time the appropriate set of OdbcParameters for a stored procedure
		/// </summary>
		/// <param name="connection">A valid OdbcConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
		/// <returns>The parameter array discovered.</returns>
		private static OdbcParameter[] DiscoverSpParameterSet(OdbcConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", Exceptions.ArgumentNull);
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", Exceptions.ArgumentNull);

			OdbcCommand cmd = new OdbcCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			connection.Open();
			OdbcCommandBuilder.DeriveParameters(cmd);
			connection.Close();

			if (!includeReturnValueParameter)
			{
				cmd.Parameters.RemoveAt(0);
			}

			OdbcParameter[] discoveredParameters = new OdbcParameter[cmd.Parameters.Count];

			cmd.Parameters.CopyTo(discoveredParameters, 0);

			// Init the parameters with a DBNull value
			foreach (OdbcParameter discoveredParameter in discoveredParameters)
			{
				discoveredParameter.Value = DBNull.Value;
			}
			return discoveredParameters;
		}
		/// <summary>
		/// Deep copy of cached OdbcParameter array
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private static OdbcParameter[] CloneParameters(OdbcParameter[] originalParameters)
		{
			OdbcParameter[] clonedParameters = new OdbcParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (OdbcParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion
	}
}