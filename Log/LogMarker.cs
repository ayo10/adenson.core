using System;
using System.Collections.Generic;
using System.Linq;

namespace Adenson.Log
{
	/// <summary>
	/// Represents a log marker
	/// </summary>
	public sealed class LogMarker : IDisposable
	{
		#region Variables
		private List<KeyValuePair<double, string>> marks = new List<KeyValuePair<double, string>>();
		private TimeSpan? lastmark;
		private LogProfiler _profiler;
		#endregion
		#region Constructor

		internal LogMarker(LogProfiler profiler)
		{
			_profiler = profiler;
			lastmark = DateTime.Now.TimeOfDay;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Records the time between when the last mark was called and this one.
		/// </summary>
		/// <remarks>The name of the method MIGHT change.</remarks>
		public void Mark()
		{
			this.Mark(null);
		}

		/// <summary>
		/// Records the time between when the last mark was called and this one.
		/// </summary>
		/// <remarks>The name of the method MIGHT change.</remarks>
		/// <param name="message">A message to display.</param>
		/// <param name="args">The arguments to pass along with message.</param>
		public void Mark(string message, params object[] args)
		{
			lock (marks)
			{
				double key = DateTime.Now.TimeOfDay.Subtract(lastmark.Value).TotalSeconds;
				string value = StringUtil.Format(message, args);
				KeyValuePair<double, string> kv = new KeyValuePair<double, string>(key, value);
				marks.Add(kv);
				lastmark = DateTime.Now.TimeOfDay;
			}
		}

		/// <summary>
		/// Effectively ends the marker.
		/// </summary>
		public void Dispose()
		{
			lock (marks)
			{
				if (marks.Count > 0)
				{
					var e = marks.Select(s => s.Key).OrderBy(s => s).ToList();
					double avg = e.Average();
					double min = e.First();
					double max = e.Last();
					string ns = marks.Where(k => k.Key == min).First().Value;
					string xs = marks.Where(k => k.Key == max).First().Value;

					_profiler.Debug("Markers:");
					_profiler.Debug("  Count: {0}", marks.Count);
					_profiler.Debug("  Avg: {0}s", Logger.Round(avg));
					_profiler.Debug("  Max: {0}s, {1}", Logger.Round(max), ns ?? "--");
					_profiler.Debug("  Min: {0}s, {1}", Logger.Round(min), xs ?? "--");
				}
				else
				{
					_profiler.Debug("No Markers");
				}
			}
		}

		#endregion
	}
}