using System;
using System.Data;
using System.Data.OleDb;

namespace Adenson.Data.OleDb
{
	/// <summary>
	/// OleDbHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public static class OleDbParameterCache
	{
		#region Variables
		private static System.Collections.Hashtable paramCache = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		#endregion
		#region Methods

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of OleDbParamters to be cached</param>
		public static void CacheParameterSet(string connectionString, string commandText, params OleDbParameter[] commandParameters)
		{
			if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNullOrEmpty);

			string hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}
		/// <summary>
		/// Retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <returns>An array of OleDbParamters</returns>
		public static OleDbParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", ExceptionMessages.ArgumentNullOrEmpty);
			if (String.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText", ExceptionMessages.ArgumentNullOrEmpty);

			string hashKey = connectionString + ":" + commandText;

			OleDbParameter[] cachedParameters = paramCache[hashKey] as OleDbParameter[];
			if (cachedParameters == null) return null;
			else return CloneParameters(cachedParameters);
		}
		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OleDbParameters</returns>
		public static OleDbParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}
		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OleDbParameters</returns>
		public static OleDbParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString", ExceptionMessages.ArgumentNullOrEmpty);
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNullOrEmpty);

			using (OleDbConnection connection = new OleDbConnection(connectionString))
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
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OleDbParameters</returns>
		internal static OleDbParameter[] GetSpParameterSet(OleDbConnection connection, string spName)
		{
			return GetSpParameterSet(connection, spName, false);
		}
		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OleDbParameters</returns>
		internal static OleDbParameter[] GetSpParameterSet(OleDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", ExceptionMessages.ArgumentNull);
			using (OleDbConnection clonedConnection = (OleDbConnection)((ICloneable)connection).Clone())
			{
				return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OleDbParameters</returns>
		private static OleDbParameter[] GetSpParameterSetInternal(OleDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", ExceptionMessages.ArgumentNull);
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNullOrEmpty);

			string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : String.Empty);

			OleDbParameter[] cachedParameters;

			cachedParameters = paramCache[hashKey] as OleDbParameter[];
			if (cachedParameters == null)
			{
				OleDbParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				paramCache[hashKey] = spParameters;
				cachedParameters = spParameters;
			}

			return CloneParameters(cachedParameters);
		}
		/// <summary>
		/// Resolve at run time the appropriate set of OleDbParameters for a stored procedure
		/// </summary>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
		/// <returns>The parameter array discovered.</returns>
		private static OleDbParameter[] DiscoverSpParameterSet(OleDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection", ExceptionMessages.ArgumentNull);
			if (String.IsNullOrEmpty(spName)) throw new ArgumentNullException("spName", ExceptionMessages.ArgumentNullOrEmpty);

			OleDbCommand cmd = new OleDbCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			connection.Open();
			OleDbCommandBuilder.DeriveParameters(cmd);
			connection.Close();

			if (!includeReturnValueParameter)
			{
				cmd.Parameters.RemoveAt(0);
			}

			OleDbParameter[] discoveredParameters = new OleDbParameter[cmd.Parameters.Count];

			cmd.Parameters.CopyTo(discoveredParameters, 0);

			// Init the parameters with a DBNull value
			foreach (OleDbParameter discoveredParameter in discoveredParameters)
			{
				discoveredParameter.Value = DBNull.Value;
			}
			return discoveredParameters;
		}
		/// <summary>
		/// Deep copy of cached OleDbParameter array
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private static OleDbParameter[] CloneParameters(OleDbParameter[] originalParameters)
		{
			OleDbParameter[] clonedParameters = new OleDbParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (OleDbParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion
	}
}