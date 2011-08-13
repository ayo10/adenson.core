using System;
using System.Collections.Generic;
using System.ComponentModel;
using Adenson.Collections;

namespace Adenson.ListenQuest
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

		protected Object GetValue(String key)
		{
			return this.GetValue(key, null);
		}
		protected Object GetValue(String key, Object returnIfNull)
		{
			return keyValues.ContainsKey(key) ? keyValues[key] : returnIfNull;
		}
		protected void SetValue(String key, Object value)
		{
			if (Object.Equals(keyValues[key], value)) return;
			this.OnPropertyChanging(key);
			keyValues[key] = value;
			this.OnPropertyChanged(key);
		}
		protected void OnPropertyChanged(String key)
		{
			PropertyChangedEventArgs e = new PropertyChangedEventArgs(key);
			if (this.PropertyChanged != null) this.PropertyChanged(this, e);
			//if (events.ContainsKey(key)) events[key](this, e);
		}
		protected void OnPropertyChanging(String key)
		{
			if (this.PropertyChanging != null) this.PropertyChanging(this, new PropertyChangingEventArgs(key));
		}

		#endregion
	}
}