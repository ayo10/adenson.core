using System;

namespace System
{
	/// <summary>
	/// Represents a coordinate
	/// </summary>
	public struct Coordinate : IEquatable<Coordinate>
	{
		#region Constructor

		/// <summary>
		/// Instantiates a new coordinate object
		/// </summary>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		public Coordinate(double latitude, double longitude) : this()
		{
			this.Latitude = latitude;
			this.Longitude = longitude;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the latitude
		/// </summary>
		public double Latitude
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the longitude
		/// </summary>
		public double Longitude
		{
			get;
			set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Gets if this instance equals the other.
		/// </summary>
		/// <param name="other">The other</param>
		/// <returns>True, if they are both equal, false otherwise</returns>
		public bool Equals(Coordinate other)
		{
			return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>true if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
		}

		#endregion
		#region Operators

		/// <summary>
		/// Checks the equality of the two specified objects.
		/// </summary>
		/// <param name="value1">The first</param>
		/// <param name="value2">The second</param>
		/// <returns>True, if they are equal, false otherwise</returns>
		public static bool operator ==(Coordinate value1, Coordinate value2)
		{
			return value1.Equals(value2);
		}

		/// <summary>
		/// Checks the inequality of the two specified objects.
		/// </summary>
		/// <param name="value1">The first</param>
		/// <param name="value2">The second</param>
		/// <returns>True, if they are not equal, false otherwise</returns>
		public static bool operator !=(Coordinate value1, Coordinate value2)
		{
			return !value1.Equals(value2);
		}

		#endregion
	}
}