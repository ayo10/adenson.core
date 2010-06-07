using System;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents a strongly typed list of objects that can be accessed by index.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	public class UnsyncedList<T> : IList<T>
	{
		#region Variables
		private System.Threading.ReaderWriterLock readWriteLock = new System.Threading.ReaderWriterLock();
		private List<T> list;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the list class that is empty and has the default initial capacity.
		/// </summary>
		public UnsyncedList()
		{
			list = new List<T>();
		}
		/// <summary>
		/// Initializes a new instance of the list class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.</param>
		public UnsyncedList(IEnumerable<T> collection)
		{
			list = new List<T>(collection);
		}
		/// <summary>
		/// Initializes a new instance of the list class that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new list can initially store.</param>
		public UnsyncedList(int capacity)
		{
			list = new List<T>(capacity);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public T this[int index]
		{
			get
			{
				readWriteLock.AcquireReaderLock(-1);
				try
				{
					return list[index];
				}
				finally
				{
					readWriteLock.ReleaseReaderLock();
				}
			}
			set
			{
				readWriteLock.AcquireWriterLock(-1);
				try
				{
					list[index] = value;
				}
				finally
				{
					readWriteLock.ReleaseWriterLock();
				}
			}
		}
		/// <summary>
		/// Gets the number of elements actually contained in the list.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}
		/// <summary>
		/// Gets if the list is readonly
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Adds an object to the end of the list.
		/// </summary>
		/// <param name="item">The object to be added to the end of the list.</param>
		public void Add(T item)
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				list.Add(item);
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Removes all elements from the list.
		/// </summary>
		public void Clear()
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				list.Clear();
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Determines whether an element is in the list.
		/// </summary>
		/// <param name="item">The object to locate in the list.</param>
		/// <returns>true if item is found in the list; false otherwise.</returns>
		public bool Contains(T item)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.Contains(item);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Copies the entire list to a compatible one-dimensional array.
		/// </summary>
		/// <param name="array">The one-dimensional System.Array that is the destination of the elements.</param>
		public void CopyTo(T[] array)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				list.CopyTo(array);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Copies the entire list to a compatible one-dimensional array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional System.Array that is the destination of the elements.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				list.CopyTo(array, arrayIndex);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Copies a range of elements from the list to a compatible one-dimensional array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="index">The zero-based index in the source System.Collections.Generic.List<T> at which copying begins.</param>
		/// <param name="array">The one-dimensional System.Array that is the destination of the elements.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// <param name="count">The number of elements to copy.</param>
		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				list.CopyTo(index, array, arrayIndex, count);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Determines whether the list contains elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The predicate delegate that defines the conditions of the elements to search for.</param>
		/// <returns>true if the list contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
		/// <exception name="System.ArgumentNullException">match is null.</exception>
		public bool Exists(Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.Exists(match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire list.
		/// </summary>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for.</param>
		/// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.
		/// <exception name="System.ArgumentNullException">match is null.</exception>
		public T Find(Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.Find(match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The predicate delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A list containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty list.</returns>
		/// <exception name="ArgumentNullException">match is null.</exception>
		public List<T> FindAll(Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindAll(match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire list.
		/// </summary>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		/// <exception name="System.ArgumentNullException">match is null.</exception>
		public int FindIndex(Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindIndex(match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the list that extends from the specified index to the last element.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the search.</param>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		/// <exception name="ArgumentNullException">match is null.</exception>
		/// <exception name="ArgumentOutOfRangeException">startIndex is outside the range of valid indexes for the list.</exception>
		public int FindIndex(int startIndex, Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindIndex(startIndex, match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the list that starts at the specified index and contains the specified number of elements.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the search.</param>
		/// <param name="count">The number of elements in the section to search.</param>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the  conditions defined by match, if found; otherwise, -1.</returns>
		/// <exception name="ArgumentNullException">match is null.</exception>
		/// <exception name="ArgumentOutOfRangeException">startIndex is outside the range of valid indexes for the list.-or-count is less than 0.-or-startIndex and count do not specify a valid section in the list.</exception>
		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindIndex(startIndex, count, match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire list.
		/// <summary>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for.</param>
		/// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
		public T FindLast(Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindLast(match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire list.
		/// </summary>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for</param>
		/// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		/// <exception name="ArgumentNullException">match is null.</exception>
		public int FindLastIndex(Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindLastIndex(match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified
		/// predicate, and returns the zero-based index of the last occurrence within
		/// the range of elements in the list that extends
		/// from the first element to the specified index.
		/// </summary>
		/// <param name="startIndex">he zero-based starting index of the backward search.</param>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for</param>
		/// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindLastIndex(startIndex, match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the list that contains the specified number of elements and ends at the specified index.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the backward search.</param>
		/// <param name="count">The number of elements in the section to search.</param>
		/// <param name="match">The predicate delegate that defines the conditions of the element to search for</param>
		/// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		/// <exception name="ArgumentNullException">match is null.</exception>
		/// <exception name="ArgumentOutOfRangeException">startIndex is outside the range of valid indexes for the list.-or-count is less than 0.-or-startIndex and count do not specify a valid section in the list.</exception>
		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				return list.FindLastIndex(startIndex, count, match);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Performs the specified action on each element of the list.
		/// </summary>
		/// <param name="action">The action delegate to perform on each element of the list.</param>
		/// <exception name="ArgumentNullException">action is null.</exception>
		public void ForEach(Action<T> action)
		{
			readWriteLock.AcquireReaderLock(-1);
			try
			{
				list.ForEach(action);
			}
			finally
			{
				readWriteLock.ReleaseReaderLock();
			}
		}
		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the entire list.
		/// </summary>
		/// <param name="item">The object to locate in the list.</param>
		/// <returns>The zero-based index of the first occurrence of item within the entire list</returns>
		public int IndexOf(T item)
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				return list.IndexOf(item);
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Inserts an element into the list at the specified index
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		public void Insert(int index, T item)
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				list.Insert(index, item);
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Removes the first occurrence of a specific object from the list.
		/// </summary>
		/// <param name="item">The object to remove from the list.</param>
		/// <returns>true if item is successfully removed; otherwise, false.</returns>
		public bool Remove(T item)
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				return list.Remove(item);
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Removes the element at the specified index of the list.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		public void RemoveAt(int index)
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				list.RemoveAt(index);
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Reverses the order of the elements in the entire list.
		/// </summary>
		public void Reverse()
		{
			list.Reverse();
		}
		/// <summary>
		/// Reverses the order of the elements in the specified range.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to reverse.</param>
		/// <param name="count">The number of elements in the range to reverse.</param>
		/// <exception cref="ArgumentOutOfRangeException">index is less than 0.-or-count is less than 0.</exception>
		/// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
		public void Reverse(int index, int count)
		{
			list.Reverse(index, count);
		}
		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List&lt;T> using the default comparer.
		/// </summary>
		public void Sort()
		{
			list.Sort();
		}
		/// <summary>
		/// Sorts the elements in the entire list using the specified Comparison.
		/// </summary>
		/// <param name="comparison">The System.Comparison&lt;T> to use when comparing elements.</param>
		public void Sort(Comparison<T> comparison)
		{
			list.Sort(comparison);
		}
		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List&lt;T> using the specified comparer.
		/// </summary>
		/// <param name="comparer">The System.Collections.Generic.IComparer&lt;T> implementation to use when comparing elements.</param>
		public void Sort(IComparer<T> comparer)
		{
			list.Sort(comparer);
		}
		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List<T> using the specified comparer.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to sort.</param>
		/// <param name="count">The length of the range to sort.</param>
		/// <param name="comparer">The System.Collections.Generic.IComparer<T> implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.</param>
		public void Sort(int index, int count, IComparer<T> comparer)
		{
			list.Sort(index, count, comparer);
		}
		/// <summary>
		/// Copies the elements of the list to a new array.
		/// </summary>
		/// <returns>An array containing copies of the elements of the list.</returns>
		public T[] ToArray()
		{
			return list.ToArray();
		}
		/// <summary>
		/// Returns an enumerator that iterates through the list.
		/// </summary>
		/// <returns>A list.Enumerator for the list.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			readWriteLock.AcquireWriterLock(-1);
			try
			{
				return list.GetEnumerator();
			}
			finally
			{
				readWriteLock.ReleaseWriterLock();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}