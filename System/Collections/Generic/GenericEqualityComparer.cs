using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
	/// <summary>
	/// A comparer using provided func delegate.
	/// </summary>
	/// <typeparam name="T">Type of object to compare.</typeparam>
	public class GenericEqualityComparer<T> : IEqualityComparer<T>
	{
		#region Variables
		private Func<T, T, bool> _func;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the GenericEqualityComparer class using specified compare function delegate.
		/// </summary>
		/// <param name="func">The compare delegate.</param>
		public GenericEqualityComparer(Func<T, T, bool> func)
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
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>Returns true if the specified objects are equal; otherwise, false.</returns>
		public bool Equals(T x, T y)
		{
			return _func(x, y);
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <param name="obj">The System.Object for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}

		#endregion
	}
}