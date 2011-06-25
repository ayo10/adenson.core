using Adenson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Adenson.Collections;
using System.Collections.Generic;
using System.IO;

namespace Adenson.CoreTest
{
    
    
    /// <summary>
    ///This is a test class for FileUtilTest and is intended
    ///to contain all FileUtilTest Unit Tests
    ///</summary>
	[TestClass]
	public class FileUtilTest
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
		///A test for CreateFile
		///</summary>
		[TestMethod]
		public void CreateFileTest()
		{
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			byte[] buffer = null; // TODO: Initialize to an appropriate value
			bool overwrite = false; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = FileUtil.CreateFile(filePath, buffer, overwrite);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for CreateMD5HashedFile
		///</summary>
		[TestMethod]
		public void CreateMD5HashedFileTest()
		{
			string directory = string.Empty; // TODO: Initialize to an appropriate value
			byte[] buffer = null; // TODO: Initialize to an appropriate value
			bool overwrite = false; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = FileUtil.CreateMD5HashedFile(directory, buffer, overwrite);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for FixFilePath
		///</summary>
		[TestMethod]
		public void FixFilePathTest()
		{
			string path = string.Empty; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = FileUtil.FixFilePath(path);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetBytes
		///</summary>
		[TestMethod]
		public void GetBytesTest()
		{
			string hexValue = string.Empty; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.GetBytes(hexValue);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetFiles
		///</summary>
		[TestMethod]
		public void GetFilesTest()
		{
			string directory = string.Empty; // TODO: Initialize to an appropriate value
			IEnumerable<string> extensions = null; // TODO: Initialize to an appropriate value
			string[] expected = null; // TODO: Initialize to an appropriate value
			string[] actual;
			actual = FileUtil.GetFiles(directory, extensions);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetIsDirectory
		///</summary>
		[TestMethod]
		public void GetIsDirectoryTest()
		{
			string fullPath = string.Empty; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = FileUtil.GetIsDirectory(fullPath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ReadStream
		///</summary>
		[TestMethod]
		public void ReadStreamTest()
		{
			Uri url = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(url);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ReadStream
		///</summary>
		[TestMethod]
		public void ReadStreamTest1()
		{
			Stream stream = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(stream);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ReadStream
		///</summary>
		[TestMethod]
		public void ReadStreamTest2()
		{
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(filePath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TryGetBytes
		///</summary>
		[TestMethod]
		public void TryGetBytesTest()
		{
			string hexValue = string.Empty; // TODO: Initialize to an appropriate value
			byte[] result = null; // TODO: Initialize to an appropriate value
			byte[] resultExpected = null; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = FileUtil.TryGetBytes(hexValue, out result);
			Assert.AreEqual(resultExpected, result);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
