using System;
using System.ComponentModel;

namespace System
{
	/// <summary>
	/// Defines a value pair that can be set or retrieved.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public struct Pair<T> : IEquatable<Pair<T>>
	{
		#region Constructor

		/// <summary>
		/// Instantiates a new instance of a pair object with specified values
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		public Pair(T left, T right) : this()
		{
			this.Left = left;
			this.Right = right;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the left side of the pair
		/// </summary>
		public T Left
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the right side of the pair
		/// </summary>
		public T Right
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets if the pair is empty, i.e, a pair that was instantiated with an empty constructor
		/// </summary>
		public bool IsEmpty
		{
			get { return this.Equals(new Pair<T>()); }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Gets if this pair equals the other
		/// </summary>
		/// <param name="other">The other to compare to.</param>
		/// <returns>true if </returns>
		public bool Equals(Pair<T> other)
		{
			if (Object.ReferenceEquals(this, other)) return true;
			if (Object.ReferenceEquals(other, null)) return false;
			return Object.Equals(this.Left, other.Left) && Object.Equals(this.Right, other.Right);
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>true if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (Object.ReferenceEquals(this, obj)) return true;
			if (obj is Pair<T>) return this.Equals((Pair<T>)obj);
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns> A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public Pair<T> Fill(Pair<T> part)
		{
			if (this.IsEmpty) return part;
			if (this.Left != null && this.Right != null) return this;
			else if (this.Left != null) return new Pair<T>(this.Left, part.Right);
			else if (this.Right != null) return new Pair<T>(part.Left, this.Right);
			return this;
		}

		/// <summary>
		/// Returns a string representation of the Pair, using the string representations of the value.
		/// </summary>
		/// <returns>A string representation of the Pair, using the string representations of the value</returns>
		public override string ToString()
		{
			if (this.IsEmpty) return String.Empty;
			else if (this.Right == null) return this.Left == null ? String.Empty : this.Left.ToString();
			else return String.Concat(this.Left, ", ", this.Right);
		}

		#endregion
		#region Operators

		/// <summary>
		/// Checks the equality of the two specified pairs
		/// </summary>
		/// <param name="pair1">The first</param>
		/// <param name="pair2">The second</param>
		/// <returns>True, if they are equal, false otherwise</returns>
		public static bool operator ==(Pair<T> pair1, Pair<T> pair2)
		{
			return pair1.Equals(pair2);
		}
		/// <summary>
		/// Checks the inequality of the two specified pairs
		/// </summary>
		/// <param name="pair1">The first</param>
		/// <param name="pair2">The second</param>
		/// <returns>True, if they are not equal, false otherwise</returns>
		public static bool operator !=(Pair<T> pair1, Pair<T> pair2)
		{
			return !pair1.Equals(pair2);
		}

		#endregion
	}
}