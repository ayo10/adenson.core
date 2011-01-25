using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using System.Xml.Linq;

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
		/// Gets the first (in document order) child element with the specified <see cref="XName"/>.
		/// </summary>
		/// <param name="source">The <see cref="XElement"/> to look into</param>
		/// <param name="name">The <see cref="XName"/> to match.</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>A <see cref="XElement"/> that matches the specified<see cref="XName"/>, or null.</returns>
		/// <exception cref="ArgumentNullException">if source is null, OR name is null or name.LocalName is whitespace</exception>
		public static XElement Element(this XContainer source, XName name, StringComparison comparisonType)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (name == null || StringUtil.IsNullOrWhiteSpace(name.LocalName)) throw new ArgumentNullException("name");
			return source.Elements().FirstOrDefault(e => String.Equals(e.Name.LocalName, name.LocalName, comparisonType));
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
		/// Gets if the specified element has the specified sub element with specified key
		/// </summary>
		/// <param name="source">The element to look into</param>
		/// <param name="name">The key to look for</param>
		/// <returns>True if an element with specified key is found, false otherwise</returns>
		/// <exception cref="ArgumentNullException">if source is null, OR name is null or name.LocalName is whitespace</exception>
		public static bool HasElement(this XContainer source, XName name)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (name == null || StringUtil.IsNullOrWhiteSpace(name.LocalName)) throw new ArgumentNullException("name");
			return source.HasElement(name, StringComparison.CurrentCulture);
		}
		/// <summary>
		/// Gets if the specified element has the specified sub element with specified key
		/// </summary>
		/// <param name="source">The <see cref="XContainer"/> to look into</param>
		/// <param name="name">The key to look for</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>True if an element with specified key is found, false otherwise</returns>
		/// <exception cref="ArgumentNullException">if source is null, OR name is null or name.LocalName is whitespace</exception>
		public static bool HasElement(this XContainer source, XName name, StringComparison comparisonType)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (name == null || StringUtil.IsNullOrWhiteSpace(name.LocalName)) throw new ArgumentNullException("name");
			return source.Elements().FirstOrDefault(e => String.Equals(e.Name.LocalName, name.LocalName, comparisonType)) != null;
		}
		/// <summary>
		/// Checks to see if the specified value is empty or null
		/// </summary>
		/// <param name="values">The enumerable object to check</param>
		/// <returns>true if values is null or empty, false otherwise</returns>
		public static bool IsEmpty(this IEnumerable values)
		{
			return (values == null) || (values.Cast<object>().Count() == 0);
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
			return FileUtil.ReadStream(stream);
		}

		internal static int GetDisableProcessingCount(this Dispatcher dispatcher)
		{
			FieldInfo fieldInfo = dispatcher.GetType().GetField("_disableProcessingCount", BindingFlags.Instance | BindingFlags.NonPublic);
			object obj = fieldInfo.GetValue(dispatcher);
			return (int)obj;
		}
	}
}