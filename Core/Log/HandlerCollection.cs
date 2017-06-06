using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Adenson.Log
{
	/// <summary>
	/// Collection of handler objects.
	/// </summary>
	public sealed class HandlerCollection : Collection<BaseHandler>
	{
		#region Variables
		private Settings _settings;
		#endregion
		#region Constructor

		internal HandlerCollection(Settings settings)
		{
			_settings = settings;
		}

		#endregion
		#region Methods

		////internal static HandlerCollection FromConfig(Settings settings, SettingsConfiguration.HandlerElementCollection handlers)
		////{
		////	HandlerCollection result = new HandlerCollection(settings);
		////	if (handlers == null || handlers.Count == 0)
		////	{
		////		result.Add(new TraceHandler());
		////	}
		////	else
		////	{
		////		foreach (SettingsConfiguration.HandlerElement element in handlers)
		////		{
		////			BaseHandler handler = null;
		////			switch (element.Handler)
		////			{
		////				case HandlerType.Console:
		////					handler = new ConsoleHandler();
		////					break;
		////				case HandlerType.Debug:
		////					handler = new DebugHandler();
		////					break;
		////						///element.GetValue("connection", "Logger"), element.GetValue("tableName", "EventLog"), element.GetValue("severityColumn", "Severity"), element.GetValue("dateColumn", "Date"), element.GetValue("typeColumn", "Type"), element.GetValue("messageColumn", "Message")
		////				case HandlerType.Email:
		////					handler = new EmailHandler(element.GetValue("From", "logger@devnull"), element.GetValue("To", null), element.GetValue("Subject", "Adenson.Log.Logger"));
		////					break;
		////				case HandlerType.EventLog:
		////					handler = new EventLogHandler(element.GetValue("source", "Application"));
		////					break;
		////				case HandlerType.File:
		////					handler = new FileHandler(element.GetValue("fileName", "eventlogger.log"));
		////					break;
		////				case HandlerType.Trace:
		////					handler = new TraceHandler();
		////					break;
		////				case HandlerType.Custom:
		////					handler = TypeUtil.CreateInstance<BaseHandler>(element.CustomType);
		////					break;
		////			}

		////			handler.Severity = element.Severity;
		////			if (!String.IsNullOrEmpty(element.Formatter))
		////			{
		////				handler.Formatter = TypeUtil.CreateInstance<BaseFormatter>(element.Formatter);
		////			}

		////			result.Add(handler);
		////		}
		////	}

		////	return result;
		////}

		/// <summary>
		/// Inserts the <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item to insert.</param>
		protected override void InsertItem(int index, BaseHandler item)
		{
			Arg.IsNotNull(item, "item");
			item.Settings = _settings;
			base.InsertItem(index, item);
		}

		/// <summary>
		/// Changes the <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item to insert.</param>
		protected override void SetItem(int index, BaseHandler item)
		{
			Arg.IsNotNull(item, "item");
			item.Settings = _settings;
			base.SetItem(index, item);
		}

		#endregion
	}
}