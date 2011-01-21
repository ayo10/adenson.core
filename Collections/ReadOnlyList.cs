using System;
using System.Collections;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents a read only list
	/// </summary>
	/// <typeparam name="T">The type of items in the list</typeparam>
	public class ReadOnlyList<T> : IEnumerable<T>
	{
		#region Variables
		private List<T> _list;
		#endregion
		#region Constructors

		/// <summary>
		/// Instantiates a new list
		/// </summary>
		public ReadOnlyList()
		{
			_list = new List<T>();
		}
		/// <summary>
		/// Instantiates a new readonly collection from specified list
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list, the exception being if collection is a .</param>
		/// <exception cref="ArgumentNullException">collection is null.</exception>
		public ReadOnlyList(IEnumerable<T> collection)
		{
			_list = new List<T>(collection);
		}
		
		#endregion
		#region Properties

		/// <summary>
		/// Gets the number of items in the list
		/// </summary>
		public int Count
		{
			get { return _list.Count; }
		}
		/// <summary>
		/// Gets the item at the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <returns>found item if any</returns>
		public T this[int index]
		{
			get { return _list[index]; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Returns if the collection contains specified item
		/// </summary>
		/// <param name="item">The item to look for</param>
		/// <returns>true, or false</returns>
		public bool Contains(T item)
		{
			return _list.Contains(item);
		}
		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the list.
		/// </summary>
		/// <param name="item">The object to locate</param>
		/// <returns> The zero-based index of the first occurrence of item within the entire list, if found; otherwise, –1.</returns>
		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}
		/// <summary>
		/// Gets the enumertor
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}