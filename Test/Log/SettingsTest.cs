using System;
using System.Configuration;
using Adenson.Log;
using NUnit.Framework;

namespace Adenson.CoreTest.Log
{
	[TestFixture]
	public class SettingsTest
	{
		#region Tests

		[Test]
		public void TestCurrentValues()
		{
			Assert.IsNotNull(Logger.Settings);
			Assert.IsNotNull(Logger.Settings.Formatter);
			Assert.IsInstanceOf<DefaultFormatter>(Logger.Settings.Formatter);
			Assert.AreEqual(1, Logger.Settings.Handlers.Count);
			Assert.IsInstanceOf<TraceHandler>(Logger.Settings.Handlers[0]);
			Assert.AreEqual(Severity.Error, Logger.Settings.Severity);

			Assert.Throws<ArgumentNullException>(delegate { Logger.Settings.Formatter = null; });
			Assert.Throws<ArgumentNullException>(delegate { Logger.Settings.Handlers.Add(null); });
		}

		[Test]
		public void TestEmptyConfig()
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "log/empty.config" };
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			Settings settings = Settings.FromConfig(config.GetSection("adenson/logSettings"));
			Assert.IsNotNull(settings);
			Assert.IsNotNull(settings.Formatter);
			Assert.IsInstanceOf<DefaultFormatter>(settings.Formatter);
			Assert.AreEqual(1, settings.Handlers.Count);
			Assert.IsInstanceOf<TraceHandler>(settings.Handlers[0]);
			Assert.AreEqual(Severity.Error, settings.Severity);

			Assert.Throws<ArgumentNullException>(delegate { settings.Formatter = null; });
			Assert.Throws<ArgumentNullException>(delegate { settings.Handlers.Add(null); });
		}

		[Test]
		public void TestFullConfig()
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "log/full.config" };
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			Settings settings = Settings.FromConfig(config.GetSection("adenson/logSettings"));
			this.TestFullConfig(settings);
		}

		[Test]
		public void TestFullNoRootConfig()
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "log/fullnoroot.config" };
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			Settings settings = Settings.FromConfig(config.GetSection("logSettings"));
			this.TestFullConfig(settings);
		}

		[Test]
		public void TestNoneConfig()
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "log/none.config" };
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			Settings settings = Settings.FromConfig(config.GetSection("adenson/logSettings"));
			Assert.IsNotNull(settings);
			Assert.IsNotNull(settings.Formatter);
			Assert.IsInstanceOf<DefaultFormatter>(settings.Formatter);
			Assert.AreEqual(1, settings.Handlers.Count);
			Assert.IsInstanceOf<TraceHandler>(settings.Handlers[0]);
			Assert.AreEqual(Severity.Error, settings.Severity);

			Assert.Throws<ArgumentNullException>(delegate { settings.Formatter = null; });
			Assert.Throws<ArgumentNullException>(delegate { settings.Handlers.Add(null); });
		}

		[Test]
		public void TestNullConfig()
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "log/none.config" };
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			Settings settings = Settings.FromConfig(config.GetSection("adenson/logSettings"));
			Assert.IsNotNull(settings);
			Assert.IsNotNull(settings.Formatter);
			Assert.IsInstanceOf<DefaultFormatter>(settings.Formatter);
			Assert.AreEqual(1, settings.Handlers.Count);
			Assert.IsInstanceOf<TraceHandler>(settings.Handlers[0]);
			Assert.AreEqual(Severity.Error, settings.Severity);

			Assert.Throws<ArgumentNullException>(delegate { settings.Formatter = null; });
			Assert.Throws<ArgumentNullException>(delegate { settings.Handlers.Add(null); });
		}

		#endregion
		#region Methods

		private void TestFullConfig(Settings settings)
		{
			Assert.IsNotNull(settings);
			Assert.IsNotNull(settings.Formatter);
			Assert.AreEqual(Severity.Debug, settings.Severity);
			Assert.IsInstanceOf<DefaultFormatter>(settings.Formatter);

			Assert.AreEqual(8, settings.Handlers.Count);

			ConsoleHandler consoleHandler = settings.Handlers[0] as ConsoleHandler;
			Assert.IsNotNull(consoleHandler);

			DatabaseHandler databaseHandler = settings.Handlers[1] as DatabaseHandler;
			Assert.IsNotNull(databaseHandler);
			Assert.AreEqual("woot", databaseHandler.Connection);
			Assert.AreEqual("testdate", databaseHandler.DateColumn);
			Assert.AreEqual("testmessage", databaseHandler.MessageColumn);
			Assert.AreEqual("testseverity", databaseHandler.SeverityColumn);
			Assert.AreEqual("testname", databaseHandler.TableName);
			Assert.AreEqual("testtype", databaseHandler.TypeColumn);
			Assert.AreEqual(settings.Formatter, databaseHandler.Formatter);

			DebugHandler debugHandler = settings.Handlers[2] as DebugHandler;
			Assert.IsNotNull(debugHandler);

			EmailHandler emailHandler = settings.Handlers[3] as EmailHandler;
			Assert.IsNotNull(emailHandler);
			Assert.AreEqual("woot@woot", emailHandler.To);
			Assert.IsInstanceOf<TestFormatter>(emailHandler.Formatter);

			EventLogHandler eventLogHandler = settings.Handlers[4] as EventLogHandler;
			Assert.IsNotNull(eventLogHandler);
			Assert.AreEqual("Woot", eventLogHandler.Source);

			FileHandler fileHandler = settings.Handlers[5] as FileHandler;
			Assert.IsNotNull(fileHandler);
			Assert.AreEqual("filename.woot", fileHandler.FilePath);

			TraceHandler traceHandler = settings.Handlers[6] as TraceHandler;
			Assert.IsNotNull(traceHandler);
			Assert.IsInstanceOf<TestFormatter>(traceHandler.Formatter);

			TestHandler testHandler = settings.Handlers[7] as TestHandler;
			Assert.IsNotNull(testHandler);

			Assert.Throws<ArgumentNullException>(delegate { settings.Formatter = null; });
			Assert.Throws<ArgumentNullException>(delegate { settings.Handlers.Add(null); });
		}

		#endregion
	}
}