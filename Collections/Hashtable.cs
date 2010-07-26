using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Adenson.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="Tkey"></typeparam>
	/// <typeparam name="Tvalue"></typeparam>
	public class Hashtable<Tkey, Tvalue> : IDictionary<Tkey, Tvalue>
	{
		#region Variables
		private Dictionary<Tkey, Tvalue> dick;
		#endregion
		#region Constructors

		public Hashtable()
		{
			dick = new Dictionary<Tkey, Tvalue>();
		}
		public Hashtable(int capacity)
		{
			dick = new Dictionary<Tkey, Tvalue>(capacity);
		}

		#endregion
		#region Properties

		public virtual Tvalue this[Tkey key]
		{
			get
			{
				if (dick.ContainsKey(key)) return dick[key];
				return default(Tvalue);
			}
			set
			{
				if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
				if (dick.ContainsKey(key))
				{
					Tvalue originalValue = dick[key];
					if ((object)value != (object)originalValue)
					{
						dick[key] = value;
						this.OnDictionaryChanged(new HashtableChangedEventArgs<Tkey>(NotifyCollectionChangedAction.Replace, key));
					}
				}
				else dick.Add(key, value);
			}
		}
		public int Count
		{
			get { return dick.Count; }
		}
		public ICollection<Tkey> Keys
		{
			get { return dick.Keys; }
		}
		public ICollection<Tvalue> Values
		{
			get { return dick.Values; }
		}
		public virtual bool IsReadOnly
		{
			get;
			internal set;
		}
		public event EventHandler<HashtableChangedEventArgs<Tkey>> DictionaryChanged;

		#endregion
		#region Methods

		public virtual void Add(Tkey key, Tvalue value)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			dick.Add(key, value);
			this.OnDictionaryChanged(new HashtableChangedEventArgs<Tkey>(NotifyCollectionChangedAction.Add, key));
		}
		public virtual void Clear()
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			dick.Clear();
			this.OnDictionaryChanged(new HashtableChangedEventArgs<Tkey>(NotifyCollectionChangedAction.Reset));
		}
		public bool ContainsKey(Tkey key)
		{
			return dick.ContainsKey(key);
		}
		public bool ContainsValue(Tvalue value)
		{
			return dick.ContainsValue(value);
		}
		public IEnumerator<KeyValuePair<Tkey, Tvalue>> GetEnumerator()
		{
			return dick.GetEnumerator();
		}
		public bool Remove(Tkey key)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			bool result = dick.Remove(key);
			if (result) this.OnDictionaryChanged(new HashtableChangedEventArgs<Tkey>(NotifyCollectionChangedAction.Remove, key));
			return result;
		}
		public bool TryGetValue(Tkey key, out Tvalue value)
		{
			return dick.TryGetValue(key, out value);
		}
		protected void OnDictionaryChanged(HashtableChangedEventArgs<Tkey> e)
		{
			if (this.DictionaryChanged != null) this.DictionaryChanged(this, e);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return dick.GetEnumerator();
		}
		void ICollection<KeyValuePair<Tkey, Tvalue>>.Add(KeyValuePair<Tkey, Tvalue> item)
		{
			this.Add(item.Key, item.Value);
		}
		void ICollection<KeyValuePair<Tkey, Tvalue>>.Clear()
		{
			this.Clear();
		}
		bool ICollection<KeyValuePair<Tkey, Tvalue>>.Contains(KeyValuePair<Tkey, Tvalue> item)
		{
			if (!this.ContainsKey(item.Key)) return false;
			if (!this.ContainsValue(item.Value)) return false;
			return Object.Equals(this[item.Key], item.Value);
		}
		void ICollection<KeyValuePair<Tkey, Tvalue>>.CopyTo(KeyValuePair<Tkey, Tvalue>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}
		bool ICollection<KeyValuePair<Tkey, Tvalue>>.Remove(KeyValuePair<Tkey, Tvalue> item)
		{
			return this.Remove(item.Key);
		}

		#endregion
	}
}