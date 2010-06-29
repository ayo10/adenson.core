using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;

namespace Adenson.Collections
{
	/// <summary>
	/// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.<br/>
	/// Portions of, from http://shevaspace.spaces.live.com/blog/cns!FD9A0F1F8DD06954!547.entry
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		#region Variables
		private ReadOnlyObservableCollection<T> readOnly;
		#endregion
		#region Constructor

		public ObservableCollection() : base()
		{
		}
		public ObservableCollection(IEnumerable<T> list) : base(list)
		{
		}
		//public ObservableCollection(IEnumerable<T> list, System.Collections.IComparer customSort) : base(list)
		//{
		//    ((ListCollectionView)this.View).CustomSort = customSort;
		//}

		#endregion
		#region Properties

		public new T this[int index]
		{
			get { return base[index]; }
			set
			{
				if (this.Count == index) base.Add(value);
				else base[index] = value;
			}
		}
		//public CollectionView View
		//{
		//    get { return (CollectionView)CollectionViewSource.GetDefaultView(this); }
		//}
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

		public void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items) this.Add(item);
		}
		public void RemoveRange(IEnumerable<T> items)
		{
			foreach (T item in items) this.Remove(item);
		}
		public void Replace(T existing, T replacement)
		{
			if (existing == null) throw new ArgumentNullException("existing");
			if (replacement == null) throw new ArgumentNullException("replacement");
			int index = this.IndexOf(existing);
			if (index == -1) this.Add(replacement);
			else this[index] = replacement;
		}
		public ReadOnlyObservableCollection<T> AsReadOnly()
		{
			if (readOnly == null) readOnly = new ReadOnlyObservableCollection<T>(this);
			return readOnly;
		}

		internal void AddPropertyChangedEvent(PropertyChangedEventHandler value)
		{
			this.PropertyChanged += value;
		}
		internal void RemovePropertyChangedEvent(PropertyChangedEventHandler value)
		{
			this.PropertyChanged -= value;
		}

		protected override void ClearItems()
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Remove, this, 0);
			this.OnCollectionChanging(e);
			if (!e.Cancel) base.ClearItems();
		}
		protected override void InsertItem(int index, T item)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Add, item, index);
			this.OnCollectionChanging(e);
			if (!e.Cancel) base.InsertItem(index, item);
		}
		protected override void MoveItem(int oldIndex, int newIndex)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Move, this[oldIndex], newIndex, oldIndex);
			this.OnCollectionChanging(e);
			if (!e.Cancel) base.MoveItem(oldIndex, newIndex);
		}
		protected override void RemoveItem(int index)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Remove, this[index], index);
			this.OnCollectionChanging(e);
			if (!e.Cancel) base.RemoveItem(index);
		}
		protected override void SetItem(int index, T item)
		{
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(NotifyCollectionChangedAction.Replace, item, this[index], index);
			this.OnCollectionChanging(e);
			if (!e.Cancel) base.SetItem(index, item);
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged == null) return;
			using (IDisposable disposable = this.BlockReentrancy())
			{
				foreach (Delegate del in this.CollectionChanged.GetInvocationList())
				{
					NotifyCollectionChangedEventHandler handler = (NotifyCollectionChangedEventHandler)del;
					DispatcherObject dispatcherInvoker = del.Target as DispatcherObject;
					ISynchronizeInvoke syncInvoker = del.Target as ISynchronizeInvoke;
					if (dispatcherInvoker != null)
					{
						int count = dispatcherInvoker.Dispatcher.GetDisableProcessingCount();
						if (count == 0) dispatcherInvoker.Dispatcher.Invoke(DispatcherPriority.Normal, new MethodInvoker(delegate { handler(this, e); }));
					}
					else if (syncInvoker != null) syncInvoker.Invoke(del, new Object[] { this, e });
					else handler(this, e);
				}
			}
		}

		private void OnCollectionChanging(CollectionChangingEventArgs<T> e)
		{
			if (this.CollectionChanging == null) return;
			this.CollectionChanging(this, e);
		}

		#endregion
	}
}