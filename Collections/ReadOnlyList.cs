using System;
using System.Collections;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents a read only collection
	/// </summary>
	/// <typeparam name="T"></typeparam>
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
			List<T> list = collection as List<T>;
			if (list != null) _list = list;
			else _list = new List<T>(collection);
		}
		
		#endregion
		#region Properties

		/// <summary>
		/// Gets the number of items in the list
		/// </summary>
		public int Count
		{
			get { return this.InnerList.Count; }
		}
		/// <summary>
		/// Gets the item at the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <returns>found item if any</returns>
		public T this[int index]
		{
			get { return this.InnerList[index]; }
		}
		/// <summary>
		/// Gets the base list, and its not overridable
		/// </summary>
		protected List<T> InnerList
		{
			get { return _list; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Returns if the collection contains specified item
		/// </summary>
		/// <param name="item">The item to look for</param>
		/// <returns>true, or false</returns>
		public virtual bool Contains(T item)
		{
			return this.InnerList.Contains(item);
		}
		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the list.
		/// </summary>
		/// <param name="item">The object to locate</param>
		/// <returns> The zero-based index of the first occurrence of item within the entire list, if found; otherwise, –1.</returns>
		public int IndexOf(T item)
		{
			return this.InnerList.IndexOf(item);
		}
		/// <summary>
		/// Gets the enumertor
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator<T> GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}
		/// <summary>
		/// Sorts the list using .NET default comparer
		/// </summary>
		public virtual void Sort()
		{
			this.InnerList.Sort();
		}
		/// <summary>
		/// Sorts the list using the comparer
		/// </summary>
		/// <param name="comparer">The comparer to use to sort the list</param>
		public virtual void Sort(IComparer<T> comparer)
		{
			this.InnerList.Sort(comparer);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}

		#endregion
	}
}