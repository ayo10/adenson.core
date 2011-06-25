using System;
using Adenson.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adenson.CoreTest.Log
{
	[TestClass]
	public class LoggerTest
	{
		[TestMethod]
		public void ConvertToStringTest1()
		{
			Exception exception = new Exception("Test1", new Exception("Test2"));
			string expected = "Test1. Test2.";
			string actual = Logger.ConvertToString(exception, true);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ConvertToStringTest2a()
		{
			Exception exception = new Exception("Test1", new Exception("Test2"));
			string expected = "System.Exception: Test1\r\n--------------------\r\nSystem.Exception: Test2\r\n";
			string actual = Logger.ConvertToString(exception);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
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
			string actual = Logger.ConvertToString(exception1, false);
			Assert.AreEqual(expected, actual);
		}
	}
}