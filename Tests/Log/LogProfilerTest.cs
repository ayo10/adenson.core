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
			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				prf.Debug("Test {0}", guid);
			}

			Assert.AreEqual(3, handler.Entries.Count);
			Assert.IsTrue(handler.Entries[0].Message.ToString().EndsWith("s] Test START"));
			Assert.IsTrue(handler.Entries[1].Message.ToString().EndsWith(String.Format("s] Test Test {0}", guid)));
			Assert.IsTrue(handler.Entries[2].Message.ToString().EndsWith("s] Test FINISH"));

			Logger.Settings.Severity = Severity.Info;
			handler.Entries.Clear();
			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				prf.Debug("Test {0}", guid);
			}

			Assert.AreEqual(0, handler.Entries.Count);
		}

		[Test]
		public void DisposeTest()
		{
			LogProfiler prf;
			using (prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				Assert.IsFalse(prf.IsDisposed);
			}

			Assert.IsTrue(prf.IsDisposed);
		}

		[Test]
		public void ElapsedTimeTest()
		{
			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				var els = prf.Elapsed;
				Assert.IsTrue(els.TotalSeconds >= 0 && els.TotalSeconds <= 1);
				global::System.Threading.Thread.Sleep(1000);
				els = prf.Elapsed;
				Assert.IsTrue(els.TotalSeconds > 1 && els.TotalSeconds <= 2);
			}
		}

		[Test]
		public void IdentifierTest()
		{
			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				Assert.AreEqual("Test", prf.Identifier);
			}
		}

		[Test]
		public void IsDisposedTest()
		{
			LogProfiler prf;
			using (prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				Assert.IsFalse(prf.IsDisposed);
			}

			Assert.IsTrue(prf.IsDisposed);
		}

		[Test]
		public void ParentTest()
		{
			var logger = Logger.Get(this.GetType());
			using (var prf = logger.GetProfiler("Test"))
			{
				Assert.AreEqual(logger, prf.Parent);
			}
		}

		[Test]
		public void TotalMemoryTest()
		{
			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
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
			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				Assert.AreNotEqual(Guid.Empty, prf.Uid);
				uids.Add(prf.Uid);
			}

			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				uids.Add(prf.Uid);
			}

			using (var prf = Logger.Get(this.GetType()).GetProfiler("Test"))
			{
				uids.Add(prf.Uid);
			}

			CollectionAssert.AllItemsAreUnique(uids);
		}
	}
}