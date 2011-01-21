using System;
using System.Collections.Generic;
using System.IO;
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
		/// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
		/// <returns>true if the value of the value parameter is the same as this string; otherwise, false.</returns>
		public static bool ContainsKey<T>(this Dictionary<string, T> dictionary, string key, StringComparison comparison)
		{
			return dictionary.Keys.Any(k => k.Equals(key, comparison));
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
		/// Checks to see if the specified value is empty or null
		/// </summary>
		/// <param name="values">The array to check</param>
		/// <returns>true if values is null or empty, false otherwise</returns>
		public static bool IsEmpty(this object[] values)
		{
			return (values == null) || (values.Length == 0);
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
		/// <summary>
		/// Converts specified stream to a byte array
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <returns>byte array, or null if stream is null</returns>
		public static byte[] ToBytes(this Stream stream)
		{
			if (stream == null) return null;
			return Util.ReadStream(stream);
		}

		internal static int GetDisableProcessingCount(this Dispatcher dispatcher)
		{
			FieldInfo fieldInfo = dispatcher.GetType().GetField("_disableProcessingCount", BindingFlags.Instance | BindingFlags.NonPublic);
			object obj = fieldInfo.GetValue(dispatcher);
			return (int)obj;
		}
	}
}