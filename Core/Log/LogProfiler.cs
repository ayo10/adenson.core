using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Adenson.Log
{
	/// <summary>
	/// Represetns a log profiler object, active only when the parent Logger object's severity is Debug or lower
	/// </summary>
	public sealed class LogProfiler : IDisposable
	{
		#region Variables
		private long memoryStart;
		private List<LogMarker> markers = new List<LogMarker>();
		#endregion
		#region Constructor

		internal LogProfiler(Logger parent, string identifier)
		{
			memoryStart = GC.GetTotalMemory(false);
			this.Stopwatch = new Stopwatch();
			this.Parent = parent;
			this.Identifier = identifier;
			this.Uid = Guid.NewGuid();
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether to append the identifier to the log.
		/// </summary>
		public bool AppendIdentifier
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the elapsed time between when the object was initialzied and this property is called.
		/// </summary>
		public TimeSpan Elapsed
		{
			get
			{
				if (this.IsDisposed)
				{
					throw new ObjectDisposedException("LogProfiler");
				}

				return this.Stopwatch.Elapsed;
			}
		}

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		public string Identifier
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether the object has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the parent <see cref="Logger"/> object.
		/// </summary>
		public Logger Parent
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the number of bytes currently thought to be allocated.
		/// </summary>
		public long TotalMemory
		{
			get { return GC.GetTotalMemory(false) - memoryStart; }
		}

		/// <summary>
		/// Gets the unique identifier for the profiler.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uid", Justification = "I love uid")]
		public Guid Uid
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a predicate that is invoked when the profilfer is disposed.
		/// </summary>
		public Action Disposed { get; internal set; }

		internal Stopwatch Stopwatch { get; private set; }

		#endregion
		#region Methods

		/// <summary>
		/// Called to log errors of type Debug.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <exception cref="ArgumentNullException">If message is null or whitespace</exception>
		[Conditional("DEBUG")]
		public void Debug(object message)
		{
			this.Parent.Debug(message);
		}

		/// <summary>
		/// Effectively ends the profiler.
		/// </summary>
		public void Dispose()
		{
			this.Stopwatch.Stop();
			this.Parent.Write(Severity.Debug, $"{this.Identifier}: {Logger.Round(this.Elapsed)}");
			this.Disposed?.Invoke();
			this.IsDisposed = true;
		}

		/// <summary>
		/// Starts a new log marker (measures the length of time between <see cref="LogMarker.Mark()"/> calls, and on dispose, averages and displays them (along with longest and shortest run.
		/// </summary>
		/// <remarks>Timer starts when the method is invoked</remarks>
		/// <remarks>The name of the method MIGHT change</remarks>
		/// <returns>A new marker instance.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024", Justification = "Not appropriate as a property.")]
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public LogMarker GetMarker()
		{
			LogMarker marker = new LogMarker(this);
			lock (markers)
			{
				markers.Add(marker);
			}

			return marker;
		}

		#endregion
	}
}