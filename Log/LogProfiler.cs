using System;
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
		#endregion
		#region Constructor

		internal LogProfiler(Logger parent, string identifier)
		{
			memoryStart = GC.GetTotalMemory(false);
			this.Start = DateTime.Now;
			this.Parent = parent;
			this.Identifier = identifier;
			this.Uid = Guid.NewGuid();
			this.Debug(SR.ProfilerStart);
		}

		#endregion
		#region Properties
		
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

				return DateTime.Now.Subtract(this.Start);
			}
		}

		/// <summary>
		/// Gets the identifier
		/// </summary>
		public string Identifier
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether the object has been disposed
		/// </summary>
		public bool IsDisposed
		{
			get;
			private set;
		}
	
		/// <summary>
		/// Gets the parent <see cref="Logger"/> object
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
		/// Gets the unique identifier for the profiler
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uid", Justification = "I love uid")]
		public Guid Uid
		{
			get;
			private set;
		}

		private DateTime Start
		{
			get;
			set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Called to log errors of type Debug
		/// </summary>
		/// <param name="message">Message to log</param>
		/// <param name="arguments">Arguments, if any to format message</param>
		/// <exception cref="ArgumentNullException">if message is null or whitespace</exception>
		public void Debug(string message, params object[] arguments)
		{
			this.Parent.Write(LogSeverityInternal.Profiler, "[{0}s] {1} {2}", this.Elapsed.TotalSeconds.ToString("0.000000", System.Globalization.CultureInfo.CurrentCulture), this.Identifier, (message == null ? String.Empty : StringUtil.Format(message, arguments)));
		}

		/// <summary>
		/// Effectively ends the profiler
		/// </summary>
		public void Dispose()
		{
			this.Debug(SR.ProfilerStop);
			this.Parent.ProfilerStop(this.Uid);
			this.IsDisposed = true;
		}

		#endregion
	}
}