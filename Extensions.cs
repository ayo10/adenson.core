using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace Adenson
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Determines whether the dictionary contains the specified key, using the comparism rule
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="dictionary">The dictionary to parse</param>
		/// <param name="key">The key to find</param>
		/// <param name="comparism">One of the enumeration values that specifies how the strings will be compared.</param>
		/// <returns>true if the value of the value parameter is the same as this string; otherwise, false.</returns>
		public static bool ContainsKey<T>(this Dictionary<string, T> dictionary, string key, StringComparison comparism)
		{
			return dictionary.Keys.Any(k => k.Equals(key, comparism));
		}
		/// <summary>
		/// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array. <see cref="String.Format(String, object)"/>
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		public static string Format(this string format, params object[] args)
		{
			return String.Format(format, args);
		}
		/// <summary>
		/// Gets the element with the specified key, case insensitive
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="dictionary">The dictionary to parse</param>
		/// <param name="key">The key to find</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a System.Collections.Generic.KeyNotFoundException, and a set operation creates a new element with the specified key.</returns>
		public static T GetValue<T>(this Dictionary<string, T> dictionary, string key)
		{
			string actualKey = dictionary.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.CurrentCultureIgnoreCase));
			return dictionary[actualKey];
		}
		/// <summary>
		/// Converts the specified value to a hex string, using BitConverter.ToString, but without the dashes
		/// </summary>
		/// <param name="buffer">The byte array</param>
		/// <returns></returns>
		public static string ToHex(this byte[] buffer)
		{
			if (buffer == null) return null;
			return BitConverter.ToString(buffer).Replace("-", String.Empty);
		}

		internal static int GetDisableProcessingCount(this Dispatcher dispatcher)
		{
			FieldInfo fieldInfo = dispatcher.GetType().GetField("_disableProcessingCount", BindingFlags.Instance | BindingFlags.NonPublic);
			object obj = fieldInfo.GetValue(dispatcher);
			return (int)obj;
		}
	}
}