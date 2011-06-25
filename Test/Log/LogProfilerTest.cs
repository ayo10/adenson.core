using System;
using System.Collections.Generic;
using Adenson.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adenson.CoreTest.Log
{
	[TestClass]
	public class LogProfilerTest
	{
		[TestMethod]
		public void DebugTest()
		{
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				prf.Debug("Test {0}", "test");
			}
		}

		[TestMethod]
		public void DisposeTest()
		{
			LogProfiler prf;
			using (prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.IsFalse(prf.IsDisposed);
			}
			Assert.IsTrue(prf.IsDisposed);
		}

		[TestMethod()]
		public void ElapsedTimeTest()
		{
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				var els = prf.Elapsed;
				Assert.IsTrue(els.TotalSeconds >= 0 && els.TotalSeconds <= 1);
				System.Threading.Thread.Sleep(1000);
				els = prf.Elapsed;
				Assert.IsTrue(els.TotalSeconds > 1 && els.TotalSeconds <= 2);
			}
		}

		[TestMethod]
		public void IdentifierTest()
		{
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.AreEqual("Test", prf.Identifier);
			}
		}

		[TestMethod]
		public void IsDisposedTest()
		{
			LogProfiler prf;
			using (prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.IsFalse(prf.IsDisposed);
			}
			Assert.IsTrue(prf.IsDisposed);
		}

		[TestMethod]
		public void ParentTest()
		{
			var logger = Logger.GetLogger(this.GetType());
			using (var prf = logger.ProfilerStart("Test"))
			{
				Assert.AreEqual(logger, prf.Parent);
			}
		}

		[TestMethod]
		public void TotalMemoryTest()
		{
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				byte[] buffer = new byte[1000000];
				Assert.IsTrue(prf.TotalMemory > 0);
			}
		}

		[TestMethod()]
		public void UidTest()
		{
			var uids = new List<Guid>();
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.AreNotEqual(Guid.Empty, prf.Uid);
				uids.Add(prf.Uid);
			}
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				uids.Add(prf.Uid);
			}
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				uids.Add(prf.Uid);
			}

			CollectionAssert.AllItemsAreUnique(uids);
		}
	}
}