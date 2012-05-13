using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Adenson.Collections
{
	/// <summary>
	/// Events for collection value changing/setting that is cancellable
	/// </summary>
	/// <typeparam name="T">The type of argument</typeparam>
	public class CollectionChangingEventArgs<T> : NotifyCollectionChangedEventArgs
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the CollectionChangingEventArgs class
		/// </summary>
		/// <param name="action">The change action</param>
		/// <param name="changedItem">The changed object</param>
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, T changedItem) : base(action, changedItem)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CollectionChangingEventArgs class
		/// </summary>
		/// <param name="action">The change action</param>
		/// <param name="changedItem">The changed object</param>
		/// <param name="index">The index of the change</param>
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, T changedItem, int index) : base(action, changedItem, index)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CollectionChangingEventArgs class
		/// </summary>
		/// <param name="action">The change action</param>
		/// <param name="changedItems">The changed objects</param>
		/// <param name="startingIndex">The index of the change</param>
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, IEnumerable<T> changedItems, int startingIndex) : base(action, changedItems.ToList(), startingIndex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CollectionChangingEventArgs class
		/// </summary>
		/// <param name="action">The change action</param>
		/// <param name="newItem">The new item</param>
		/// <param name="oldItem">The replaced item</param>
		/// <param name="index">The index of the change</param>
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index) : base(action, newItem, oldItem, index)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CollectionChangingEventArgs class
		/// </summary>
		/// <param name="action">The change action</param>
		/// <param name="changedItem">The changed object</param>
		/// <param name="index">The index of the change</param>
		/// <param name="oldIndex">The old index</param>
		public CollectionChangingEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex) : base(action, changedItem, index, oldIndex)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether to cancel the event or not
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