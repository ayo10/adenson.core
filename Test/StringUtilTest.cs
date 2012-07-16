using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adenson.CoreTest
{
	[TestClass]
	public class StringUtilTest
	{
		[TestMethod]
		public void FormatTest()
		{
			Assert.AreEqual("Test1 Test2", StringUtil.Format("Test1 {0}", "Test2"));
			Assert.AreEqual("Test1 Test2 {1}", StringUtil.Format("Test1 {0} {1}", "Test2"));
			Assert.AreEqual("Test1 {1}", StringUtil.Format("Test1 {1}", "Test2"));
		}

		[TestMethod]
		public void GenerateRandomStringTest()
		{
			int length = 10;
			var target = StringUtil.GenerateRandomString(length);
			Assert.IsFalse(String.IsNullOrEmpty(target));
			Assert.AreEqual(length, target.Length);
		}

		[TestMethod]
		public void IsNullOrWhiteSpaceTest()
		{
			Assert.IsTrue(StringUtil.IsNullOrWhiteSpace(String.Empty));
			Assert.IsTrue(StringUtil.IsNullOrWhiteSpace(" "));
			Assert.IsFalse(StringUtil.IsNullOrWhiteSpace(" Test "));
		}

		[TestMethod]
		public void ToStringTest1()
		{
			byte[] buffer = null;
			Assert.AreEqual(null, StringUtil.ToString(buffer));
			
			buffer = new byte[] { 1, 2, 3 };
			Assert.AreEqual(Convert.ToBase64String(buffer), StringUtil.ToString(buffer));
		}

		[TestMethod]
		public void ToStringTest2()
		{
			object value = null;
			Assert.AreEqual(null, StringUtil.ToString(value));

			value = "test";
			Assert.AreEqual(value.ToString(), StringUtil.ToString(value));

			value = 5;
			Assert.AreEqual(value.ToString(), StringUtil.ToString(value));

			value = new Exception("Test1");
			Assert.AreEqual("Test1.", StringUtil.ToString(value));

			var buffer = new byte[] { 1, 2, 3 };
			Assert.AreEqual(Convert.ToBase64String(buffer), StringUtil.ToString(buffer));
		}
	}
}