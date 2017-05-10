using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace Adenson.CoreTest.System.Xml.Linq
{
	[TestFixture]
	public class ExtensionsTest
	{
		[Test]
		public void ElementTest()
		{
			XContainer source = XDocument.Parse("<eLem>1</eLem>");
			Assert.AreEqual("eLem", source.Element("ELEM", StringComparison.CurrentCultureIgnoreCase).Name.LocalName);
		}

		[Test]
		public void GetValueTest()
		{
			XElement source = XDocument.Parse("<root><elem>1</elem></root>").Root;
			Assert.AreEqual(1, source.GetValue<int>("elem"));

			source = XDocument.Parse("<root><elem>1</elem></root>").Root;
			Assert.AreEqual(1, source.GetValue<int>("elem"));

			source = XDocument.Parse("<elem attrib1=\"1\" attrib2=\"some string\" />").Root;
			Assert.AreEqual(1, source.GetValue<int>("attrib1"));
			Assert.AreEqual("some string", source.GetValue("attrib2"));
		}

		[Test]
		public void HasElementTest1()
		{
			XContainer source = XDocument.Parse("<elem>1</elem>");
			Assert.IsTrue(source.HasElement("elem"));
			Assert.IsFalse(source.HasElement("ELEM"));
		}

		[Test]
		public void HasElementTest2()
		{
			XContainer source = XDocument.Parse("<elem>1</elem>");
			Assert.IsTrue(source.HasElement("elem", StringComparison.CurrentCulture));
			Assert.IsTrue(source.HasElement("ELEM", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(source.HasElement("ELEM", StringComparison.CurrentCulture));
		}
	}
}
