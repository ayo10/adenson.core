using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Reflection
{
	/// <summary>
	/// PropertyInfo wrapper class.
	/// </summary>
	public sealed class PropertyWrapper : BaseWrapper<PropertyInfo>
	{
		#region Fields
		private static Dictionary<Type, List<PropertyWrapper>> cache = new Dictionary<Type, List<PropertyWrapper>>();
		#endregion
		#region Constructor

		private PropertyWrapper(Reflector parent, PropertyInfo item) : base(parent, item)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the name of the property
		/// </summary>
		public string Name
		{
			get { return this.Item.Name; }
		}

		#endregion
		#region Methods

		internal static IEnumerable<PropertyWrapper> Read(Reflector parent, Type type)
		{
			List<PropertyWrapper> results;
			if (!cache.TryGetValue(type, out results))
			{
				results = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
					.Select(m => new PropertyWrapper(parent, m))
					.ToList();
				cache.Add(type, results);
			}

			return results;
		}

		#endregion
	}
}