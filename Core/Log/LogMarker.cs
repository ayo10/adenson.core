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
		private List<Tuple<long, string>> marks = new List<Tuple<long, string>>();
		private LogProfiler _profiler;
		private long lastTick;
		#endregion
		#region Constructor

		internal LogMarker(LogProfiler profiler)
		{
			_profiler = profiler;
			lastTick = profiler.Stopwatch.ElapsedTicks;
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
			long last = _profiler.Stopwatch.ElapsedTicks;
			long key = last - lastTick;
			Tuple<long, string> kv = new Tuple<long, string>(key, message);
			marks.Add(kv);
			lastTick = last;
		}

		/// <summary>
		/// Effectively ends the marker.
		/// </summary>
		public void Dispose()
		{
			if (marks.Count > 0)
			{
				var e = marks.Select(s => s.Item1).OrderBy(s => s).ToList();
				long avg = (long)e.Average();
				long first = e.First();
				long last = e.Last();
				string ns = marks.Where(k => k.Item1 == first).First().Item2;
				string xs = marks.Where(k => k.Item1 == last).First().Item2;
				_profiler.Debug("Markers:");
				_profiler.Debug($"  Count: {marks.Count}");
				_profiler.Debug($"  Avg: {Logger.Round(TimeSpan.FromTicks(avg))}");
				_profiler.Debug($"  Max: {Logger.Round(TimeSpan.FromTicks(last))}, {ns ?? "--"}");
				_profiler.Debug($"  Min: {Logger.Round(TimeSpan.FromTicks(first))}, {xs ?? "--"}");
			}
			else
			{
				_profiler.Debug("No Markers");
			}
		}

		#endregion
	}
}