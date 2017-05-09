using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Reflection
{
	/// <summary>
	/// Method info wrapper.
	/// </summary>
	public sealed class MethodWrapper : BaseWrapper<MethodInfo>
	{
		#region Fields
		private static Dictionary<Type, List<MethodWrapper>> cache = new Dictionary<Type, List<MethodWrapper>>();
		#endregion
		#region Constructor

		private MethodWrapper(Reflector parent, MethodInfo item) : base(parent, item)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the name of the method.
		/// </summary>
		public string Name
		{
			get { return this.Item.Name; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Invokes the method represented by the current instance, using the specified parameters.
		/// </summary>
		/// <param name="parameters">An argument list for the invoked method.</param>
		/// <returns>An object containing the return value of the invoked method.</returns>
		/// <exception cref="ArgumentException">The elements of the parameters array do not match the signature of the method.</exception>
		public object Invoke(params object[] parameters)
		{
			return this.Item.Invoke(this.Parent.Item, parameters);
		}

		internal static IEnumerable<MethodWrapper> Read(Reflector parent, Type type)
		{
			List<MethodWrapper> results;
			if (!cache.TryGetValue(type, out results))
			{
				#if NETSTANDARD1_6
				results = type.GetTypeInfo().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
					.Select(m => new MethodWrapper(parent, m))
					.ToList();
				#else
				results = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
					.Select(m => new MethodWrapper(parent, m))
					.ToList();
				#endif
				cache.Add(type, results);
			}

			return results;
		}

		#endregion
	}
}