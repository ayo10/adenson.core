using System;
using System.Collections.Specialized;

namespace Adenson.Collections
{
	/// <summary>
	/// Event data for <see cref="Hashtable"/>
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	public class HashtableChangedEventArgs<TKey> : EventArgs
	{
		#region Constructor

		/// <summary>
		/// Instantiates a new event
		/// </summary>
		/// <param name="action">The action</param>
		/// <param name="key">The key that changed</param>
		public HashtableChangedEventArgs(NotifyCollectionChangedAction action, TKey key)
		{
			this.Action = action;
			this.Key = key;
		}

		#endregion
		#region Properties

		/// <summary>
		/// The action that took place
		/// </summary>
		public NotifyCollectionChangedAction Action
		{
			get;
			private set;
		}
		/// <summary>
		/// The key
		/// </summary>
		public TKey Key
		{
			get;
			private set;
		}

		#endregion
	}
}