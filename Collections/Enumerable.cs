using System;
using System.Collections.Generic;

namespace Adenson.Collections
{
	public abstract class Enumerable<T> : IEnumerable<T>
	{
		protected Enumerable()
		{
			this.InnerList = new List<T>();
		}

		public T this[int index]
		{
			get { return this.InnerList[index]; }
		}
		/// <summary>
		/// Gets the number of items in the list
		/// </summary>
		public int Count
		{
			get { return this.InnerList.Count; }
		}
		protected List<T> InnerList
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets if the media reference
		/// </summary>
		/// <param name="medium">The medium to check for</param>
		/// <returns>true, or false</returns>
		public bool Contains(T item)
		{
			return this.InnerList.Contains(item);
		}
		/// <summary>
		/// Gets the Enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}
		/// <summary>
		/// Gets the index of the specified item
		/// </summary>
		/// <param name="item">The item to check for</param>
		/// <returns>The index of, or -1</returns>
		public int IndexOf(T item)
		{
			return this.InnerList.IndexOf(item);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
