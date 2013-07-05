using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Adenson.CoreTest.Sys
{
	[TestFixture]
	public class FileUtilTest
	{
		[Test]
		public void CreateFileTest()
		{
			string filePath = Path.GetTempFileName();
			byte[] buffer = new byte[] { 1, 3, 4, 5 };
			bool overwrite = false;
			string actual = FileUtil.CreateFile(filePath, buffer, overwrite);

			Assert.IsTrue(File.Exists(actual));
			Assert.AreEqual(filePath, actual);
			byte[] bytes = FileUtil.ReadStream(filePath);
			buffer.IsEquivalentTo(bytes);
		}

		[Test]
		public void FixFileNameTest()
		{
			string path = string.Empty; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = FileUtil.FixFileName(path);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[Test]
		public void GetBytesTest()
		{
			string hexValue = string.Empty; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.GetBytes(hexValue);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[Test]
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

		[Test]
		public void GetIsDirectoryTest()
		{
			string fullPath = string.Empty; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = FileUtil.GetIsDirectory(fullPath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[Test]
		public void ReadStreamTest1()
		{
			Uri url = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(url);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[Test]
		public void ReadStreamTest2()
		{
			Stream stream = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(stream);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[Test]
		public void ReadStreamTest3()
		{
			string filePath = string.Empty; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = FileUtil.ReadStream(filePath);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[Test]
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