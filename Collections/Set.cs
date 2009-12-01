using System;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// As implied by its name, models the mathematical set
	/// </summary>
	/// <typeparam name="T">The type of elements</typeparam>
	public sealed class Set<T> : ReadOnlyList<T>
	{
		#region Variables
		private IComparer<T> _comparer;
		#endregion
		#region Constructors

		/// <summary>
		/// Instantiates a new instance that is empty
		/// </summary>
		public Set() : base()
		{
		}
		/// <summary>
		/// Initializes a new instance that contains elements copied from the specified collection
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.</param>
		/// <exception cref="ArgumentNullException">collection is null.</exception>
		public Set(IEnumerable<T> collection) : base(collection)
		{
			if (collection == null) throw new ArgumentNullException("collection", ExceptionMessages.ArgumentNull);
			Set<T> set = collection as Set<T>;
			if (set != null) this.Comparer = set.Comparer;
		}
		/// <summary>
		/// Instantiates a new instance that is empty and sets the comparer property
		/// </summary>
		/// <param name="comparer">The icomparer that is used to compare elements</param>
		/// <exception cref="ArgumentNullException">comparer is null.</exception>
		public Set(IComparer<T> comparer) : this()
		{
			if (comparer == null) throw new ArgumentNullException("comparer", ExceptionMessages.ArgumentNull);
			_comparer = comparer;
		}
		/// <summary>
		/// Initializes a new instance that contains elements copied from the specified collection
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.</param>
		/// <exception cref="ArgumentNullException">collection is null or comparer is null.</exception>
		public Set(IEnumerable<T> collection, IComparer<T> comparer) : base(collection)
		{
			if (comparer == null) throw new ArgumentNullException("comparer", ExceptionMessages.ArgumentNull);
			this.Comparer = comparer;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Creates a new Set containing all the elements that this contains
		/// </summary>
		/// <returns>A new set</returns>
		public Set<T> Clone()
		{
			return new Set<T>(this);
		}
		/// <summary>
		/// Does a difference operation between this and the other collection
		/// </summary>
		/// <param name="set">The set to compare with</param>
		/// <returns>A collection containing the result of the difference</returns>
		public IEnumerable<T> Diff(IEnumerable<T> set)
		{
			Set<T> result = this.Clone();
			foreach (T obj in set)
			{
				if (result.Contains(obj)) result.InnerList.Remove(obj);
			}
			return result;
		}
		/// <summary>
		/// Does a difference operation between this and the other collection
		/// </summary>
		/// <param name="set">The set to compare with</param>
		/// <returns>A collection containing the result of the difference</returns>
		public IEnumerable<T> Difference(IEnumerable<T> set)
		{
			return this.Diff(set);
		}
		/// <summary>
		/// Returns the intersection of this collection and the other collection
		/// </summary>
		/// <param name="set">The set to compare with</param>
		/// <returns>A collection containing the result of the intersection operation</returns>
		public IEnumerable<T> Intersect(IEnumerable<T> set)
		{
			Set<T> result = this.Clone();
			Set<T> compare = set as Set<T>;
			if (compare == null) compare = new Set<T>(set);
			foreach (T obj in this)
			{
				if (compare.Contains(obj)) result.InnerList.Add(obj);
			}
			return result;
		}
		/// <summary>
		/// Does a exclusive or operation on this and the other collection
		/// </summary>
		/// <param name="set">The set to compare with</param>
		/// <returns>A collection containing the result of the exclusive or operation</returns>
		public IEnumerable<T> XOr(IEnumerable<T> set)
		{
			Set<T> result = this.Clone();
			foreach (T obj in set)
			{
				if (result.Contains(obj)) result.InnerList.Remove(obj);
				else result.InnerList.Add(obj);
			}
			return result;
		}
		/// <summary>
		/// Does a exclusive or operation on this and the other collection
		/// </summary>
		/// <param name="set">The set to compare with</param>
		/// <returns>A collection containing the result of the exclusive or operation</returns>
		public IEnumerable<T> ExclusiveOr(IEnumerable<T> set)
		{
			return this.XOr(set);
		}
		/// <summary>
		/// Does a union operation on this and the other collection
		/// </summary>
		/// <param name="set">The set to compare with</param>
		/// <returns>A collection containing the result of the union operation</returns>
		public IEnumerable<T> Union(IEnumerable<T> set)
		{
			Set<T> result = this.Clone();
			if (this.Comparer != null) result.Sort(this.Comparer);
			else result.Sort();
			foreach (T obj in set)
			{
				if (result.Contains(obj)) result.InnerList.Add(obj);
			}
			return result;
		}
		/// <summary>
		/// Checks to see if the collection contains specified object, using specified comparer
		/// </summary>
		/// <param name="item">the object</param>
		/// <returns>true, if the item is in the list, false otherwise</returns>
		public override bool Contains(T item)
		{
			if (this.Comparer == null) return base.Contains(item);
			foreach (T obj in this)
			{
				if (this.Comparer.Compare(item, obj) == 0) return true;
			}
			return false;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the IComparer object used in objects in collection comparism
		/// </summary>
		public IComparer<T> Comparer
		{
			get { return _comparer; }
			set { _comparer = value; }
		}

		#endregion
		#region Static Methods

		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> Diff(IEnumerable<T> collection1, IEnumerable<T> collection2)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1);
			return set1.Difference(new Set<T>(collection2));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <param name="comparer">The comparer to use to compare items in the collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> Diff(IEnumerable<T> collection1, IEnumerable<T> collection2, IComparer<T> comparer)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1, comparer);
			else set1.Comparer = comparer;
			return set1.Difference(new Set<T>(collection2, comparer));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> Intersect(IEnumerable<T> collection1, IEnumerable<T> collection2)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1);
			return set1.Intersect(new Set<T>(collection2));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <param name="comparer">The comparer to use to compare items in the collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> Intersect(IEnumerable<T> collection1, IEnumerable<T> collection2, IComparer<T> comparer)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1, comparer);
			else set1.Comparer = comparer;
			return set1.Intersect(new Set<T>(collection2, comparer));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> XOr(IEnumerable<T> collection1, IEnumerable<T> collection2)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1);
			return set1.XOr(new Set<T>(collection2));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <param name="comparer">The comparer to use to compare items in the collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> XOr(IEnumerable<T> collection1, IEnumerable<T> collection2, IComparer<T> comparer)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1, comparer);
			else set1.Comparer = comparer;
			return set1.XOr(new Set<T>(collection2, comparer));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> Union(IEnumerable<T> collection1, IEnumerable<T> collection2)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1);
			return set1.Union(new Set<T>(collection2));
		}
		/// <summary>
		/// Performs a Math set difference between passed in collections
		/// </summary>
		/// <param name="collection1">The first collection</param>
		/// <param name="collection2">The second collection</param>
		/// <param name="comparer">The comparer to use to compare items in the collection</param>
		/// <returns>The result </returns>
		public static IEnumerable<T> Union(IEnumerable<T> collection1, IEnumerable<T> collection2, IComparer<T> comparer)
		{
			Set<T> set1 = collection1 as Set<T>;
			if (set1 == null) set1 = new Set<T>(collection1, comparer);
			else set1.Comparer = comparer;
			return set1.Union(new Set<T>(collection2, comparer));
		}

		#endregion
		#region Static Operators

		/// <summary>
		/// Union of the two sets
		/// </summary>
		/// <param name="set1">The left hand side set</param>
		/// <param name="set2">The right hand set</param>
		/// <returns>the result of set1.Intersect(set2)</returns>
		public static IEnumerable<T> operator |(Set<T> set1, Set<T> set2)
		{
			return set1.Union(set2);
		}
		/// <summary>
		/// Intersection of the two sets
		/// </summary>
		/// <param name="set1">The left hand side set</param>
		/// <param name="set2">The right hand set</param>
		/// <returns>the result of set1.Intersect(set2)</returns>
		public static IEnumerable<T> operator &(Set<T> set1, Set<T> set2)
		{
			return set1.Intersect(set2);
		}
		/// <summary>
		/// Difference of the two sets
		/// </summary>
		/// <param name="set1">The left hand side set</param>
		/// <param name="set2">The right hand set</param>
		/// <returns>the result of set1.Difference(set2)</returns>
		public static IEnumerable<T> operator -(Set<T> set1, Set<T> set2)
		{
			return set1.Difference(set2);
		}
		/// <summary>
		/// Exclusiveness of the two sets
		/// </summary>
		/// <param name="set1">The left hand side set</param>
		/// <param name="set2">The right hand set</param>
		/// <returns>the result of set1.ExclusiveOr(set2)</returns>
		public static IEnumerable<T> operator ^(Set<T> set1, Set<T> set2)
		{
			return set1.XOr(set2);
		}

		#endregion
	}
}