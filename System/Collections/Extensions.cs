using System;
using System.Linq;

namespace System.Collections
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		#region Methods

		/// <summary>
		/// Does equality comparism of both arrays, first by instance, then item by item comparism.
		/// </summary>
		/// <param name="array1">The frst array.</param>
		/// <param name="array2">The second array.</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise.</returns>
		public static bool IsEquivalentTo(this IEnumerable array1, IEnumerable array2)
		{
			if (Object.ReferenceEquals(array1, array2))
			{
				return true;
			}

			if ((array1 == null && array2 != null) || (array1 != null && array2 == null))
			{
				return false;
			}

			var a1 = array1.Cast<object>();
			var a2 = array2.Cast<object>();
			if (a1.Count() != a2.Count())
			{
				return false;
			}

			for (int i = 0; i < a1.Count(); i++)
			{
				object e1 = a1.ElementAt(i);
				object e2 = a2.ElementAt(i);
				if (!Object.Equals(e1, e2))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Checks to see if the specified value is empty or null
		/// </summary>
		/// <param name="values">The enumerable object to check</param>
		/// <returns>true if values is null or empty, false otherwise</returns>
		public static bool IsNullOrEmpty(this IEnumerable values)
		{
			return (values == null) || (values.Cast<object>().Count() == 0);
		}

		/// <summary>
		/// Merges the content of <paramref name="value"/> with the contents of <paramref name="other"/>, returning a new <see cref="IEnumerable"/> instance.
		/// </summary>
		/// <param name="value">The value to merge.</param>
		/// <param name="other">The other value to merge.</param>
		/// <returns>A new <see cref="IEnumerable"/> instance containing contents of <paramref name="value"/> and <paramref name="other"/>.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="other"/> is null.</exception>
		public static IEnumerable MergeWith(this IEnumerable value, IEnumerable other)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if (other == null)
			{
				throw new ArgumentNullException("other");
			}

			MergeEnumerable list = new MergeEnumerable();
			foreach (object item in value)
			{
				list.Add(item);
			}

			foreach (object item in other)
			{
				list.Add(item);
			}

			return list;
		}

		#endregion
		#region Inner Class

		private class MergeEnumerable : ArrayList
		{
		}

		#endregion
	}
}