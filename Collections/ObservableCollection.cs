using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.<br/>
	/// Portions of, from http://shevaspace.spaces.live.com/blog/cns!FD9A0F1F8DD06954!547.entry.
	/// </summary>
	/// <typeparam name="T">The type of items.</typeparam>
	public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		#region Variables
		private ReadOnlyObservableCollection<T> readOnly;
		private bool suspendCollectionChange;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the ObservableCollection class.
		/// </summary>
		public ObservableCollection() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ObservableCollection class that contains elements copied from the specified collection.
		/// </summary>
		/// <param name="collection">The collection from which the elements are copied.</param>
		/// <exception cref="ArgumentNullException">The list parameter cannot be null.</exception>
		public ObservableCollection(IEnumerable<T> collection) : base(collection)
		{
			System.Collections.ObjectModel.ObservableCollection<T> obs = collection as System.Collections.ObjectModel.ObservableCollection<T>;
			if (obs != null)
			{
				obs.CollectionChanged += this.OnSourceCollectionChanged;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ObservableCollection class that contains elements copied from the specified collection.
		/// </summary>
		/// <param name="list">The collection from which the elements are copied.</param>
		/// <exception cref="ArgumentNullException">The list parameter cannot be null.</exception>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Someone tell microsoft that")]
		public ObservableCollection(List<T> list)
			: base(list)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Occurs when an item is about to be added, removed, changed, or moved.
		/// </summary>
		public event EventHandler<CollectionChangingEventArgs<T>> CollectionChanging;

		/// <summary>
		/// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
		/// </summary>
		public override event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
		#region Methods

		/// <summary>
		/// Adds each item in items to the list
		/// </summary>
		/// <param name="items">The items to add.</param>
		/// <exception cref="ArgumentNullException">If items is null</exception>
		public void AddRange(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			suspendCollectionChange = true;
			int startingIndex = this.Count;
			foreach (T item in items)
			{
				this.Add(item);
			}

			suspendCollectionChange = false;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), startingIndex));
		}

		/// <summary>
		/// Removes each item in items from the list
		/// </summary>
		/// <param name="items">The items to remove</param>
		public void RemoveRange(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			foreach (T item in items)
			{
				this.Remove(item);
			}
		}

		/// <summary>
		/// Replaces the specified item with the new one
		/// </summary>
		/// <param name="existing">The existing item</param>
		/// <param name="replacement">The replacement</param>
		public void Replace(T existing, T replacement)
		{
			if (existing == null)
			{
				throw new ArgumentNullException("existing");
			}

			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}

			int index = this.IndexOf(existing);
			if (index == -1)
			{
				this.Add(replacement);
			}
			else
			{
				this[index] = replacement;
			}
		}

		/// <summary>
		/// Converts the list into a read only list
		/// </summary>
		/// <returns>A read only collection</returns>
		public ReadOnlyObservableCollection<T> AsReadOnly()
		{
			if (readOnly == null)
			{
				readOnly = new ReadOnlyObservableCollection<T>(this);
			}

			return readOnly;
		}

		/// <summary>
		/// Removes all items from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Remove, this, 0);
			this.OnCollectionChanging(e);
			if (!e.Cancel)
			{
				base.ClearItems();
			}
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		protected override void InsertItem(int index, T item)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Add, item, index);
			this.OnCollectionChanging(e);
			if (!e.Cancel)
			{
				base.InsertItem(index, item);
			}
		}

		/// <summary>
		/// Moves the item at the specified index to a new location in the collection.
		/// </summary>
		/// <param name="oldIndex">The old index</param>
		/// <param name="newIndex">The new index</param>
		protected override void MoveItem(int oldIndex, int newIndex)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Move, this[oldIndex], newIndex, oldIndex);
			this.OnCollectionChanging(e);
			if (!e.Cancel)
			{
				base.MoveItem(oldIndex, newIndex);
			}
		}

		/// <summary>
		/// Removes the item at the specified index of the collection.
		/// </summary>
		/// <param name="index">The index to remove the item</param>
		protected override void RemoveItem(int index)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Remove, this[index], index);
			this.OnCollectionChanging(e);
			if (!e.Cancel)
			{
				base.RemoveItem(index);
			}
		}

		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The index to set the item</param>
		/// <param name="item">The item to set</param>
		protected override void SetItem(int index, T item)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Replace, item, this[index], index);
			this.OnCollectionChanging(e);
			if (!e.Cancel)
			{
				base.SetItem(index, item);
			}
		}

		/// <summary>
		/// Raises the CollectionChanged event with the provided arguments.
		/// </summary>
		/// <param name="e">The event</param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged == null || suspendCollectionChange)
			{
				return;
			}

			using (IDisposable disposable = this.BlockReentrancy())
			{
				foreach (Delegate del in this.CollectionChanged.GetInvocationList())
				{
					NotifyCollectionChangedEventHandler handler = (NotifyCollectionChangedEventHandler)del;
					DispatcherObject dispatcherInvoker = del.Target as DispatcherObject;
					ISynchronizeInvoke syncInvoker = del.Target as ISynchronizeInvoke;
					if (dispatcherInvoker != null)
					{
						int count = GetDisableProcessingCount(dispatcherInvoker.Dispatcher);
						if (count == 0)
						{
							dispatcherInvoker.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { handler(this, e); }));
						}
						else
						{
							dispatcherInvoker.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate { handler(this, e); }));
						}
					}
					else if (syncInvoker != null)
					{
						syncInvoker.Invoke(del, new object[] { this, e });
					}
					else
					{
						handler(this, e);
					}
				}
			}
		}

		private static int GetDisableProcessingCount(Dispatcher dispatcher)
		{
			return (int)dispatcher.GetType().GetField("_disableProcessingCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(dispatcher);
		}

		private void OnCollectionChanging(CollectionChangingEventArgs<T> e)
		{
			if (this.CollectionChanging == null)
			{
				return;
			}

			this.CollectionChanging(this, e);
		}

		private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					this.AddRange(e.NewItems.Cast<T>());
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Remove:
					this.RemoveRange(e.OldItems.Cast<T>());
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
			}
		}

		#endregion
	}
}