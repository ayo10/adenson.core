using System;
using System.Diagnostics.CodeAnalysis;

namespace Adenson.Log
{
	/// <summary>
	/// Represetns a log profiler object
	/// </summary>
	public sealed class LogProfiler : IDisposable
	{
		#region Constructor

		internal LogProfiler(Logger parent)
		{
			this.Start = DateTime.Now;
			this.Parent = parent;
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
			set;
		}

		#endregion
		#region Methods

		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Perfectly happy with this implementation.")]
		void IDisposable.Dispose()
		{
			this.IsDisposed = true;
			this.Parent.MeasureStop(this.Uid);
		}

		#endregion
	}
}