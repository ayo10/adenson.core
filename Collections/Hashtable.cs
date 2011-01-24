using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Adenson.Collections
{
	/// <summary>
	/// Behaves like hashtable of old, adds change events
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	public class Hashtable<TKey, TValue> : IDictionary<TKey, TValue>
	{
		#region Variables
		private Dictionary<TKey, TValue> dick;
		#endregion
		#region Constructors

		/// <summary>
		/// Instantiates a new hashtable
		/// </summary>
		public Hashtable()
		{
			dick = new Dictionary<TKey, TValue>();
		}
		/// <summary>
		/// Instantiates a new hashtable with specified capacity
		/// </summary>
		/// <param name="capacity"></param>
		public Hashtable(int capacity)
		{
			dick = new Dictionary<TKey, TValue>(capacity);
		}

		#endregion
		#region Properties

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TValue this[TKey key]
		{
			get
			{
				if (dick.ContainsKey(key)) return dick[key];
				return default(TValue);
			}
			set
			{
				if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
				if (dick.ContainsKey(key)) dick[key] = value;
				else dick.Add(key, value);
			}
		}
		/// <summary>
		/// Gets the number of items in the dictionary
		/// </summary>
		public int Count
		{
			get { return dick.Count; }
		}
		/// <summary>
		/// 
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return dick.Keys; }
		}
		/// <summary>
		/// 
		/// </summary>
		public ICollection<TValue> Values
		{
			get { return dick.Values; }
		}
		/// <summary>
		/// Gets if the dictionary is read only
		/// </summary>
		public virtual bool IsReadOnly
		{
			get;
			internal set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Adds an element with the provided key and value.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value</param>
		public virtual void Add(TKey key, TValue value)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			dick.Add(key, value);
		}
		/// <summary>
		/// Removes all items
		/// </summary>
		public virtual void Clear()
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			dick.Clear();
		}
		/// <summary>
		/// Determines whether an element with the specified key exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(TKey key)
		{
			return dick.ContainsKey(key);
		}
		/// <summary>
		/// Determines whether the dictionary contains a specific value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ContainsValue(TValue value)
		{
			return dick.ContainsValue(value);
		}
		/// <summary>
		/// Returns an enumerator that iterates through the dictionary
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return dick.GetEnumerator();
		}
		/// <summary>
		/// Removes the value with the specified key from the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Remove(TKey key)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			bool result = dick.Remove(key);
			return result;
		}
		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return dick.TryGetValue(key, out value);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return dick.GetEnumerator();
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			this.Clear();
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			if (!this.ContainsKey(item.Key)) return false;
			if (!this.ContainsValue(item.Value)) return false;
			return Object.Equals(this[item.Key], item.Value);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.Remove(item.Key);
		}

		#endregion
	}
}