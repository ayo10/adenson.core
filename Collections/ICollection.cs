using System;

namespace Adenson.Collections
{
	/// <summary>
	/// Overriden System.Collections.Generic.ICollection&lt;T&gt; with indexer
	/// </summary>
	/// <typeparam name="T">The type of object the collection will contain.</typeparam>
	public interface ICollection<T> : System.Collections.Generic.ICollection<T>
	{
		/// <summary>
		/// Gets the object at specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <returns>Found (T) object if any</returns>
		T this[int index] { get; }
	}
}