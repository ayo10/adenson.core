using System;
using System.Text;
using Adenson.Cryptography;
using NUnit.Framework;

namespace Adenson.CoreTest.Cryptography
{
	[TestFixture]
	public abstract class BaseCryptTest<T> where T : BaseSymmetricalCrypt
	{
		protected int IVSize = 16;
		protected int KeySize = 16;

		[Test]
		public void EncryptDecryptWithKeysTest()
		{
			byte[] iv = CryptUtil.GenerateRandom(this.IVSize);
			byte[] key = CryptUtil.GenerateRandom(this.KeySize);
			T crypt = this.GetCrypt(key, iv);

			string expected = "Test test test test";
			byte[] value = Encoding.Default.GetBytes(expected);
			byte[] encrypted = crypt.Encrypt(value);
			Assert.AreNotEqual(value, encrypted);
			string encryptedSting = Encoding.Default.GetString(encrypted);
			Assert.AreNotEqual(expected, encryptedSting);

			byte[] decrypted = crypt.Decrypt(encrypted);
			Assert.AreNotEqual(decrypted, encrypted);
			Assert.AreEqual(value, decrypted);
			string decryptedSting = Encoding.Default.GetString(decrypted);
			Assert.AreEqual(expected, decryptedSting);
		}

		protected abstract T GetCrypt(byte[] key, byte[] iv);
	}
}
