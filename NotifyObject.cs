using System;
using System.Collections.Generic;
using System.ComponentModel;
using Adenson.Collections;

namespace System
{
	/// <summary>
	/// Object that fires <see cref="INotifyPropertyChanged.PropertyChanged"/> and <see cref="INotifyPropertyChanging.PropertyChanging"/> events
	/// </summary>
	public abstract class NotifyObject : INotifyPropertyChanged, INotifyPropertyChanging
	{
		#region Variables
		private Hashtable<string, object> keyValues = new Hashtable<string, object>();
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
		protected object GetValue(String key)
		{
			return this.GetValue(key, null);
		}

		/// <summary>
		/// Gets the value matching the specified key, or <paramref name="defaultValue"/> if none is found.
		/// </summary>
		/// <param name="key">The key of the value</param>
		/// <param name="defaultValue">Value to return if the specified key doesn't exist</param>
		/// <returns>The found value, <paramref name="defaultValue"/> otherwise</returns>
		protected object GetValue(String key, object defaultValue)
		{
			return keyValues.ContainsKey(key) ? keyValues[key] : defaultValue;
		}

		/// <summary>
		/// Sets the value with the specified key
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		protected void SetValue(String key, object value)
		{
			if (Object.Equals(keyValues[key], value))
			{
				return;
			}

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
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, e);
			}
		}

		/// <summary>
		/// Called before the property's value changes.
		/// </summary>
		/// <param name="key">The value's key</param>
		protected void OnPropertyChanging(String key)
		{
			if (this.PropertyChanging != null)
			{
				this.PropertyChanging(this, new PropertyChangingEventArgs(key));
			}
		}

		#endregion
	}
}