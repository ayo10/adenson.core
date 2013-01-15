using System;
using System.Linq;

namespace System.Collections.Generic
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Adds the value with the specified key if the key exists already in the dictionary, else replaces the key's current value with the specified <paramref name="value"/>
		/// </summary>
		/// <typeparam name="TKey">The type of key</typeparam>
		/// <typeparam name="TValue">The type of value</typeparam>
		/// <param name="dictionary">The dictionary to parse</param>
		/// <param name="key">The key to find</param>
		/// <param name="value">The value to set</param>
		public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// Determines whether the dictionary contains the specified key, using the specified comparism rule.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="dictionary">The dictionary to parse.</param>
		/// <param name="key">The key to find</param>
		/// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
		/// <returns>true if the value of the value parameter is the same as this string; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="key"/> is null.</exception>
		public static bool ContainsKey<T>(this IDictionary<string, T> dictionary, string key, StringComparison comparison)
		{
			if (StringUtil.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException("key");
			}

			return dictionary.Keys.Any(k => k.Equals(key, comparison));
		}

		/// <summary>
		/// Determines all the items in array1 and array2 are both equal (same instance at the same index).
		/// </summary>
		/// <typeparam name="T">The type of items</typeparam>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		public static bool SameAs<T>(this IEnumerable<T> array1, IEnumerable<T> array2)
		{
			if (Object.ReferenceEquals(array1, array2))
			{
				return true;
			}

			if ((array1 == null && array2 != null) || (array1 != null && array2 == null))
			{
				return false;
			}

			if (array1.Count() != array2.Count())
			{
				return false;
			}

			for (int i = 0; i < array1.Count(); i++)
			{
				T e1 = array1.ElementAt(i);
				T e2 = array2.ElementAt(i);
				if (!Object.Equals(e1, e2))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Determines all the strings in array1 and array2 are both equal (same instance at the same index) using the specified comparison type.
		/// </summary>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		/// <exception cref="ArgumentNullException">If array1 is null or array2 is null and they are both not null.</exception>
		public static bool SameAs(this IEnumerable<string> array1, IEnumerable<string> array2, StringComparison comparisonType)
		{
			if (Object.ReferenceEquals(array1, array2))
			{
				return true;
			}

			if ((array1 == null && array2 != null) || (array1 != null && array2 == null))
			{
				return false;
			}

			if (array1.Count() != array2.Count())
			{
				return false;
			}

			for (int i = 0; i < array1.Count(); i++)
			{
				if (!String.Equals(array1.ElementAt(i), array2.ElementAt(i), comparisonType))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets the element with the specified string key, using specified comparison type
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="dictionary">The dictionary to parse</param>
		/// <param name="key">The key to find</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a System.Collections.Generic.KeyNotFoundException, and a set operation creates a new element with the specified key.</returns>
		/// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
		public static T GetValue<T>(this IDictionary<string, T> dictionary, string key, StringComparison comparisonType)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			string actualKey = dictionary.Keys.FirstOrDefault(k => k.Equals(key, comparisonType));
			if (actualKey == null)
			{
				throw new KeyNotFoundException(key);
			}

			return dictionary[actualKey];
		}

		/// <summary>
		/// Checks to see if the specified value is empty or null
		/// </summary>
		/// <typeparam name="T">The type of items in the enumeration.</typeparam>
		/// <param name="values">The enumerable object to check</param>
		/// <returns>true if values is null or empty, false otherwise</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
		{
			return (values == null) || (values.Count() == 0);
		}
	}
}