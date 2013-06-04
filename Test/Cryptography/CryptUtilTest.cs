using System;
using System.Linq;
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
			byte[] expectedBytes = new byte[] { 206, 167, 175, 90, 108, 117, 232, 113, 162, 237, 185, 245, 125, 51, 5, 72, 10, 201, 121, 183, 96, 136, 202, 124, 175, 225, 88, 162, 61, 99, 126, 116 };
			string encryptedSting = Encoding.Default.GetString(encrypted);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreEqual(expectedBytes, encrypted);
			Assert.AreNotEqual(expected, encryptedSting);

			byte[] decrypted = CryptUtil.Decrypt(encrypted);
			Assert.AreNotEqual(decrypted, encrypted);
			Assert.AreEqual(value, decrypted);
			string decryptedSting = Encoding.Default.GetString(decrypted);
			Assert.AreEqual(expected, decryptedSting);

			byte[] iv = CryptUtil.GenerateRandomBytes(16);
			byte[] key = CryptUtil.GenerateRandomBytes(32);
			byte[] encrypted2 = CryptUtil.Encrypt(value, key, iv);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreNotEqual(encrypted2, encrypted, "Different key and iv, different encryption.");

			byte[] decrypted2 = CryptUtil.Decrypt(encrypted2, key, iv);
			Assert.AreNotEqual(decrypted2, encrypted2);
			Assert.AreEqual(value, decrypted2);
			decryptedSting = Encoding.Default.GetString(decrypted2);
			Assert.AreEqual(expected, decryptedSting);
		}

		[Test]
		public void GenerateRandomBytesTest()
		{
			// Uses RNGCryptoServiceProvider which generates random bytes, every single time. Since we have no control over that, all we can do is to tst to make sure we are getting values.
			byte[] bytes = CryptUtil.GenerateRandomBytes(16);
			Assert.IsNotNull(bytes);
			Assert.AreEqual(16, bytes.Length);
		}

		[Test]
		public void GetHashTest()
		{
			byte[] value = Encoding.Default.GetBytes("Test test test test");
			byte[] hash = CryptUtil.GetHash(value);
			Assert.IsNotNull(hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA512, value), "Returns HashType.SHA512 by default");
			Assert.AreNotEqual(value, hash, "HashType.None should return the value, regardless.");

			hash = CryptUtil.GetHash(HashType.None, value);
			Assert.AreEqual(value, hash);

			hash = CryptUtil.GetHash(HashType.MD5, value);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.MD5, value), "Hash should always return the same value");

			hash = CryptUtil.GetHash(HashType.RIPEMD160, value);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value), "Hash should always return the same value");

			hash = CryptUtil.GetHash(HashType.SHA1, value);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA1, value), "Hash should always return the same value");

			hash = CryptUtil.GetHash(HashType.SHA256, value);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA256, value), "Hash should always return the same value");

			hash = CryptUtil.GetHash(HashType.SHA384, value);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA384, value), "Hash should always return the same value");

			hash = CryptUtil.GetHash(HashType.SHA512, value);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA512, value), "Hash should always return the same value");

			Assert.Throws<ArgumentException>(delegate { CryptUtil.GetHash(HashType.HMACRIPEMD160, value); }, "HMACRIPEMD160 should require salt.");
			Assert.Throws<ArgumentException>(delegate { CryptUtil.GetHash(HashType.MACTripleDES, value); }, "MACTripleDES should require salt.");
			Assert.Throws<ArgumentException>(delegate { CryptUtil.GetHash(HashType.PBKDF2, value); }, "PBKDF2 should require salt.");
		}

		[Test]
		public void GetHashWithSaltTest()
		{
			byte[] value = Encoding.Default.GetBytes("Test test test test");
			byte[] salt = CryptUtil.GenerateRandomBytes(24);
			byte[] hash = CryptUtil.GetHash(HashType.None, value, salt);
			Assert.AreEqual(value, hash, "HashType.None should return the value, regardless.");

			hash = CryptUtil.GetHash(HashType.HMACRIPEMD160, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.HMACRIPEMD160, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.HMACRIPEMD160, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.MACTripleDES, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.MACTripleDES, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.MACTripleDES, value, CryptUtil.GenerateRandomBytes(16)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.MD5, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.MD5, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.MD5, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.MD5, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.PBKDF2, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.PBKDF2, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.PBKDF2, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.RIPEMD160, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA1, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA1, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA1, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA1, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA256, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA256, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA256, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA256, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA384, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA384, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA384, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA384, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA512, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA512, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA512, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA512, value, CryptUtil.GenerateRandomBytes(8)), "Different salt, different result.");

			Assert.Throws<ArgumentNullException>(delegate { CryptUtil.GetHash(HashType.HMACRIPEMD160, value, null); }, "Salt is required.");
		}
	}
}