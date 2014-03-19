using System;
using System.Linq;
using Adenson.Log;
using NUnit.Framework;

namespace Adenson.CoreTest.Log
{
	[TestFixture]
	public class LoggerTest
	{
		#region Variables
		private TestHandler handler = new TestHandler();
		private Logger testLogger = Logger.Get(typeof(LoggerTest));
		#endregion
		#region Init

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Logger.Settings.Handlers.Add(handler);
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Logger.Settings.Severity = Severity.Error;
			Logger.Settings.Handlers.Remove(handler);
		}
		
		#endregion
		#region Tests

		[Test]
		public void GetLoggerTest()
		{
			Logger logger1 = Logger.Get(this.GetType());
			Assert.IsNotNull(logger1);
			Assert.AreEqual(logger1.ClassType, typeof(LoggerTest));

			Logger logger2 = Logger.Get(this.GetType());
			Assert.IsNotNull(logger2);
			Assert.AreSame(logger1, logger2, "GetLogger should always return the same object per type");
			Assert.AreEqual(logger2.ClassType, typeof(LoggerTest));

			Logger logger3 = Logger.Get(typeof(LogProfilerTest));
			Assert.IsNotNull(logger3);
			Assert.AreNotSame(logger1, logger3, "GetLogger should return a different logger per type.");
			Assert.AreEqual(logger3.ClassType, typeof(LogProfilerTest));
		}

		[Test]
		public void CriticalTest()
		{
			this.TestLog(Severity.Critical);
		}

		[Test]
		public void DebugTest()
		{
			this.TestLog(Severity.Debug);
		}

		[Test]
		public void ErrorTest()
		{
			this.TestLog(Severity.Error);
		}

		[Test]
		public void InfoTest()
		{
			this.TestLog(Severity.Info);
		}

		[Test]
		public void WarnTest()
		{
			this.TestLog(Severity.Warn);
		}

		[Test]
		public void ProfilerTest()
		{
			Logger.Settings.Severity = Severity.Debug;
			int count = handler.Entries.Count;
			var prf = testLogger.GetProfiler("ProfileTest");
			Assert.IsNotNull(prf);
			Assert.AreEqual("ProfileTest", prf.Identifier);

			Assert.AreEqual(++count, handler.Entries.Count);
			Assert.AreEqual("ProfileTest START", this.Strip(handler.Entries.Last().Message));
			
			prf.Debug("Test1");
			Assert.AreEqual(++count, handler.Entries.Count);
			Assert.AreEqual("ProfileTest Test1", this.Strip(handler.Entries.Last().Message));

			var lmk = prf.GetMarker();
			Assert.IsNotNull(lmk);

			prf.Dispose();
			Assert.AreEqual(++count, handler.Entries.Count);
			Assert.AreEqual("ProfileTest FINISH", this.Strip(handler.Entries.Last().Message));

			Assert.Throws<ArgumentNullException>(delegate { testLogger.GetProfiler(null); });
			Assert.Throws<ArgumentNullException>(delegate { testLogger.GetProfiler(String.Empty); });
			Assert.Throws<ArgumentNullException>(delegate { testLogger.GetProfiler(" "); });
		}

		#endregion
		#region Methods

		private void TestLog(Severity severity)
		{
			Logger.Settings.Severity = severity;
			int count = handler.Entries.Count;
			int s = (int)severity;
			for (int i = 5; i > 0; i--)
			{
				switch (i)
				{
					case 1:
						testLogger.Debug("This is a {0} message", (Severity)i);
						break;
					case 2:
						testLogger.Info("This is a {0} message", (Severity)i);
						break;
					case 3:
						testLogger.Warn("This is a {0} message", (Severity)i);
						break;
					case 4:
						testLogger.Error("This is a {0} message", (Severity)i);
						break;
					case 5:
						testLogger.Critical("This is a {0} message", (Severity)i);
						break;
				}

				if (i >= (int)severity)
				{
					count++;
					Assert.AreEqual(count, handler.Entries.Count);
					if (i == (int)severity)
					{
						LogEntry log = handler.Entries.Last();
						DateTime dt1 = DateTime.Now;
						DateTime dt2 = log.Date;
						Assert.AreEqual(String.Format("This is a {0} message", severity), log.Message);
						Assert.AreEqual(new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second), new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second));
						Assert.AreEqual(severity, log.Severity);
						Assert.AreEqual(typeof(LoggerTest).Name, log.TypeName);
					}
				}
				else
				{
					Assert.AreEqual(count, handler.Entries.Count, "No change in entries, higher severity");
				}
			}
		}

		private string Strip(object message)
		{
			var sp = message.ToString().Split('[', ']');
			Assert.AreEqual(3, sp.Length);
			Assert.AreEqual(String.Empty, sp[0]);
			Assert.IsTrue(sp[1].EndsWith("s"));
			Assert.DoesNotThrow(delegate { Assert.IsTrue(double.Parse(sp[1].Substring(0, sp[1].Length - 1).Trim()) >= 0); });
			return String.Join(String.Empty, sp.Skip(2).ToArray()).Trim();
		}

		#endregion
	}
}