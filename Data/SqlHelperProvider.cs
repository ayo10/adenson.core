using System;
using System.Configuration;
using System.Globalization;
using Adenson.Configuration;

namespace Adenson.Data
{
	/// <summary>
	/// All its job is to create a new SqlHelper class based on configuration files, works with either<br/>
	/// a) SimpleKeyValueConfigSet files with 'AssemblyName' and 'TypeName' keys -OR-<br/>
	/// b) IDictionary ConfigurationManager Section named 'Adenson/SqlHelper' with 'AssemblyName' and 'TypeName' attributes.
	/// </summary>
	public static class SqlHelperProvider
	{
		#region Methods

		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance
		/// </summary>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="TypeLoadException">The type specified in the configuration file was not found in the specified assembly.</exception>
		/// <exception cref="MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from System.MarshalByRefObject.</exception>
		/// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="BadImageFormatException">The assembly specified in the configuration file is not a valid assembly, could also be as a result of Version 2.0 or later of the common language runtime is currently loaded and assemblyName was compiled with a later version.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The assembly specified in the configuration file was not found.</exception>
		/// <exception cref="System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public static SqlHelperBase Create()
		{
			return SqlHelperProvider.Create(ConnectionStrings.Default);
		}
		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance
		/// </summary>
		/// <param name="connectionKey">Connection key that the new instance should use.</param>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="ArgumentNullException">if 'connectionKey' is null or empty.</exception>
		public static SqlHelperBase Create(string connectionKey)
		{
			if (String.IsNullOrEmpty(connectionKey)) throw new ArgumentNullException("connectionKey");

			return SqlHelperProvider.Create(ConnectionStrings.Get(connectionKey));
		}
		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="ArgumentNullException">if 'connectionString' is null or its 'ConnectionString' property is null.</exception>
		/// <exception cref="NotSupportedException">if unable to create a SqlHelperBase object from specified connectionstringSettings object.</exception>
		public static SqlHelperBase Create(ConnectionStringSettings connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException("connectionString");

			if (String.IsNullOrEmpty(connectionString.ProviderName))
			{
				string connString = connectionString.ConnectionString;
				if (connString.IndexOf(".mdf", StringComparison.CurrentCultureIgnoreCase) > -1) connectionString.ProviderName = "System.Data.SqlClient";
				else if (connString.IndexOf(".sdf", StringComparison.CurrentCultureIgnoreCase) > -1) connectionString.ProviderName = "System.Data.SqlServerCe";
				else
				{
					var splits = connString.Split(';');
					foreach (string keyval in splits)
					{
						var subsplit = keyval.Split('=');
						if (subsplit.Length > 1)
						{
							var split0 = subsplit[0];
							if (split0.Equals("provider", StringComparison.CurrentCultureIgnoreCase) || split0.Equals("providername", StringComparison.CurrentCultureIgnoreCase)) connectionString.ProviderName = subsplit[1];
						}
					}
				}
			}
			switch (connectionString.ProviderName)
			{
				case "System.Data.Odbc": return new OdbcSqlHelper(connectionString);
				case "System.Data.OleDb": return new OleDbSqlHelper(connectionString);
				case "System.Data.SqlClient": return new SqlClientHelper(connectionString);
				case "Microsoft.SqlServerCe.Client":
				case "System.Data.SqlServerCe":
				case "System.Data.SqlServerCe.3.5": return new SqlCeHelper(connectionString);
				case "System.Data.OracleClient": throw new NotSupportedException(connectionString.ProviderName);
				default: throw new NotSupportedException("Unable to determine sql provider type, please set the 'ProverName' property of the ConnectionStringSettings object");
			}
		}

		#endregion
	}
}