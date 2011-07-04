using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adenson.CoreTest
{
	[TestClass]
	public class FileUtilTest
	{
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

		[TestMethod]
		public void ReadStreamTest1()
		{
			Uri url = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(url);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod]
		public void ReadStreamTest2()
		{
			Stream stream = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(stream);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod]
		public void ReadStreamTest3()
		{
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(filePath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

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