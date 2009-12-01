using System;
using System.Data;
using System.Data.SqlClient;

namespace Adenson.Data.SqlClient
{
	/// <summary>
	/// SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public static class SqlClientParameterCache
	{
		#region Variables
		private static System.Collections.Hashtable paramCache = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		#endregion
		#region Methods

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters to be cached</param>
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", ExceptionMessages.ArgumentNullOrEmpty);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNullOrEmpty);

			string hashKey = connectionString + ":" + commandText;
			paramCache[hashKey] = commandParameters;
		}
		/// <summary>
		/// Retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An array of SqlParamters</returns>
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", ExceptionMessages.ArgumentNullOrEmpty);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNullOrEmpty);

			string hashKey = connectionString + ":" + commandText;
			SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
			if (cachedParameters == null) return null;
			else return CloneParameters(cachedParameters);
		}
		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of SqlParameters</returns>
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}
		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a SqlConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of SqlParameters</returns>
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", ExceptionMessages.ArgumentNullOrEmpty);
			if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNullOrEmpty);

			using (SqlConnection connection = new SqlConnection(connectionString))
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
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of SqlParameters</returns>
		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
		{
			return GetSpParameterSet(connection, spName, false);
		}
		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of SqlParameters</returns>
		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", ExceptionMessages.ArgumentNull);
			using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
			{
				return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of SqlParameters</returns>
		private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", ExceptionMessages.ArgumentNull);
			if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNullOrEmpty);

			string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : String.Empty);

			SqlParameter[] cachedParameters;

			cachedParameters = paramCache[hashKey] as SqlParameter[];
			if (cachedParameters == null)
			{
				SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				paramCache[hashKey] = spParameters;
				cachedParameters = spParameters;
			}

			return CloneParameters(cachedParameters);
		}
		/// <summary>
		/// Resolve at run time the appropriate set of SqlParameters for a stored procedure
		/// </summary>
		/// <param name="connection">A valid SqlConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
		/// <returns>The parameter array discovered.</returns>
		private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", ExceptionMessages.ArgumentNull);
			if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNullOrEmpty);

			SqlCommand cmd = new SqlCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			connection.Open();
			SqlCommandBuilder.DeriveParameters(cmd);
			connection.Close();

			if (!includeReturnValueParameter)
			{
				cmd.Parameters.RemoveAt(0);
			}

			SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

			cmd.Parameters.CopyTo(discoveredParameters, 0);

			// Init the parameters with a DBNull value
			foreach (SqlParameter discoveredParameter in discoveredParameters)
			{
				discoveredParameter.Value = DBNull.Value;
			}
			return discoveredParameters;
		}
		/// <summary>
		/// Deep copy of cached SqlParameter array
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion
	}
}