using System;
using System.Collections.Generic;

namespace Adenson.Collections
{
	/// <summary>
	/// Monitored collection is a collection type with events you can hook callbacks into.
	/// A replacement for MonitoredCollection.
	/// </summary>
	public class MonitoredCollection<T> : System.Collections.ObjectModel.Collection<T>, ICollection<T>
	{
		#region Variables
		private bool _allowNulls = true;
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the MonitoredCollection class that is empty.
		/// </summary>
		public MonitoredCollection() : base(new List<T>())
		{
		}
		/// <summary>
		/// Initializes a new instance of the MonitoredCollection class that is empty.
		/// </summary>
		/// <param name="allowNulls">If the collection should allow nulls</param>
		public MonitoredCollection(bool allowNulls) : this()
		{
			this.AllowNulls = allowNulls;
		}
		/// <summary>
		/// Initializes a new instance of the MonitoredCollection class as a wrapper for the specified list.
		/// </summary>
		/// <param name="list">The list that is wrapped by the new collection.</param>
		public MonitoredCollection(IEnumerable<T> collection) : base(new List<T>(collection))
		{
		}
		/// <summary>
		/// Initializes a new instance of the MonitoredCollection class as a wrapper for the specified list.
		/// </summary>
		/// <param name="list">The list that is wrapped by the new collection.</param>
		/// <param name="allowNulls">If the collection should allow nulls</param>
		/// <exception cref="ArgumentNullException">if any value in the collection is null and allowNulls is false</exception>
		public MonitoredCollection(IEnumerable<T> collection, bool allowNulls) : this(collection)
		{
			if (!allowNulls)
			{
				foreach (object item in collection)
				{
					if (item == null) throw new ArgumentNullException(ExceptionMessages.AllowNullsFalseNulls);
				}
			}
			this.AllowNulls = allowNulls;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Occurs before an item is added into the collection and is left to occur if CollectionChangingEventArgs.Cancel is still true after being handled
		/// </summary>
		/// <remarks>The item added will be on the CollectionChangingEventArgs.NewValue property</remarks>
		public event EventHandler<CollectionChangingEventArgs<T>> PreInsert;
		/// <summary>
		/// Occurs after the item has been inserted into the collection
		/// </summary>
		/// <remarks>The item added will be on the CollectionChangingEventArgs.NewValue property</remarks>
		public event EventHandler<CollectionChangedEventArgs<T>> PostInsert;
		/// <summary>
		/// Occurs before the collection is cleared, and is cancellable.
		/// </summary>        
		public event EventHandler<CollectionChangingEventArgs> PreClear;
		/// <summary>
		/// Occurs after the collection has been cleared of its items
		/// </summary>         
		public event EventHandler<CollectionChangedEventArgs> PostClear;
		/// <summary>
		/// Occurs before an item is removed from the collection and is left to occur if CollectionChangingEventArgs.Cancel is still true after being handled
		/// </summary>
		/// <remarks>The item removed will be on the CollectionChangingEventArgs.OldValue property</remarks>
		public event EventHandler<CollectionChangingEventArgs<T>> PreRemove;
		/// <summary>
		/// Occurs after the item has been removed from the collection
		/// </summary>
		/// <remarks>The item removed will be on the CollectionChangingEventArgs.OldValue property</remarks>
		public event EventHandler<CollectionChangedEventArgs<T>> PostRemove;
		/// <summary>
		/// Occurs before one item is exchanged for the other and is cancellable
		/// </summary>  
		public event EventHandler<CollectionChangingEventArgs<T>> PreSet;
		/// <summary>
		/// Occurs after items have been exchanged
		/// </summary>          
		public event EventHandler<CollectionChangedEventArgs<T>> PostSet;
		/// <summary>
		/// Gets if the collection allow nulls or not, defaults to true
		/// </summary>
		public bool AllowNulls
		{
			get { return _allowNulls; }
			private set { _allowNulls = value; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Returns a list that cannot be modified directly
		/// </summary>
		/// <returns>A read-only list</returns>
		public IReadOnlyList<T> AsReadOnly()
		{
			return new ReadOnlyList<T>(this);
		}
		/// <summary>
		/// Returns an enumerable objects in the collection that match specified type
		/// </summary>
		/// <typeparam name="Ts">The type</typeparam>
		/// <returns>An IEnumerable list</returns>
		public IEnumerable<Ts> OfType<Ts>() where Ts : T
		{
			for (int i = 0; i < this.Count; i++)
			{
				T obj = this[i];
				//Cant use 'as' in this case because Ts can be a value type
				if (obj is Ts) yield return (Ts)obj;
			}
		}
		/// <summary>
		/// Returns an enumerable objects in the collection that match specified type
		/// </summary>
		/// <typeparam name="Ts">The type</typeparam>
		/// <returns>An IEnumerable list</returns>
		public IEnumerable<Ts> Cast<Ts>() where Ts : T
		{
			foreach (T obj in this) yield return (Ts)obj;
		}
		/// <summary>
		/// Sorts the list using .NET default comparer
		/// </summary>
		public virtual void Sort()
		{
			(this.Items as List<T>).Sort();
		}
		/// <summary>
		/// Sorts the list using the comparer
		/// </summary>
		/// <param name="comparer">The comparer to use to sort the list</param>
		public virtual void Sort(IComparer<T> comparer)
		{
			(this.Items as List<T>).Sort(comparer);
		}
		/// <summary>
		/// Removes all elements from the Collection.
		/// </summary>
		protected override void ClearItems()
		{
			CollectionChangingEventArgs e = new CollectionChangingEventArgs();
			if (this.PreClear != null) this.PreClear(this, e);
			if (!e.Cancel)
			{
				base.ClearItems();
				if (this.PostClear != null) this.PostClear(this, new CollectionChangedEventArgs());
			}
		}
		/// <summary>
		/// Inserts an element into the Collection at the specified index.
		/// </summary>
		/// <param name="index">The object to insert. The value can be null for reference types.</param>
		/// <param name="item">The zero-based index at which item should be inserted.</param>
		protected override void InsertItem(int index, T item)
		{
			if (!this.AllowNulls && (object)item == null) throw new ArgumentNullException(ExceptionMessages.CollectionsNoNullValues);

			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(index, default(T), item);
			if (this.PreInsert != null) this.PreInsert(this, e);
			if (!e.Cancel)
			{
				base.InsertItem(index, item);
				if (this.PostInsert != null) this.PostInsert(this, new CollectionChangedEventArgs<T>(index, default(T), item));
			}
		}
		/// <summary>
		/// Removes the element at the specified index of the Collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		protected override void RemoveItem(int index)
		{
			T item = this[index];
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(index, item, default(T));
			if (this.PreRemove != null) this.PreRemove(this, e);
			if (!e.Cancel)
			{
				base.RemoveItem(index);
				if (this.PostRemove != null) this.PostRemove(this, new CollectionChangedEventArgs<T>(index, item, default(T)));
			}
		}
		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			if (!this.AllowNulls && (object)item == null) throw new ArgumentNullException(ExceptionMessages.CollectionsNoNullValues);

			T oldValue = this[index];
			CollectionChangingEventArgs<T> e = new CollectionChangingEventArgs<T>(index, oldValue, item);
			if (this.PreSet != null) this.PreSet(this, e);
			if (!e.Cancel)
			{
				base.SetItem(index, item);
				if (this.PostSet != null) this.PostSet(this, new CollectionChangedEventArgs<T>(index, oldValue, item));
			}
		}

		#endregion
	}
}