using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
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
		#region Variables
		private static Log.Logger logger = new Adenson.Log.Logger(typeof(SqlHelperProvider));
		private static string assemblyName, typeName;
		#endregion
		#region Constructor

		static SqlHelperProvider()
		{
			Dictionary<string, string> dict = ConfigSectionHelper.GetDictionary("SqlHelper");
			if (dict != null)
			{
				assemblyName = dict["AssemblyName"];
				typeName = dict["TypeName"];
				return;
			}
		}

		#endregion
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
			return SqlHelperProvider.Create(ConnectionStrings.Default, true);
		}
		/// <summary>
		/// Creates a new ISqlHelper instance, information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance
		/// </summary>
		/// <param name="arguments">Arguments to use to create a new ISqlHelper instance.</param>
		/// <returns>New ISqlHelper instance if one was created successfully</returns>
		/// <exception cref="MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="TypeLoadException">The type specified in the configuration file was not found in the specified assembly.</exception>
		/// <exception cref="MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from System.MarshalByRefObject.</exception>
		/// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="BadImageFormatException">The assembly specified in the configuration file is not a valid assembly, could also be as a result of Version 2.0 or later of the common language runtime is currently loaded and assemblyName was compiled with a later version.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The assembly specified in the configuration file was not found.</exception>
		/// <exception cref="System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public static SqlHelperBase Create(object[] arguments)
		{
			try
			{
				AppDomain.CurrentDomain.Load(assemblyName);
				return (SqlHelperBase)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true, BindingFlags.CreateInstance, null, arguments, null, null, null);
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
				throw;
			}
		}
		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance
		/// </summary>
		/// <param name="connectionKey">Connection key that the new instance should use.</param>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="TypeLoadException">The type specified in the configuration file was not found in the specified assembly.</exception>
		/// <exception cref="MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from System.MarshalByRefObject.</exception>
		/// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="BadImageFormatException">The assembly specified in the configuration file is not a valid assembly, could also be as a result of Version 2.0 or later of the common language runtime is currently loaded and assemblyName was compiled with a later version.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The assembly specified in the configuration file was not found.</exception>
		/// <exception cref="System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public static SqlHelperBase Create(string connectionKey)
		{
			if (assemblyName == null && typeName == null || assemblyName == "Adenson.Core" && typeName == "Adenson.Data.SqlClient.SqlClientImpl") return new SqlClient.SqlClientImpl(connectionKey);
			try
			{
				AppDomain.CurrentDomain.Load(assemblyName);
				return (SqlHelperBase)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true, BindingFlags.CreateInstance, null, new object[] { connectionKey }, null, null, null);
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
				throw;
			}
		}
		/// <summary>
		/// Creates a new SqlHelperBase instance using information from configuration files. If none exist, returns a new Adenson.Data.SqlClient.SqlClientImpl instance
		/// </summary>
		/// <param name="connectionKeyOrString">Either a connection string or a connection key.</param>
		/// <param name="isConnectionString">if the first argumet is a connection string</param>
		/// <returns>New SqlHelperBase instance if one was created successfully</returns>
		/// <exception cref="MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="TypeLoadException">The type specified in the configuration file was not found in the specified assembly.</exception>
		/// <exception cref="MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from System.MarshalByRefObject.</exception>
		/// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="BadImageFormatException">The assembly specified in the configuration file is not a valid assembly, could also be as a result of Version 2.0 or later of the common language runtime is currently loaded and assemblyName was compiled with a later version.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The assembly specified in the configuration file was not found.</exception>
		/// <exception cref="System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public static SqlHelperBase Create(string connectionKeyOrString, bool isConnectionString)
		{
			if (assemblyName == null && typeName == null || assemblyName == "Adenson.Core" && typeName == "Adenson.Data.SqlClient.SqlClientImpl") return new SqlClient.SqlClientImpl(connectionKeyOrString, isConnectionString);
			try
			{
				AppDomain.CurrentDomain.Load(assemblyName);
				return (SqlHelperBase)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true, BindingFlags.CreateInstance, null, new object[] { connectionKeyOrString, isConnectionString }, null, null, null);
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
				throw;
			}
		}

		#endregion
	}
}