using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents a collection of keys and values.
	/// </summary>
	/// <remarks>
	/// Behaves like System.Collections.Hashtable of old.
	/// </remarks>
	/// <typeparam name="K">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="V">The type of values in the dictionary.</typeparam>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "No, I want the name Hashtable, NOT WhateverDictionary")]
	public class Hashtable<K, V> : IDictionary<K, V>
	{
		#region Variables
		private IDictionary<K, V> dick;
		#endregion
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Hashtable class.
		/// </summary>
		public Hashtable()
		{
			dick = new Dictionary<K, V>();
		}

		/// <summary>
		/// Initializes a new instance of the Hashtable class with specified capacity
		/// </summary>
		/// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
		public Hashtable(int capacity)
		{
			dick = new Dictionary<K, V>(capacity);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the number of items in the dictionary
		/// </summary>
		public int Count
		{
			get { return dick.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the dictionary is read only
		/// </summary>
		public bool IsReadOnly
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets all the keys in the hashtable
		/// </summary>
		public ICollection<K> Keys
		{
			get { return dick.Keys; }
		}

		/// <summary>
		/// Gets all the values in the hashtable
		/// </summary>
		public ICollection<V> Values
		{
			get { return dick.Values; }
		}

		/// <summary>
		/// Gets or sets the value at the specified item
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>Found value</returns>
		public V this[K key]
		{
			get
			{
				if (dick.ContainsKey(key))
				{
					return dick[key];
				}

				return default(V);
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
				}

				if (dick.ContainsKey(key))
				{
					dick[key] = value;
				}
				else
				{
					dick.Add(key, value);
				}
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Adds an element with the provided key and value.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value</param>
		public void Add(K key, V value)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			}

			dick.Add(key, value);
		}

		/// <summary>
		/// Removes all items
		/// </summary>
		public void Clear()
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			}

			dick.Clear();
		}

		/// <summary>
		/// Copies the elements of the hashtable to the specified array, starting at the specified index.
		/// </summary>
		/// <param name="array">The one-dimensional System.Array that is the destination of the elements.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			dick.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Determines whether the hashtable contains the specified item.
		/// </summary>
		/// <param name="item">The object to locate.</param>
		/// <returns>true if item is found; otherwise, false.</returns>
		public bool Contains(KeyValuePair<K, V> item)
		{
			return dick.Contains(item);
		}

		/// <summary>
		/// Determines whether an element with the specified key exists.
		/// </summary>
		/// <param name="key">The key to locate</param>
		/// <returns>true if the hashtable contains an element with the key; otherwise, false.</returns>
		public bool ContainsKey(K key)
		{
			return dick.ContainsKey(key);
		}

		/// <summary>
		/// Determines whether the dictionary contains a specific value.
		/// </summary>
		/// <param name="value">The value</param>
		/// <returns>true if the hashtable contains an element with the value; otherwise, false.</returns>
		public bool ContainsValue(V value)
		{
			return dick.Values.Contains(value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the dictionary
		/// </summary>
		/// <returns>The enumerator</returns>
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return dick.GetEnumerator();
		}

		/// <summary>
		/// Removes the value with the specified key from the dictionary
		/// </summary>
		/// <param name="key">The key to locate</param>
		/// <returns>true if the item was removed, false otherwise.</returns>
		public bool Remove(K key)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			}

			bool result = dick.Remove(key);
			return result;
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value found if any</param>
		/// <returns>true, or false</returns>
		public bool TryGetValue(K key, out V value)
		{
			return dick.TryGetValue(key, out value);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return dick.GetEnumerator();
		}

		void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
		{
			this.Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<K, V>>.Clear()
		{
			this.Clear();
		}

		bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
		{
			return this.Remove(item.Key);
		}

		#endregion
	}
}