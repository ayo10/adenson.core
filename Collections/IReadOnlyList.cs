using System;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents what a collection of IVirtualObject objects will be like
	/// </summary>
	public interface IReadOnlyList<T> : IEnumerable<T>
	{
		/// <summary>
		/// Gets the number of elements contained in the collection
		/// </summary>
		int Count { get; }
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		T this[int index] { get; }
		/// <summary>
		/// Gets if the list contains the specified item 
		/// </summary>
		/// <param name="item">The item to look for</param>
		/// <returns>true if the item exists, false otherwise</returns>
		bool Contains(T item);
		/// <summary>
		/// Sorts the list using .NET default comparer
		/// </summary>
		void Sort();
		/// <summary>
		/// Sorts the list using the comparer
		/// </summary>
		/// <param name="comparer">The comparer to use to sort the list</param>
		void Sort(IComparer<T> comparer);
	}
}