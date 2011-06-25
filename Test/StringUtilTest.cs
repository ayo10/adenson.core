using Adenson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Adenson.CoreTest
{
    
    
    /// <summary>
    ///This is a test class for StringUtilTest and is intended
    ///to contain all StringUtilTest Unit Tests
    ///</summary>
	[TestClass]
	public class StringUtilTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for Format
		///</summary>
		[TestMethod]
		public void FormatTest()
		{
			string value = string.Empty; // TODO: Initialize to an appropriate value
			object arg = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = StringUtil.Format(value, arg);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Format
		///</summary>
		[TestMethod]
		public void FormatTest1()
		{
			string value = string.Empty; // TODO: Initialize to an appropriate value
			object[] args = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = StringUtil.Format(value, args);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GenerateRandomString
		///</summary>
		[TestMethod]
		public void GenerateRandomStringTest()
		{
			int length = 0; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = StringUtil.GenerateRandomString(length);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for IsNullOrWhiteSpace
		///</summary>
		[TestMethod]
		public void IsNullOrWhiteSpaceTest()
		{
			string value = string.Empty; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = StringUtil.IsNullOrWhiteSpace(value);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod]
		public void ToStringTest()
		{
			object value = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = StringUtil.ToString(value);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod]
		public void ToStringTest1()
		{
			byte[] buffer = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = StringUtil.ToString(buffer);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
