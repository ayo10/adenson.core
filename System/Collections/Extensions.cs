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