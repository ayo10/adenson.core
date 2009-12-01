using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Adenson.Data
{
	/// <summary>
	/// Represents simple a key/value pair that can be automagically turned into a IDbDataParameter 
	/// </summary>
	public struct Parameter : IEquatable<Parameter>
	{
		#region Variables
		private string _key;
		private object _value;
		private bool changed;
		#endregion
		#region Constructor

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
			return other.Name == this.Name && other.Value == this.Value;
		}
		/// <summary>
		/// if obj is Parameter, does this.Equals((Parameter)obj), else does the base equality check
		/// </summary>
		/// <param name="obj">The other dude</param>
		/// <returns>true, or false, i dont know</returns>
		public override bool Equals(object obj)
		{
			if (obj is Parameter) return this.Equals((Parameter)obj);
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
		/// <summary>
		/// Converts this instance to specified type instance by using reflection to instantiate
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <returns>The result of the instantiation</returns>
		public T Convert<T>() where T : IDataParameter
		{
			if (this.IsEmpty) return default(T);
			Type type = typeof(T);
			T parameter = (T)type.Assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, new object[] { this.Name, this.Value }, null, null);
			return parameter;
		}

		/// <summary>
		/// This is just a lazy way of doing new Parameter(){set properties here}
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		/// <returns>A Parameter object</returns>
		public static Parameter New(string key, object value)
		{
			Parameter sp = new Parameter();
			sp.Name = key;
			sp.Value = value;
			return sp;
		}
		/// <summary>
		/// Implicitly converts this instance into a SqlParameter, cheating, yeah I know
		/// </summary>
		/// <param name="param">The parameter to convert</param>
		/// <returns>A SqlParameter object</returns>
		public static implicit operator SqlParameter(Parameter param)
		{
			if (param.IsEmpty) return null;
			return new SqlParameter(param.Name, param.Value);
		}

		#endregion
	}
}