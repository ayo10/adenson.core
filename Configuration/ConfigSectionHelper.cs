using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;

namespace Adenson.Configuration
{
	/// <summary>
	/// Hopes to simplify returning of config sections
	/// </summary>
	public static class ConfigSectionHelper
	{
		/// <summary>
		/// Returns the config section with specified group and name
		/// </summary>
		/// <param name="group">The section group name</param>
		/// <param name="name">The section name</param>
		/// <returns>Found section if any</returns>
		public static object GetSection(string group, string name)
		{
			return ConfigurationManager.GetSection(string.Concat(group, "/", name));
		}
		/// <summary>
		/// Returns the config section with specified group and name as a Dictionary object
		/// </summary>
		/// <param name="group">The section group name</param>
		/// <param name="name">The section name</param>
		/// <returns>Found section if any</returns>
		/// <exception cref="ArgumentException">if the section is not the type that returns IDictionary results</exception>
		public static IDictionary<string, string> GetDictionary(string group, string name)
		{
			return Convert(ConfigSectionHelper.GetSection(group, name));
		}

		internal static object GetSection(string section)
		{
			object result = ConfigSectionHelper.GetSection("adenson", section); //the new way
			if (result == null) result = ConfigSectionHelper.GetSection("adenson.Utils", section); //Logger and DirectoryInfo uses this
			if (result == null && section == "logger") result = ConfigSectionHelper.GetSection("adenson.log", section);
			if (result == null && section == "sqlhelper") result = ConfigSectionHelper.GetSection("adenson.data", section);
			if (result == null && section == "directoryinfo")
			{
				result = GetSection("adenson.io", section);
				if (result == null) GetSection("adenson.utils", "ioutils");
			}

			return result;
		}
		internal static Dictionary<string, string> GetDictionary(string section)
		{
			return Convert(ConfigSectionHelper.GetSection(section));
		}
		private static Dictionary<string, string> Convert(object obj)
		{
			if (obj == null) return null;

			IDictionary dictionary = obj as IDictionary;
			if (dictionary == null) throw new ArgumentException(Exceptions.SectionNotDictionarySection);

			Dictionary<string, string> result = new Dictionary<string, string>();

			foreach (string key in dictionary.Keys) result.Add(key, (string)dictionary[key]);//return a clone!!!

			return result;
		}
	}
}