using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Adenson.Collections
{
	/// <summary>
	/// Behaves like hashtable of old, adds change events
	/// </summary>
	/// <typeparam name="Tk">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="Tv">The type of values in the dictionary.</typeparam>
	public class Hashtable<Tk, Tv> : IDictionary<Tk, Tv>
	{
		#region Variables
		private Dictionary<Tk, Tv> dick;
		private event EventHandler<HashtableChangedEventArgs<Tk>> _dictionaryChanged;
		#endregion
		#region Constructors

		/// <summary>
		/// Instantiates a new hashtable
		/// </summary>
		public Hashtable()
		{
			dick = new Dictionary<Tk, Tv>();
		}
		/// <summary>
		/// Instantiates a new hashtable with specified capacity
		/// </summary>
		/// <param name="capacity"></param>
		public Hashtable(int capacity)
		{
			dick = new Dictionary<Tk, Tv>(capacity);
		}

		#endregion
		#region Properties

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Tv this[Tk key]
		{
			get
			{
				if (dick.ContainsKey(key)) return dick[key];
				return default(Tv);
			}
			set
			{
				if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
				if (dick.ContainsKey(key))
				{
					Tv originalValue = dick[key];
					if ((object)value != (object)originalValue)
					{
						dick[key] = value;
						this.OnDictionaryChanged(new HashtableChangedEventArgs<Tk>(NotifyCollectionChangedAction.Replace, key));
					}
				}
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
		public ICollection<Tk> Keys
		{
			get { return dick.Keys; }
		}
		/// <summary>
		/// 
		/// </summary>
		public ICollection<Tv> Values
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
		/// <summary>
		/// Occurs when a value in the dictionary has changed
		/// </summary>
		public event EventHandler<HashtableChangedEventArgs<Tk>> DictionaryChanged
		{
			add { _dictionaryChanged += value; }
			remove { _dictionaryChanged -= value; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Adds an element with the provided key and value.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value</param>
		public virtual void Add(Tk key, Tv value)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			dick.Add(key, value);
			this.OnDictionaryChanged(new HashtableChangedEventArgs<Tk>(NotifyCollectionChangedAction.Add, key));
		}
		/// <summary>
		/// Removes all items
		/// </summary>
		public virtual void Clear()
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			dick.Clear();
			this.OnDictionaryChanged(new HashtableChangedEventArgs<Tk>(NotifyCollectionChangedAction.Reset, default(Tk)));
		}
		/// <summary>
		/// Determines whether an element with the specified key exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(Tk key)
		{
			return dick.ContainsKey(key);
		}
		/// <summary>
		/// Determines whether the dictionary contains a specific value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ContainsValue(Tv value)
		{
			return dick.ContainsValue(value);
		}
		/// <summary>
		/// Returns an enumerator that iterates through the dictionary
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<Tk, Tv>> GetEnumerator()
		{
			return dick.GetEnumerator();
		}
		/// <summary>
		/// Removes the value with the specified key from the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Remove(Tk key)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			bool result = dick.Remove(key);
			if (result) this.OnDictionaryChanged(new HashtableChangedEventArgs<Tk>(NotifyCollectionChangedAction.Remove, key));
			return result;
		}
		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue(Tk key, out Tv value)
		{
			return dick.TryGetValue(key, out value);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected void OnDictionaryChanged(HashtableChangedEventArgs<Tk> e)
		{
			if (_dictionaryChanged != null) _dictionaryChanged(this, e);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return dick.GetEnumerator();
		}
		void ICollection<KeyValuePair<Tk, Tv>>.Add(KeyValuePair<Tk, Tv> item)
		{
			this.Add(item.Key, item.Value);
		}
		void ICollection<KeyValuePair<Tk, Tv>>.Clear()
		{
			this.Clear();
		}
		bool ICollection<KeyValuePair<Tk, Tv>>.Contains(KeyValuePair<Tk, Tv> item)
		{
			if (!this.ContainsKey(item.Key)) return false;
			if (!this.ContainsValue(item.Value)) return false;
			return Object.Equals(this[item.Key], item.Value);
		}
		void ICollection<KeyValuePair<Tk, Tv>>.CopyTo(KeyValuePair<Tk, Tv>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}
		bool ICollection<KeyValuePair<Tk, Tv>>.Remove(KeyValuePair<Tk, Tv> item)
		{
			return this.Remove(item.Key);
		}

		#endregion
	}
}