using System;
using System.Collections.Generic;
using System.Threading;

namespace Adenson.ListenQuest
{
	sealed class BackgroundThread
	{
		#region Variables
		private static List<BackgroundThread> createdThreads = new List<BackgroundThread>();
		Thread thread;
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
		public event EventHandler Aborted;
		#endregion
		#region Methods

		public bool IsAlive
		{
			get { return thread.IsAlive; }
		}
		public void Start()
		{
			thread.Start();
		}
		public void Start(object parameter)
		{
			thread.Start(parameter);
		}
		public void Abort()
		{
			thread.Abort();
			if (this.Aborted != null) this.Aborted(this, EventArgs.Empty);
		}
		public void Abort(object stateInfo)
		{
			thread.Abort(stateInfo);
		}

		public static BackgroundThread Create(ThreadStart ts)
		{
			BackgroundThread th = new BackgroundThread(ts);
			createdThreads.Add(th);
			th.Start();
			return th;
		}
		public static void Pool(WaitCallback waitCallback)
		{
			System.Threading.ThreadPool.SetMaxThreads(1, 1);
			ThreadPool.QueueUserWorkItem(waitCallback);
		}
		public static void AbortThreads()
		{
			foreach (BackgroundThread th in createdThreads)
			{
				if (th.IsAlive) th.Abort();
			}
		}

		#endregion
	}
}