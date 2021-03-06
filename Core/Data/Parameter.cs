#if !NETSTANDARD1_0
using System;
using System.Data;
using System.Diagnostics;

namespace Adenson.Data
{
	/// <summary>
	/// Represents simple a key/value pair that can be automagically turned into a IDbDataParameter 
	/// </summary>
	public sealed class Parameter : IEquatable<Parameter>
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Parameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public Parameter(string name, object value)
		{
			this.Name = Arg.IsNotEmpty(name);
			this.Value = value == null ? DBNull.Value : value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Parameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The value type.</param>
		public Parameter(string name, object value, DbType type) : this(name, value)
		{
			this.DbType = type;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the value of the parameter.
		/// </summary>
		public object Value
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the db type.
		/// </summary>
		public DbType? DbType
		{
			get;
			set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Checks equality of name and value of the other.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <returns>true, or false, i dont know</returns>
		public bool Equals(Parameter other)
		{
			if (other == null)
			{
				return false;
			}

			if (Object.ReferenceEquals(this, other))
			{
				return true;
			}

			return other.Name == this.Name && other.Value == this.Value;
		}

		/// <summary>
		/// Checks equality of name and value of the other.
		/// </summary>
		/// <param name="obj">The other.</param>
		/// <returns>Returns true if they are the same reference, false otherwise.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as Parameter;
			if (other != null)
			{
				return this.Equals(other);
			}

			return base.Equals(obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion
	}
}
#endif