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
		private List<Tuple<double, string>> marks = new List<Tuple<double, string>>();
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
		public void Mark(string message)
		{
			double key = DateTime.Now.TimeOfDay.Subtract(lastmark.Value).TotalSeconds;
			Tuple<double, string> kv = new Tuple<double, string>(key, message);
			marks.Add(kv);
			lastmark = DateTime.Now.TimeOfDay;
		}

		/// <summary>
		/// Effectively ends the marker.
		/// </summary>
		public void Dispose()
		{
			if (marks.Count > 0)
			{
				var e = marks.Select(s => s.Item1).OrderBy(s => s).ToList();
				double avg = e.Average();
				double min = e.First();
				double max = e.Last();
				string ns = marks.Where(k => k.Item1 == min).First().Item2;
				string xs = marks.Where(k => k.Item1 == max).First().Item2;
				_profiler.Debug("Markers:");
				_profiler.Debug($"  Count: {marks.Count}");
				_profiler.Debug($"  Avg: {Logger.Round(avg)}");
				_profiler.Debug($"  Max: {Logger.Round(max)}, {ns ?? "--"}");
				_profiler.Debug($"  Min: {Logger.Round(min)}, {xs ?? "--"}");
			}
			else
			{
				_profiler.Debug("No Markers");
			}
		}

		#endregion
	}
}