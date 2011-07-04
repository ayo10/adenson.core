using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace System
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Does equality comparism of both arrays, if not same instance, then item by item comparism
		/// </summary>
		/// <typeparam name="T">The type of items</typeparam>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		public static bool AreEqualTo<T>(this IEnumerable<T> array1, IEnumerable<T> array2)
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
		/// Determines all the strings in array1 and array2 are both equal (same instance @ same index) using the specified culture, case, and sort rules used in the comparison.
		/// </summary>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		/// <exception cref="ArgumentNullException">if array1 is null or array2 is null and they are both not null.</exception>
		public static bool AreEqualTo(this IEnumerable<string> array1, IEnumerable<string> array2, StringComparison comparisonType)
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
		/// <exception cref="ArgumentNullException">if <paramref name="key"/> is null.</exception>
		public static bool ContainsKey<T>(this Dictionary<string, T> dictionary, string key, StringComparison comparison)
		{
			if (StringUtil.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
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
		/// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
		public static T GetValue<T>(this Dictionary<string, T> dictionary, string key)
		{
			if (key == null) throw new ArgumentNullException("key");
			
			string actualKey = dictionary.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.CurrentCultureIgnoreCase));
			if (actualKey == null) throw new KeyNotFoundException(key);
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
		public static T GetValue<T>(this XElement source, XName name)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (name == null || StringUtil.IsNullOrWhiteSpace(name.LocalName)) throw new ArgumentNullException("name");

			T result = default(T);
			string value = null;

			value = source.Value;

			if (StringUtil.IsNullOrWhiteSpace(value))
			{
				var element = source.Element(name);
				if (element != null) value = element.Value;
			}

			if (StringUtil.IsNullOrWhiteSpace(value))
			{
				var attribute = source.Attribute(name);
				if (attribute != null) value = attribute.Value;
			}

			if (!StringUtil.IsNullOrWhiteSpace(value))
			{
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
		/// Rounds a double-precision floating-point value to a specified number of fractional digits.
		/// </summary>
		/// <remarks>All it does is to call Math.Round(value, digits)</remarks>
		/// <param name="value">A double-precision floating-point number to be rounded.</param>
		/// <param name="digits">The number of fractional digits in the return value.</param>
		/// <returns>The number nearest to value that contains a number of fractional digits equal to digits.</returns>
		/// <exception cref="ArgumentOutOfRangeException">digits is less than 0 or greater than 15.</exception>
		public static double Round(this double value, int digits)
		{
			return Math.Round(value, digits);
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
		/// <remarks>
		/// <para>Any numerals greater than 5000 will begin with the the overbar character, i.e, 5000 = ¯V, 10000 = ¯X, etc, etc</para>
		/// <para>Based on http://www.dreamcubes.com/b2/software-development/21/to-roman-c-int-to-roman-converter/ </para>
		/// </remarks>
		/// <param name="value">The value to convert</param>
		/// <returns>Returns '0', if <paramref name="value"/> was 0 else the value in roman numeral</returns>
		public static string ToRoman(this int value)
		{
			if (value == 0) return String.Empty;
			///if (value > 4999) throw new NotSupportedException("Roman numerals over 4999 are not supported a the current time.");

			var wasNegative = value < 0;
			if (wasNegative) value = value * -1;

			int[] nums = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000, 5000, 10000, 50000, 100000, 500000, 1000000 };
			string[] romanNums = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M", "¯V", "¯X", "¯L", "¯C", "¯D", "¯M" };

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
		/// Subtracts the milliseconds duration from the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="date">The source date.</param>
		/// <returns>A new <see cref="DateTime"/> object without its milliseconds bit</returns>
		public static DateTime Trim(this DateTime date)
		{
			return date.Trim("f");
		}

		/// <summary>
		/// Subtracts the specified bit duration from the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="date">The source date.</param>
		/// <param name="specifier">The date format specifying the bit to be trimmed (i.e. 'ff', 'ss', 'mm')</param>
		/// <returns>A new <see cref="DateTime"/> object without its specified bit.</returns>
		public static DateTime Trim(this DateTime date, string specifier)
		{
			if (StringUtil.IsNullOrWhiteSpace(specifier)) throw new ArgumentNullException("specifier");

			return date.Subtract(date.TimeOfDay.Trim(specifier));
		}

		/// <summary>
		/// Subtracts the milliseconds duration from the specified <see cref="TimeSpan"/> object.
		/// </summary>
		/// <param name="span">The source time.</param>
		/// <returns>A new <see cref="TimeSpan"/> object without its milliseconds bit.</returns>
		public static TimeSpan Trim(this TimeSpan span)
		{
			return span.Subtract(new TimeSpan(0, 0, 0, 0, span.Milliseconds));
		}

		/// <summary>
		/// Subtracts the specified bit duration from the specified <see cref="TimeSpan"/> object.
		/// </summary>
		/// <param name="span">The source date.</param>
		/// <param name="specifier">The date format specifying the bit to be trimmed (i.e. 'ff', 'ss', 'mm')</param>
		/// <returns>A new <see cref="TimeSpan"/> object without its specified bit.</returns>
		public static TimeSpan Trim(this TimeSpan span, string specifier)
		{
			if (StringUtil.IsNullOrWhiteSpace(specifier)) throw new ArgumentNullException("specifier");

			TimeSpan subtract;
			switch (specifier)
			{
				case "f":
				case "ff":
				case "fff":
					subtract = new TimeSpan(0, 0, 0, 0, span.Milliseconds);
					break;
				case "s":
				case "ss":
					subtract = new TimeSpan(0, 0, 0, span.Seconds, span.Milliseconds);
					break;
				case "m":
				case "mm":
					subtract = new TimeSpan(0, 0, span.Minutes, span.Seconds, span.Milliseconds);
					break;
				case "h":
				case "hh":
					subtract = new TimeSpan(0, span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
					break;
				case "d":
				case "dd":
					subtract = new TimeSpan(span.Days, span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
					break;
				default:
					throw new NotSupportedException();
			}
			return span.Subtract(subtract);
		}
	}
}