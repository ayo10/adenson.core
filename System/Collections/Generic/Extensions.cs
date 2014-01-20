using System;
using System.Linq;

namespace System.Collections.Generic
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		#region Methods

		/// <summary>
		/// Adds the value with the specified key if the key exists already in the dictionary, else replaces the key's current value with the specified <paramref name="value"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="dictionary">The dictionary to parse.</param>
		/// <param name="key">The key to find.</param>
		/// <param name="value">The value to set.</param>
		public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

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
		/// Gets the element with the specified key if it exists in the dictionary, else returns <paramref name="returnIfNull"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="dictionary">The dictionary to read.</param>
		/// <param name="key">The key to find.</param>
		/// <param name="returnIfNotContained">The value to return if no such key exists in the dictionary.</param>
		/// <returns>Found value if any else added <paramref name="returnIfNull"/>.</returns>
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue returnIfNotContained)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			TValue value;
			if (dictionary.TryGetValue(key, out value))
			{
				return value;
			}

			return returnIfNotContained;
		}

		/// <summary>
		/// Gets the element with the specified string key, using specified comparison type.
		/// </summary>
		/// <typeparam name="TValue">The type.</typeparam>
		/// <param name="dictionary">The dictionary to parse.</param>
		/// <param name="key">The key to find.</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a System.Collections.Generic.KeyNotFoundException, and a set operation creates a new element with the specified key.</returns>
		/// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
		public static TValue Get<TValue>(this IDictionary<string, TValue> dictionary, string key, StringComparison comparisonType)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

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
		/// Gets the element with the specified key if it exists in the dictionary, else adds <paramref name="addIfNull"/> to the dictionary with the specified key and returns it.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="dictionary">The dictionary to read.</param>
		/// <param name="key">The key to find.</param>
		/// <param name="addIfNull">The value to add and return if no such key exists in the dictionary.</param>
		/// <returns>Found value if any else added <paramref name="addIfNull"/>.</returns>
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue addIfNull)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			TValue value;
			if (!dictionary.TryGetValue(key, out value))
			{
				value = addIfNull;
				dictionary.Add(key, addIfNull);
			}

			return value;
		}

		/// <summary>
		/// Gets the element with the specified key, else returns the default of <typeparamref name="TValue"/>.
		/// </summary>
		/// <typeparam name="TKey">The type of key.</typeparam>
		/// <typeparam name="TValue">The type of value.</typeparam>
		/// <param name="dictionary">The dictionary to read.</param>
		/// <param name="key">The key to find.</param>
		/// <returns>Found value if any, else default of <typeparamref name="TValue"/>.</returns>
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			return dictionary.Get(key, default(TValue));
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
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			if (StringUtil.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException("key");
			}

			return dictionary.Keys.Any(k => k.Equals(key, comparison));
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

		/// <summary>
		/// Merges the content of <paramref name="value"/> with the contents of <paramref name="other"/>, returning a new <see cref="IEnumerable"/> instance.
		/// </summary>
		/// <typeparam name="T">The type of items.</typeparam>
		/// <param name="value">The value to merge.</param>
		/// <param name="other">The other value to merge.</param>
		/// <returns>A new <see cref="IEnumerable"/> instance containing contents of <paramref name="value"/> and <paramref name="other"/>.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="other"/> is null.</exception>
		public static T[] MergeWith<T>(this T[] value, T[] other)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if (other == null)
			{
				throw new ArgumentNullException("other");
			}

			T[] result = new T[value.Length + other.Length];
			value.CopyTo(result, 0);
			other.CopyTo(result, value.Length);
			return result;
		}

		/// <summary>
		/// Merges the content of <paramref name="value"/> with the contents of <paramref name="other"/>, returning a new <see cref="IEnumerable"/> instance.
		/// </summary>
		/// <typeparam name="T">The type of items.</typeparam>
		/// <param name="value">The value to merge.</param>
		/// <param name="other">The other value to merge.</param>
		/// <returns>A new <see cref="IEnumerable"/> instance containing contents of <paramref name="value"/> and <paramref name="other"/>.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="other"/> is null.</exception>
		public static IEnumerable<T> MergeWith<T>(this IEnumerable<T> value, IEnumerable<T> other)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if (other == null)
			{
				throw new ArgumentNullException("other");
			}

			MergeEnumerable<T> list = new MergeEnumerable<T>();
			list.AddRange(value);
			list.AddRange(other);
			return list;
		}

		#endregion
		#region Inner Class

		private class MergeEnumerable<T> : List<T>
		{
		}

		#endregion
	}
}