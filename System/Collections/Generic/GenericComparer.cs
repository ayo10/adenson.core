using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
	/// <summary>
	/// A comparer using provided func delegate.
	/// </summary>
	/// <typeparam name="T">Type of object to compare.</typeparam>
	public class GenericComparer<T> : IComparer<T>
	{
		#region Variables
		private Func<T, T, int> _func;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the GenericComparer class using specified compare function delegate.
		/// </summary>
		/// <param name="func">The compare delegate.</param>
		public GenericComparer(Func<T, T, int> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}

			_func = func;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>A signed integer that indicates the relative values of x and y, as shown in the following table.Value Meaning Less than zerox is less than y.Zerox equals y.Greater than zerox is greater than y.</returns>
		public int Compare(T x, T y)
		{
			return _func(x, y);
		}

		#endregion
	}
}