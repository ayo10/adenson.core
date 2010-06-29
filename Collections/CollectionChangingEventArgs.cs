using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// Events for collection value changing/setting that is cancellable
	/// </summary>
	public class CollectionChangingEventArgs<T> : NotifyCollectionChangedEventArgs
	{
		#region Constructors

		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, T changedItem) : base(action, changedItem)
		{
		}
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, T changedItem, int index) : base(action, changedItem, index)
		{
		}
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, IEnumerable<T> changedItems, int startingIndex) : base(action, changedItems.ToList(), startingIndex)
		{
		}
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index) : base(action, newItem, oldItem, index)
		{
		}
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex) : base(action, changedItem, index, oldIndex)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets if to cancel the event or not
		/// </summary>
		public bool Cancel
		{
			get;
			set;
		}
		/// <summary>
		/// Gets the list of new items involved in the change.
		/// </summary>
		public new IList<T> NewItems 
		{
			get { return base.NewItems.Cast<T>().ToList(); }
		}

		#endregion
	}
}