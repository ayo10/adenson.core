using System;
using System.Collections.Specialized;

namespace Adenson.Collections
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="Tk"></typeparam>
	public class HashtableChangedEventArgs<Tk> : EventArgs
	{
		/// <summary>
		/// The action that took place
		/// </summary>
		public readonly NotifyCollectionChangedAction Action;
		/// <summary>
		/// The key
		/// </summary>
		public readonly Tk Key;

		/// <summary>
		/// Instantiates a new event
		/// </summary>
		/// <param name="action">The action</param>
		/// <param name="key">The key that changed</param>
		public HashtableChangedEventArgs(NotifyCollectionChangedAction action, Tk key)
		{
			this.Action = action;
			this.Key = key;
		}
	}
}