using System;
using System.Xml.Linq;
using Adenson.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adenson.CoreTest.Configuration
{
	[TestClass]
	public class ConfigHelperTest
	{
		[TestMethod]
		public void GetSectionTest1()
		{
			var section = ConfigHelper.GetSection<XDocument>("adenson/loggerSettings");
			Assert.IsNotNull(section);
			Assert.AreEqual("<loggerSettings />", section.Root.ToString());
		}

		[TestMethod]
		public void GetSectionTest2()
		{
			var section = ConfigHelper.GetSection<XDocument>("adenson", "loggerSettings");
			Assert.IsNotNull(section);
			Assert.AreEqual("<loggerSettings />", section.Root.ToString());
		}

		[TestMethod]
		public void GetValueTest1()
		{
			Assert.AreEqual("Test1", ConfigHelper.GetValue<string>("Test1"));
			Assert.AreEqual(2, ConfigHelper.GetValue<int>("Test2"));
		}

		[TestMethod]
		public void GetValueTest2()
		{
			Assert.AreEqual("Test1", ConfigHelper.GetValue<string>("Test1", "Woot"));
			Assert.AreEqual(2, ConfigHelper.GetValue<int>("Test2", 3));

			Assert.AreEqual("Woot", ConfigHelper.GetValue<string>("Test3", "Woot"));
			Assert.AreEqual(3, ConfigHelper.GetValue<int>("Test3", 3));
		}
	}
}