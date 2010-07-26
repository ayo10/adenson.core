using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Adenson.Collections
{
	public class HashtableChangedEventArgs<Tkey> : EventArgs
	{
		public readonly NotifyCollectionChangedAction Action;
		public readonly Tkey Key;
		public HashtableChangedEventArgs(NotifyCollectionChangedAction action)
		{
			this.Action = action;
		}
		public HashtableChangedEventArgs(NotifyCollectionChangedAction action, Tkey key) : this(action)
		{
			this.Key = key;
		}
	}
}