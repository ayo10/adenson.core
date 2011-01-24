using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Adenson.Data
{
	/// <summary>
	/// Represents simple a key/value pair that can be automagically turned into a IDbDataParameter 
	/// </summary>
	public sealed class Parameter : IEquatable<Parameter>
	{
		#region Variables
		private string _key;
		private object _value;
		private bool changed;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Parameter"/> struct.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public Parameter(string name, object value)
		{
			_key = name;
			_value = value;
			changed = true;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets if this instance has been set or not
		/// </summary>
		public bool IsEmpty
		{
			get 
			{
				if (!changed) return this.Name == null && this.Value == null;
				return false;
			}
		}
		/// <summary>
		/// Gets or sets the name of the parameter
		/// </summary>
		public string Name
		{
			get { return _key; }
			set 
			{ 
				_key = value;
				changed = true;
			}
		}
		/// <summary>
		/// Gets or sets the value of the parameter
		/// </summary>
		public object Value
		{
			get { return _value; }
			set
			{
				_value = value;
				changed = true;
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Checks equality of name and value of the other
		/// </summary>
		/// <param name="other">The other dude</param>
		/// <returns>true, or false, i dont know</returns>
		public bool Equals(Parameter other)
		{
			if (Object.ReferenceEquals(this, other)) return true;
			return other.Name == this.Name && other.Value == this.Value;
		}
		/// <summary>
		/// if obj is Parameter, does this.Equals((Parameter)obj), else does the base equality check
		/// </summary>
		/// <param name="obj">The other dude</param>
		/// <returns>true, or false, i dont know</returns>
		public override bool Equals(object obj)
		{
			var other = obj as Parameter;
			if (other != null) return this.Equals(other);
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