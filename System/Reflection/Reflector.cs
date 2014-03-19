using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
	/// <summary>
	/// A quick reflector object.
	/// </summary>
	public sealed class Reflector
	{
		#region Fields
		private static Dictionary<int, Reflector> created = new Dictionary<int, Reflector>();
		private static Dictionary<Type, List<MethodInfo>> methods = new Dictionary<Type, List<MethodInfo>>();
		private static Dictionary<Type, List<PropertyInfo>> properties = new Dictionary<Type, List<PropertyInfo>>();
		private WeakReference itemReference;
		#endregion
		#region Constructor

		private Reflector(object item)
		{
			throw new NotImplementedException();
			this.Item = Item;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets all the attributes on the object.
		/// </summary>
		public IEnumerable<Attribute> Attributes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets all the method names of the object.
		/// </summary>
		public IEnumerable<string> MethodNames
		{
			get
			{
				foreach (var m in this.Methods)
				{
					yield return m.Name;
				}
			}
		}

		/// <summary>
		/// Gets the item being wrapped.
		/// </summary>
		public object Item
		{
			get 
			{
				if (itemReference.IsAlive)
				{
					return null;
				}

				return itemReference.Target;
			}
			private set
			{
				itemReference = new WeakReference(value);
			}
		}

		/// <summary>
		/// Gets all the methods of the object.
		/// </summary>
		public IEnumerable<MethodInfo> Methods
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets all the properties of the object.
		/// </summary>
		public IEnumerable<PropertyInfo> Properties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets all the property names of the object.
		/// </summary>
		public IEnumerable<string> PropertyNames
		{
			get
			{
				foreach (var p in this.Properties)
				{
					yield return p.Name;
				}
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Gets an existing instance of a reflector object if method was called for the object (using its HashCode), else creates a new instance of the object.
		/// </summary>
		/// <typeparam name="T">The type if object.</typeparam>
		/// <param name="item">The item to wrap.</param>
		/// <returns>A new reflector.</returns>
		internal static Reflector Wrap<T>(T item) where T : class
		{
			throw new NotImplementedException();
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			else if (item is Type)
			{
				throw new ArgumentException("The object cannot be a System.Type object.", "item");
			}

			Reflector value;
			if (!created.TryGetValue(item.GetHashCode(), out value))
			{
				value = new Reflector(item);
				created.Add(item.GetHashCode(), value);
			}

			return value;
		}

		#endregion
	}
}