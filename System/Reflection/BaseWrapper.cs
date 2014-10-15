using System;

namespace System.Reflection
{
	/// <summary>
	/// Base wrapper class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseWrapper<T> where T : MemberInfo
	{
		/// <summary>
		/// .
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="item"></param>
		protected BaseWrapper(Reflector parent, T item)
		{
			this.Parent = parent;
			this.Item = item;
		}

		/// <summary>
		/// Gets the wrapped member info object.
		/// </summary>
		protected T Item
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the parent reflector object.
		/// </summary>
		protected Reflector Parent
		{
			get;
			private set;
		}
	}
}