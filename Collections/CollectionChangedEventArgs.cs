using System;
using System.Collections.Generic;
using System.Text;

namespace Adenson.Collections
{
	/// <summary>
	/// Events for collection value changing/setting
	/// </summary>
	public class CollectionChangedEventArgs : EventArgs
	{
	}
	/// <summary>
	/// Events for collection value changing/setting
	/// </summary>
	public class CollectionChangedEventArgs<T> : CollectionChangedEventArgs
	{
		#region Constructor

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="oldValue">The old value</param>
		/// <param name="newValue">The new new value</param>
		public CollectionChangedEventArgs(int position, T oldValue, T newValue)
		{
			this.Position = position;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		#endregion
		#region Properties

		/// <summary>
		/// The position
		/// </summary>
		public readonly int Position;
		/// <summary>
		/// The old value
		/// </summary>
		public readonly T OldValue;
		/// <summary>
		/// The new value
		/// </summary>
		public readonly T NewValue;

		#endregion
	}
}