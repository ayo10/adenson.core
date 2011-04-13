﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Adenson
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Gets if the specified <paramref name="value"/> is in the specified <paramref name="source"/> using specified <see cref="StringComparison"/> object.
		/// </summary>
		/// <param name="source">The string to look into</param>
		/// <param name="value">The string to seek.</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
		/// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> is null OR <paramref name="value"/> is null.</exception>
		public static bool Contains(this string source, string value, StringComparison comparisonType)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (value == null) throw new ArgumentNullException("value");
			return source.IndexOf(value, comparisonType) > -1;
		}
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
		/// Does equality comparism of both arrays, if not same instance, then item by item comparism
		/// </summary>
		/// <typeparam name="T">The type of items</typeparam>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		public static bool Equals<T>(this IEnumerable<T> array1, IEnumerable<T> array2)
		{
			if (Object.ReferenceEquals(array1, array2)) return true;
			if (array1 == null && array2 != null || array1 != null && array2 == null) return false;
			if (array1.Count() != array2.Count()) return false;
			for (int i = 0; i < array1.Count(); i++)
			{
				T e1 = array1.ElementAt(i);
				T e2 = array2.ElementAt(i);
				if (!Object.Equals(e1, e2)) return false;
			}
			return true;
		}
		/// <summary>
		/// Determines all the strings in array1 and array2 are both equal (same instance @ same index) using <see cref="StringComparison.CurrentCultureIgnoreCase"/>.
		/// </summary>
		/// <remarks>calls Extensions.Equals(array1, array2, StringComparison.CurrentCultureIgnoreCase)</remarks>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		public static bool Equals(this IEnumerable<string> array1, IEnumerable<string> array2)
		{
			return Extensions.Equals(array1, array2, StringComparison.CurrentCultureIgnoreCase);
		}
		/// <summary>
		/// Determines all the strings in array1 and array2 are both equal (same instance @ same index) using the specified culture, case, and sort rules used in the comparison.
		/// </summary>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		public static bool Equals(this IEnumerable<string> array1, IEnumerable<string> array2, StringComparison comparisonType)
		{
			if (Object.ReferenceEquals(array1, array2)) return true;
			if (array1 == null && array2 != null || array1 != null && array2 == null) return false;
			if (array1.Count() != array2.Count()) return false;
			for (int i = 0; i < array1.Count(); i++)
			{
				string e1 = array1.ElementAt(i);
				string e2 = array2.ElementAt(i);
				if (e1 == null && e2 != null || e1 != null && e2 == null) return false;
				if (!e1.Equals(e2, comparisonType)) return false;
			}
			return true;
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
		/// Gets the value of a child of specified <paramref name="source"/> with specified name, and converts it into specified type
		/// </summary>
		/// <typeparam name="T">The type to convert the value to</typeparam>
		/// <param name="source">The <see cref="XElement"/> to look into</param>
		/// <param name="name">The <see cref="XName"/> to match.</param>
		/// <returns>Found value of any, default of T otherwise</returns>
		/// <exception cref="ArgumentNullException">if source is null, OR name is null or name.LocalName is whitespace</exception>
		public static T GetValue<T>(this XContainer source, XName name)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (name == null || StringUtil.IsNullOrWhiteSpace(name.LocalName)) throw new ArgumentNullException("name");

			T result = default(T);
			var element = source.Element(name);
			if (element != null)
			{
				var value = element.Value;
				T output;
				if (TypeUtil.TryConvert<T>(value, out output)) result = output;
			}
			return result;
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
		/// Converts specified stream to a byte array
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <returns>byte array, or null if stream is null</returns>
		public static byte[] ToBytes(this Stream stream)
		{
			if (stream == null) return null;
			return FileUtil.ReadStream(stream);
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
		/// Converts specified integer value to roman numeral
		/// </summary>
		/// <remarks>http://www.dreamcubes.com/b2/software-development/21/to-roman-c-int-to-roman-converter/</remarks>
		/// <param name="value">The value to convert</param>
		/// <returns>Returns '0', if <paramref name="value"/> was 0 else the value in roman numeral</returns>
		public static string ToRoman(this int value)
		{
			if (value == 0) return "O";

			var wasNegative = value < 0;
			if (wasNegative) value = value * -1;

			int[] nums = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
			string[] romanNums = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };

			string result = "";
			int n = value;
			for (int i = nums.Length - 1; i >= 0; --i)
			{
				while (n >= nums[i])
				{
					n -= nums[i];
					result += romanNums[i];
				}
			}
			return (wasNegative ? "-" : String.Empty) + result;
		}
		/// <summary>
		/// Subtracts the milliseconds duration from the specified datetime
		/// </summary>
		/// <param name="date">The source date time</param>
		/// <returns>A new <see cref="DateTime"/> object without its milliseconds bit</returns>
		public static DateTime TrimMilliseconds(this DateTime date)
		{
			return date.Subtract(new TimeSpan(0, 0, 0, 0, date.TimeOfDay.Milliseconds));
		}
		/// <summary>
		/// Subtracts the seconds (and milliseconds) duration duration from the specified datetime
		/// </summary>
		/// <param name="date">The source date time</param>
		/// <returns>A new <see cref="DateTime"/> object without its seconds and milliseconds bit</returns>
		public static DateTime TrimSeconds(this DateTime date)
		{
			return date.Subtract(new TimeSpan(0, 0, 0, date.TimeOfDay.Seconds, date.TimeOfDay.Milliseconds));
		}
	}
}