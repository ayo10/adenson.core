using System;
using System.ComponentModel;
using System.Configuration;

namespace Adenson.Configuration
{
	/// <summary>
	/// Configuration helper class
	/// </summary>
	public static class ConfigHelper
	{
		#region Methods

		/// <summary>
		/// Returns the config section with specified group and name
		/// </summary>
		/// <param name="group">The section group name</param>
		/// <param name="name">The section name</param>
		/// <returns>Found section if any</returns>
		public static T GetSection<T>(string sectionName) where T : class
		{
			if (String.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");

			return (T)ConfigurationManager.GetSection(sectionName);
		}

		/// <summary>
		/// Returns the config section with specified group and name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="groupName">The section group name</param>
		/// <param name="sectionName">The section name</param>
		/// <returns>Found section if any</returns>
		public static T GetSection<T>(string groupName, string sectionName) where T : class
		{
			if (String.IsNullOrEmpty(groupName)) throw new ArgumentNullException("groupName");
			if (String.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");

			return ConfigHelper.GetSection<T>(String.Concat(groupName, "/", sectionName));
		}

		/// <summary>
		/// Reads the config file to get the specified value from the appSettings section.
		/// </summary>
		/// <typeparam name="T">The type of value to return</typeparam>
		/// <param name="key">The name of the value</param>
		/// <returns>Found value (and converted to type) if found, default of T otherwise.</returns>
		public static T GetValue<T>(string key)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

			return ConfigHelper.GetValue<T>(key, default(T));
		}

		/// <summary>
		/// Reads the config file to get the specified value from the appSettings section.
		/// </summary>
		/// <typeparam name="T">The type of value to return</typeparam>
		/// <param name="section">The name of the section to find.</param>
		/// <param name="defaultResult">The result to return if no value was found</param>
		/// <returns>Found value (and converted to type) if found, <paramref name="defaultResult"/> otherwise.</returns>
		public static T GetValue<T>(string key, T defaultResult)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

			string value = ConfigurationManager.AppSettings[key];
			T result = defaultResult;

			if (typeof(T) == typeof(string) && value != null) result = (T)(object)value;//one thing that sucks about generics is, you cant cast somethign straight to T if you dont specify T isnt a value type
			else if (!String.IsNullOrEmpty(value))
			{
				TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
				if (typeConverter.CanConvertFrom(typeof(String))) result = (T)typeConverter.ConvertFrom(value);
			}

			return (T)result;
		}

		#endregion
	}
}