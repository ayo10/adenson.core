using System;
using System.Diagnostics.CodeAnalysis;

namespace Adenson.Log
{
	/// <summary>
	/// Represetns a log profiler object, active only when the parent Logger object's severity is Debug or lower
	/// </summary>
	public sealed class LogProfiler : IDisposable
	{
		#region Constructor

		internal LogProfiler(Logger parent, string identifier)
		{
			this.Start = DateTime.Now;
			this.Parent = parent;
			this.Identifier = identifier;
			this.Uid = Guid.NewGuid();
		}

		#endregion
		#region Properties
		
		/// <summary>
		/// Gets the elapsed time (in total seconds) between when the object was initialzied and this property is called.
		/// </summary>
		public double ElapsedTime
		{
			get
			{
				if (this.IsDisposed) throw new ObjectDisposedException("LogProfiler");
				return DateTime.Now.Subtract(this.Start).TotalSeconds.Round(6);
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
		/// Gets if the object has been disposed
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
		/// Gets when the profiler was initialized
		/// </summary>
		public DateTime Start
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the unique identifier for the profiler
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uid")]
		public Guid Uid
		{
			get;
			private set;
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
			this.Parent.Debug("[{0}s] {1} {2}", this.ElapsedTime.ToString(8, '0'), this.Identifier, (message == null ? String.Empty : StringUtil.Format(message, arguments)));
		}

		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Perfectly happy with this implementation.")]
		void IDisposable.Dispose()
		{
			this.Parent.ProfilerStop(this.Uid);
			this.IsDisposed = true;
		}

		#endregion
	}
}