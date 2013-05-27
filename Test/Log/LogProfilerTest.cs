using System;
using System.Collections.Generic;
using Adenson.Log;
using NUnit.Framework;

namespace Adenson.CoreTest.Log
{
	[TestFixture]
	public class LogProfilerTest
	{
		private TestHandler handler = new TestHandler();
		
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Logger.Settings.Severity = Severity.Debug;
			Logger.Settings.Handlers.Add(handler);
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Logger.Settings.Severity = Severity.Error;
			Logger.Settings.Handlers.Remove(handler);
		}

		[Test]
		public void DebugTest()
		{
			handler.Entries.Clear();
			Guid guid = Guid.NewGuid();
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				prf.Debug("Test {0}", guid);
			}

			Assert.AreEqual(3, handler.Entries.Count);
			Assert.IsTrue(handler.Entries[0].Message.EndsWith("s] Test START"));
			Assert.IsTrue(handler.Entries[1].Message.EndsWith(String.Format("s] Test Test {0}", guid)));
			Assert.IsTrue(handler.Entries[2].Message.EndsWith("s] Test FINISH"));
		}

		[Test]
		public void DisposeTest()
		{
			LogProfiler prf;
			using (prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.IsFalse(prf.IsDisposed);
			}

			Assert.IsTrue(prf.IsDisposed);
		}

		[Test]
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

		[Test]
		public void IdentifierTest()
		{
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.AreEqual("Test", prf.Identifier);
			}
		}

		[Test]
		public void IsDisposedTest()
		{
			LogProfiler prf;
			using (prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				Assert.IsFalse(prf.IsDisposed);
			}

			Assert.IsTrue(prf.IsDisposed);
		}

		[Test]
		public void ParentTest()
		{
			var logger = Logger.GetLogger(this.GetType());
			using (var prf = logger.ProfilerStart("Test"))
			{
				Assert.AreEqual(logger, prf.Parent);
			}
		}

		[Test]
		public void TotalMemoryTest()
		{
			using (var prf = Logger.GetLogger(this.GetType()).ProfilerStart("Test"))
			{
				byte[] buffer = new byte[1000000];
				Assert.IsTrue(prf.TotalMemory > 0);
				buffer = null;
			}
		}

		[Test]
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