using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Threading
{
	/// <summary>
	/// Represents a back ground running task in a thread
	/// </summary>
	public sealed class BackgroundThread
	{
		#region Variables
		private static List<BackgroundThread> createdThreads = new List<BackgroundThread>();
		private Thread thread;
		#endregion
		#region Constructor

		private BackgroundThread(ThreadStart ts)
		{
			thread = new Thread(ts);
			thread.IsBackground = true;
			thread.Priority = ThreadPriority.Lowest;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Occurs when Abort is called on the current thread.
		/// </summary>
		public event EventHandler Aborted;

		/// <summary>
		/// Gets a value indicating whether the thread is alive or not.
		/// </summary>
		public bool IsAlive
		{
			get { return thread.IsAlive; }
		}
		
		#endregion
		#region Methods
		
		/// <summary>
		/// Causes the operating system to change the state of the <see cref="ThreadState"/> used to start the BackgroundThread object.
		/// </summary>
		public void Start()
		{
			thread.Start();
		}

		/// <summary>
		/// Causes the operating system to change the state of the <see cref="ThreadState"/> used to start the BackgroundThread object.
		/// </summary>
		/// <param name="parameter">An object that contains data to be used by the method the thread executes.</param>
		public void Start(object parameter)
		{
			thread.Start(parameter);
		}

		/// <summary>
		/// Aborts this thread by raising a <see cref="ThreadAbortException"/> in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.
		/// </summary>
		public void Abort()
		{
			thread.Abort();
			if (this.Aborted != null)
			{
				this.Aborted(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Aborts this thread by raising a <see cref="ThreadAbortException"/> in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.
		/// </summary>
		/// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted.</param>
		public void Abort(object stateInfo)
		{
			thread.Abort(stateInfo);
		}

		/// <summary>
		/// Creates a new <see cref="BackgroundThread"/> class.
		/// </summary>
		/// <param name="threadStart">A <see cref="ThreadStart"/> delegate that represents the methods to be invoked when the thread begins executing.</param>
		/// <returns>A BackgroundThread object</returns>
		public static BackgroundThread Create(ThreadStart threadStart)
		{
			BackgroundThread th = new BackgroundThread(threadStart);
			createdThreads.Add(th);
			th.Start();
			return th;
		}

		/// <summary>
		///  Queues a method for execution by a <see cref="BackgroundThread"/> object. The method executes when a thread pool thread becomes available.
		/// </summary>
		/// <param name="waitCallback">A <see cref="WaitCallback"/> that represents the method to be executed.</param>
		public static void Pool(WaitCallback waitCallback)
		{
			System.Threading.ThreadPool.SetMaxThreads(1, 1);
			ThreadPool.QueueUserWorkItem(waitCallback);
		}

		/// <summary>
		/// Aborts all threads created by <see cref="Create(ThreadStart)"/> by raising a <see cref="ThreadAbortException"/> in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.
		/// </summary>
		public static void AbortThreads()
		{
			foreach (BackgroundThread th in createdThreads)
			{
				if (th.IsAlive)
				{
					th.Abort();
				}
			}
		}

		/// <summary>
		/// Aborts all threads created by <see cref="Create(ThreadStart)"/> by raising a <see cref="ThreadAbortException"/> in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.
		/// </summary>
		/// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted.</param>
		public static void AbortThreads(object stateInfo)
		{
			foreach (BackgroundThread th in createdThreads)
			{
				if (th.IsAlive)
				{
					th.Abort(stateInfo);
				}
			}
		}

		#endregion
	}
}