using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Adenson.CoreTest.System
{
	[TestFixture]
	public class FileUtilTest
	{
		[Test]
		public void CreateFileTest()
		{
			string filePath = Path.GetTempFileName();
			byte[] buffer = new byte[] { 1, 3, 4, 5 };
			string actual = FileUtil.CreateFile(filePath, buffer);

			Assert.IsTrue(File.Exists(actual));
			Assert.AreEqual(filePath, actual);
			byte[] bytes = FileUtil.ReadStream(new Uri(filePath)).ToBytes();
			buffer.SequenceEqual(bytes);
		}

		[Test]
		public void FixFileNameTest()
		{
			Assert.AreEqual("a b c d e f g h i j k j l-m,n", FileUtil.FixFileName("a\"b<c>d|e\0f\ag\bh\ti\nj\vk\fj\rl:m/n"));
		}

		[Test]
		public void GetBytesTest()
		{
			Assert.AreEqual(new byte[] { 170, 171, 170, 187 }, FileUtil.GetBytes("aaAbaaBB"));
		}

		[Test]
		public void GetFilesTest()
		{
			string directory = TestContext.CurrentContext.WorkDirectory;
			IEnumerable<string> extensions = new string[] { "*.dll", "*.pdb" };
			string[] expected = Directory.GetFiles(directory, extensions.First(), SearchOption.AllDirectories).Union(Directory.GetFiles(directory, extensions.ElementAt(1), SearchOption.AllDirectories)).ToArray();
			string[] actual = FileUtil.GetFiles(directory, extensions);
			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Test]
		public void GetIsDirectoryTest()
		{
			string fullPath = TestContext.CurrentContext.WorkDirectory;
			Assert.IsTrue(FileUtil.GetIsDirectory(fullPath));
			Assert.IsFalse(FileUtil.GetIsDirectory(Directory.GetFiles(fullPath, "*.dll", SearchOption.AllDirectories).First()));
		}

		[Test]
		public void ReadStreamTest1()
		{
			string path = Path.GetTempFileName();
			using (var stream = File.CreateText(path))
			{
				stream.WriteLine("a");
			}

			Uri url = new Uri(path);
			Assert.AreEqual(new byte[] { 97, 13, 10 }, FileUtil.ReadStream(url));
		}

		[Test]
		public void TryGetBytesTest()
		{
			byte[] result = null;
			Assert.IsTrue(FileUtil.TryGetBytes("aaAbaaBB", out result));
			Assert.AreEqual(new byte[] { 170, 171, 170, 187 }, result);
			
			Assert.IsFalse(FileUtil.TryGetBytes("aaAbaaB", out result));
		}
	}
}