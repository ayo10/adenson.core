using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance.
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
			#if NETSTANDARD1_6 || NETSTANDARD1_5 || NETSTANDARD1_3
			throw new NotSupportedException();
			#else
			return SqlHelperProvider.Create(Configuration.ConnectionStrings.Default);
			#endif
		}

		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance.
		/// </summary>
		/// <param name="connectionKey">Connection key that the new instance should use.</param>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="ArgumentNullException">If 'connectionKey' is null or empty.</exception>
		public static SqlHelperBase Create(string connectionKey)
		{
			Arg.IsNotEmpty(connectionKey);
			#if NETSTANDARD1_6 || NETSTANDARD1_5 || NETSTANDARD1_3
			throw new NotSupportedException();
			#else
			return SqlHelperProvider.Create(Configuration.ConnectionStrings.Get(connectionKey));
			#endif
		}

		#if !NETSTANDARD1_6 && !NETSTANDARD1_5 && !NETSTANDARD1_3
		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="ArgumentNullException">If 'connectionString' is null or its 'ConnectionString' property is null.</exception>
		/// <exception cref="NotSupportedException">If unable to create a SqlHelperBase object from specified connectionstringSettings object.</exception>
		[SuppressMessage("Microsoft.Globalization", "CA1304", Justification = "Fine as is.")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Fine as is.")]
		public static SqlHelperBase Create(System.Configuration.ConnectionStringSettings connectionString)
		{
			Arg.IsNotNull(connectionString);
			if (String.IsNullOrEmpty(connectionString.ProviderName))
			{
				string connString = connectionString.ConnectionString;
				if (connString.IndexOf(".mdf", StringComparison.CurrentCultureIgnoreCase) > -1)
				{
					connectionString.ProviderName = "System.Data.SqlClient";
				}
				else if (connString.IndexOf(".sdf", StringComparison.CurrentCultureIgnoreCase) > -1)
				{
					connectionString.ProviderName = "System.Data.SqlServerCe";
				}
				else
				{
					string[] splits = connString.Split(';');
					foreach (string keyval in splits)
					{
						var subsplit = keyval.Split('=');
						if (subsplit.Length > 1)
						{
							var split0 = subsplit[0];
							if (split0.Equals("provider", StringComparison.CurrentCultureIgnoreCase) || split0.Equals("providername", StringComparison.CurrentCultureIgnoreCase))
							{
								connectionString.ProviderName = subsplit[1];
							}
						}
					}
				}
			}

			if (String.IsNullOrEmpty(connectionString.ProviderName))
			{
				throw new ArgumentException("Unable to determine sql provider type, please set the 'ProverName' property of the ConnectionStringsSettings object");
			}

			Assembly assembly;
			switch (connectionString.ProviderName.ToLower())
			{
				case "system.data.odbc":
					return new OdbcSqlHelper(connectionString);
				case "system.data.oledb":
					return new OleDbSqlHelper(connectionString);
				case "system.data.sqlclient":
					return new SqlServerHelper(connectionString);
				case "microsoft.sqlserverce.client":
				case "system.data.sqlserverce":
				case "system.data.sqlserverce.3.5":
				case "system.data.sqlserverce.4.0":
					assembly = Assembly.Load("Adenson.Core.SqlCe");
					return (SqlHelperBase)assembly?.CreateInstance("Adenson.Data.SqlCeHelper", true, BindingFlags.CreateInstance, null, new object[] { connectionString }, null, null);
				case "system.data.oracleclient":
					throw new NotSupportedException(connectionString.ProviderName);
				default:
					return SqlHelperProvider.CreateCustomHelper(connectionString.ProviderName);
			}
		}

		private static SqlHelperBase CreateCustomHelper(string providerName)
		{
			Type type = TypeUtil.GetType(providerName, false);
			if (type != null && typeof(SqlHelperBase).IsAssignableFrom(type))
			{
				return TypeUtil.CreateInstance<SqlHelperBase>(type);
			}

			throw new NotSupportedException(providerName);
		}

		#endif

		#endregion
	}
}