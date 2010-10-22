using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Adenson.Collections
{
	/// <summary>
	/// A read only but observable collection
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ReadOnlyObservableCollection<T> : IEnumerable<T>, INotifyPropertyChanged
	{
		#region Variables
		private ObservableCollection<T> BackingList = new ObservableCollection<T>();
		#endregion
		#region Constructors

		/// <summary>
		/// Instantiates a new read only from specified collection
		/// </summary>
		/// <param name="collection"></param>
		public ReadOnlyObservableCollection(IEnumerable<T> collection)
		{
			var list = collection as ObservableCollection<T>;
			this.BackingList = list == null ? new ObservableCollection<T>(collection) : list;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		public T this[int index]
		{
			get { return this.BackingList[index]; }
		}
		/// <summary>
		/// Gets the number of elements actually contained in the list
		/// </summary>
		public int Count
		{
			get { return this.BackingList.Count; }
		}
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { this.BackingList.AddPropertyChangedEvent(value); }
			remove { this.BackingList.RemovePropertyChangedEvent(value); }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Determines whether an element is in the list
		/// </summary>
		/// <param name="item">The object to locate</param>
		/// <returns>true if item is found in the list otherwise, false.</returns>
		public bool Contains(T item)
		{
			return this.BackingList.Contains(item);
		}
		/// <summary>
		/// Copies the entire list to a compatible one-dimensional System.Array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional System.Array that is the destination of the elements</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.BackingList.CopyTo(array, arrayIndex);
		}
		/// <summary>
		/// Returns an enumerator that iterates through the list.
		/// </summary>
		/// <returns>An enumerator for the list</returns>
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

		#endregion
	}
}