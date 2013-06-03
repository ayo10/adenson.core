using System;
using NUnit.Framework;

namespace Adenson.CoreTest.Sys
{
	[TestFixture]
	public class StringUtilTest
	{
		[Test]
		public void FormatTest()
		{
			Assert.AreEqual("Test1 Test2", StringUtil.Format("Test1 {0}", "Test2"));
			Assert.AreEqual("Test1 Test2 {1}", StringUtil.Format("Test1 {0} {1}", "Test2"));
			Assert.AreEqual("Test1 {1}", StringUtil.Format("Test1 {1}", "Test2"));
		}

		[Test]
		public void GenerateRandomStringTest()
		{
			int length = 10;
			var target = StringUtil.GenerateRandomString(length);
			Assert.IsFalse(String.IsNullOrEmpty(target));
			Assert.AreEqual(length, target.Length);
		}

		[Test]
		public void IsNullOrWhiteSpaceTest()
		{
			Assert.IsTrue(StringUtil.IsNullOrWhiteSpace(String.Empty));
			Assert.IsTrue(StringUtil.IsNullOrWhiteSpace(" "));
			Assert.IsFalse(StringUtil.IsNullOrWhiteSpace(" Test "));
		}

		[Test]
		public void ToStringTest1()
		{
			byte[] buffer = null;
			Assert.AreEqual(null, StringUtil.ToString(buffer));
			
			buffer = new byte[] { 1, 2, 3 };
			Assert.AreEqual(Convert.ToBase64String(buffer), StringUtil.ToString(buffer));
		}

		[Test]
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
		
		[Test]
		public void ConvertToStringTest1()
		{
			Exception exception = new Exception("Test1", new Exception("Test2"));
			string expected = "Test1. Test2.";
			string actual = StringUtil.ToString(exception, true);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ConvertToStringTest2a()
		{
			Exception exception = new Exception("Test1", new Exception("Test2"));
			string expected = "System.Exception: Test1\r\n--------------------\r\nSystem.Exception: Test2\r\n";
			string actual = StringUtil.ToString(exception);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ConvertToStringTest2b()
		{
			Exception exception1 = new Exception("Test1");
			Exception exception2;
			try
			{
				throw exception1;//<==This line number should be the same as line number below for this unit test to pass.
			}

			catch (Exception ex)
			{
				exception2 = new Exception("Test2", ex);
			}

			string expected = "System.Exception: Test1\r\n   at Adenson.CoreTest.Log.LoggerTest.ConvertToStringTest2b() in C:\\Projects\\Adenson\\Adenson.Core\\Test\\Log\\LoggerTest.cs:line 35\r\n";
			string actual = StringUtil.ToString(exception1, false);
			Assert.AreEqual(expected, actual);
		}
	}
}