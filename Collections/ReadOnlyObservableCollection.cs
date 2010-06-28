using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ListenQuest
{
	public sealed class ReadOnlyObservableCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IList
	{
		#region Variables
		private ObservableCollection<T> BackingList = new ObservableCollection<T>();
		#endregion
		#region Constructors

		public ReadOnlyObservableCollection()
		{
			new TimeSpan();
		}
		public ReadOnlyObservableCollection(IEnumerable<T> collection)
		{
			this.BackingList = new ObservableCollection<T>(collection);
		}
		internal ReadOnlyObservableCollection(ObservableCollection<T> collection)
		{
			this.BackingList = collection;
		}

		#endregion
		#region Properties

		public T this[int index]
		{
			get { return this.BackingList[index]; }
		}
		public int Count
		{
			get { return this.BackingList.Count; }
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { this.BackingList.CollectionChanged += value; }
			remove { this.BackingList.CollectionChanged -= value; }
		}
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { this.BackingList.AddPropertyChangedEvent(value); }
			remove { this.BackingList.RemovePropertyChangedEvent(value); }
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { throw new NotSupportedException(); }
		}
		bool ICollection.IsSynchronized
		{
			get { throw new NotSupportedException(); }
		}
		object ICollection.SyncRoot
		{
			get { return ((ICollection)this.BackingList).SyncRoot; }
		}
		bool IList.IsFixedSize
		{
			get { throw new NotSupportedException(); }
		}
		bool IList.IsReadOnly
		{
			get { throw new NotSupportedException(); }
		}

		#endregion
		#region Methods

		public bool Contains(T item)
		{
			return this.BackingList.Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.BackingList.CopyTo(array, arrayIndex);
		}
		public IEnumerator<T> GetEnumerator()
		{
			return this.BackingList.GetEnumerator();
		}

		internal void Add(T item)
		{
			this.BackingList.Add(item);
		}
		internal void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items) this.Add(item);
		}
		internal void Clear()
		{
			this.BackingList.Clear();
		}
		internal bool Remove(T item)
		{
			return this.BackingList.Remove(item);
		}
		internal void RemoveRange(IEnumerable<T> items)
		{
			foreach (T item in items) this.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		void ICollection<T>.Add(T item)
		{
			throw new NotImplementedException();
		}
		void ICollection<T>.Clear()
		{
			throw new NotImplementedException();
		}
		bool ICollection<T>.IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}
		bool ICollection<T>.Remove(T item)
		{
			throw new NotImplementedException();
		}
		int IList.Add(object value)
		{
			throw new NotImplementedException();
		}
		void IList.Clear()
		{
			throw new NotImplementedException();
		}
		bool IList.Contains(object value)
		{
			throw new NotImplementedException();
		}
		int IList.IndexOf(object value)
		{
			throw new NotImplementedException();
		}
		void IList.Insert(int index, object value)
		{
			throw new NotImplementedException();
		}
		void IList.Remove(object value)
		{
			throw new NotImplementedException();
		}
		void IList.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}
		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}