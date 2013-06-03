using System;
using System.Text;
using Adenson.Cryptography;
using NUnit.Framework;

namespace Adenson.CoreTest.Cryptography
{
	[TestFixture]
	public class CryptUtilTest
	{
		[Test]
		public void EncryptDecryptTest()
		{
			string expected = "Test test test test";
			byte[] value = Encoding.Default.GetBytes(expected);
			byte[] encrypted = CryptUtil.Encrypt(value);
			Assert.AreNotEqual(value, encrypted);
			string encryptedSting = Encoding.Default.GetString(encrypted);
			Assert.AreNotEqual(expected, encryptedSting);

			byte[] decrypted = CryptUtil.Decrypt(encrypted);
			Assert.AreNotEqual(decrypted, encrypted);
			Assert.AreEqual(value, decrypted);
			string decryptedSting = Encoding.Default.GetString(decrypted);
			Assert.AreEqual(expected, decryptedSting);
		}

		[Test]
		public void GenerateRandomBytesTest()
		{

		}

		[Test]
		public void GetHashTest()
		{
		}
	}
}