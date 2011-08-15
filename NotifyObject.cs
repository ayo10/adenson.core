using System;
using System.Collections.Generic;
using System.ComponentModel;
using Adenson.Collections;

namespace System
{
	/// <summary>
	/// Object that fires <see cref="INotifyPropertyChanged.PropertyChanged"/> and <see cref="INotifyPropertyChanging.PropertyChanging"/> events
	/// </summary>
	public abstract class NotifyObject<T> : INotifyPropertyChanged, INotifyPropertyChanging
	{
		#region Variables
		private Hashtable<string, T> keyValues = new Hashtable<string, T>();
		#endregion
		#region Properties

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Occurs when a property value is changing.
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;

		#endregion
		#region Methods

		/// <summary>
		/// Gets the value matching the specified key.
		/// </summary>
		/// <param name="key">The key of the value</param>
		/// <returns>The found value, default of T otherwise</returns>
		protected T GetValue(String key)
		{
			return this.GetValue(key, default(T));
		}
		/// <summary>
		/// Gets the value matching the specified key, or <paramref name="defaultValue"/> if none is found.
		/// </summary>
		/// <param name="key">The key of the value</param>
		/// <returns>The found value, <paramref name="defaultValue"/> otherwise</returns>
		protected T GetValue(String key, T defaultValue)
		{
			return keyValues.ContainsKey(key) ? keyValues[key] : defaultValue;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		protected void SetValue(String key, T value)
		{
			if (Object.Equals(keyValues[key], value)) return;
			this.OnPropertyChanging(key);
			keyValues[key] = value;
			this.OnPropertyChanged(key);
		}
		/// <summary>
		/// Called after the property has changed.
		/// </summary>
		/// <param name="key">The value's key</param>
		protected void OnPropertyChanged(String key)
		{
			PropertyChangedEventArgs e = new PropertyChangedEventArgs(key);
			if (this.PropertyChanged != null) this.PropertyChanged(this, e);
			//if (events.ContainsKey(key)) events[key](this, e);
		}
		/// <summary>
		/// Called before the property's value changes.
		/// </summary>
		/// <param name="key">The value's key</param>
		protected void OnPropertyChanging(String key)
		{
			if (this.PropertyChanging != null) this.PropertyChanging(this, new PropertyChangingEventArgs(key));
		}

		#endregion
	}
	/// <summary>
	/// Extension of NotifyObject that uses <see cref="Object"/> for its values
	/// </summary>
	public abstract class NotifyObject : NotifyObject<Object>
	{
	}
}