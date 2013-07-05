using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Adenson.Log
{
	/// <summary>
	/// Collection of handler objects.
	/// </summary>
	public sealed class HandlerCollection : Collection<BaseHandler>
	{
		private Settings _settings;
		internal HandlerCollection(Settings settings)
		{
			_settings = settings;
		}

		internal static HandlerCollection FromConfig(Settings settings, SettingsConfiguration.HandlerElementCollection handlers)
		{
			HandlerCollection result = new HandlerCollection(settings);
			if (handlers == null || handlers.Count == 0)
			{
				result.Add(new TraceHandler());
			}
			else
			{
				foreach (SettingsConfiguration.HandlerElement element in handlers)
				{
					BaseHandler handler = null;
					switch (element.Handler)
					{
						case HandlerType.Console:
							handler = new ConsoleHandler();
							break;
						case HandlerType.Database:
							handler = new DatabaseHandler(element);
							break;
						case HandlerType.Debug:
							handler = new DebugHandler();
							break;
						case HandlerType.Email:
							handler = new EmailHandler(element);
							break;
						case HandlerType.EventLog:
							handler = new EventLogHandler(element);
							break;
						case HandlerType.File:
							handler = new FileHandler(element);
							break;
						case HandlerType.Trace:
							handler = new TraceHandler();
							break;
						case HandlerType.Custom:
							handler = TypeUtil.CreateInstance<BaseHandler>(element.CustomType);
							break;
					}
;
					if (!String.IsNullOrWhiteSpace(element.Formatter))
					{
						handler.Formatter = TypeUtil.CreateInstance<BaseFormatter>(element.Formatter);
					}

					result.Add(handler);
				}
			}

			return result;
		}

		/// <summary>
		/// Inserts the <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item to insert.</param>
		protected override void InsertItem(int index, BaseHandler item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

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
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			item.Settings = _settings;
			base.SetItem(index, item);
		}
	}
}