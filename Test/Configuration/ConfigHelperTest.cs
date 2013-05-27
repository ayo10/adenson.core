using System;
using System.Xml.Linq;
using Adenson.Configuration;
using NUnit.Framework;

namespace Adenson.CoreTest.Configuration
{
	[TestFixture]
	public class ConfigHelperTest
	{
		[Test]
		public void GetSectionTest1()
		{
			var section = ConfigHelper.GetSection<XDocument>("adenson/testSettings");
			Assert.IsNotNull(section);
			Assert.AreEqual("<testSettings />", section.Root.ToString());
		}

		[Test]
		public void GetSectionTest2()
		{
			var section = ConfigHelper.GetSection<XDocument>("adenson", "testSettings");
			Assert.IsNotNull(section);
			Assert.AreEqual("<testSettings />", section.Root.ToString());
		}

		[Test]
		public void GetValueTest1()
		{
			Assert.AreEqual("Test1", ConfigHelper.GetValue<string>("Test1"));
			Assert.AreEqual(2, ConfigHelper.GetValue<int>("Test2"));
		}

		[Test]
		public void GetValueTest2()
		{
			Assert.AreEqual("Test1", ConfigHelper.GetValue<string>("Test1", "Woot"));
			Assert.AreEqual(2, ConfigHelper.GetValue<int>("Test2", 3));

			Assert.AreEqual("Woot", ConfigHelper.GetValue<string>("Test3", "Woot"));
			Assert.AreEqual(3, ConfigHelper.GetValue<int>("Test3", 3));
		}
	}
}