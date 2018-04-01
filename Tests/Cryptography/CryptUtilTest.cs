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
		#region Variables
		private byte[] key = new byte[] { 143, 48, 7, 241, 35, 6, 35, 236, 123, 93, 240, 244, 62, 229, 41, 246, 49, 154, 85, 106, 14, 65, 208, 202, 228, 38, 253, 171, 52, 219, 22, 175 };
		private byte[] iv = new byte[] { 181, 230, 54, 105, 12, 203, 61, 109, 211, 133, 34, 177, 76, 29, 245, 43 };
		#endregion
		#region Tests

		[Test]
		public void EncryptAESTest()
		{
			string expected = "Test test test test";
			byte[] value = Encoding.Default.GetBytes(expected);
			byte[] encrypted = CryptUtil.Encrypt(EncryptionType.AES, value, key, iv);
			byte[] expectedBytes = new byte[] { 206, 167, 175, 90, 108, 117, 232, 113, 162, 237, 185, 245, 125, 51, 5, 72, 10, 201, 121, 183, 96, 136, 202, 124, 175, 225, 88, 162, 61, 99, 126, 116 };
			string encryptedSting = Encoding.Default.GetString(encrypted);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreEqual(expectedBytes, encrypted);
			Assert.AreNotEqual(expected, encryptedSting);

			byte[] decrypted = CryptUtil.Decrypt(EncryptionType.AES, encrypted, key, iv);
			Assert.AreNotEqual(decrypted, encrypted);
			Assert.AreEqual(value, decrypted);
			string decryptedSting = Encoding.Default.GetString(decrypted);
			Assert.AreEqual(expected, decryptedSting);

			byte[] iv2 = CryptUtil.GenerateRandom(16);
			byte[] key2 = CryptUtil.GenerateRandom(32);
			byte[] encrypted2 = CryptUtil.Encrypt(EncryptionType.AES, value, key2, iv2);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreNotEqual(encrypted2, encrypted, "Different key and iv, different encryption.");

			byte[] decrypted2 = CryptUtil.Decrypt(EncryptionType.AES, encrypted2, key2, iv2);
			Assert.AreNotEqual(decrypted2, encrypted2);
			Assert.AreEqual(value, decrypted2);
			decryptedSting = Encoding.Default.GetString(decrypted2);
			Assert.AreEqual(expected, decryptedSting);
		}

		[Test]
		public void EncryptRijndaelTest()
		{
			string expected = "Test test test test";
			byte[] value = Encoding.Default.GetBytes(expected);
			byte[] encrypted = CryptUtil.Encrypt(EncryptionType.Rijndael, value, key, iv);
			byte[] expectedBytes = new byte[] { 206, 167, 175, 90, 108, 117, 232, 113, 162, 237, 185, 245, 125, 51, 5, 72, 10, 201, 121, 183, 96, 136, 202, 124, 175, 225, 88, 162, 61, 99, 126, 116 };
			string encryptedSting = Encoding.Default.GetString(encrypted);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreEqual(expectedBytes, encrypted);
			Assert.AreNotEqual(expected, encryptedSting);

			byte[] decrypted = CryptUtil.Decrypt(EncryptionType.Rijndael, encrypted, key, iv);
			Assert.AreNotEqual(decrypted, encrypted);
			Assert.AreEqual(value, decrypted);
			string decryptedSting = Encoding.Default.GetString(decrypted);
			Assert.AreEqual(expected, decryptedSting);

			byte[] iv2 = CryptUtil.GenerateRandom(16);
			byte[] key2 = CryptUtil.GenerateRandom(32);
			byte[] encrypted2 = CryptUtil.Encrypt(EncryptionType.Rijndael, value, key2, iv2);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreNotEqual(encrypted2, encrypted, "Different key and iv, different encryption.");

			byte[] decrypted2 = CryptUtil.Decrypt(EncryptionType.Rijndael, encrypted2, key2, iv2);
			Assert.AreNotEqual(decrypted2, encrypted2);
			Assert.AreEqual(value, decrypted2);
			decryptedSting = Encoding.Default.GetString(decrypted2);
			Assert.AreEqual(expected, decryptedSting);
		}

		[Test]
		public void EncryptTripleDESTest()
		{
			string expected = "Test test test test";
			byte[] value = Encoding.Default.GetBytes(expected);
			byte[] encrypted = CryptUtil.Encrypt(EncryptionType.TripleDES, value, key.Take(16).ToArray(), iv.Take(8).ToArray());
			byte[] expectedBytes = new byte[] { 234, 120, 182, 195, 236, 78, 66, 60, 186, 199, 154, 206, 153, 170, 86, 108, 137, 5, 148, 152, 98, 231, 150, 16 };
			string encryptedSting = Encoding.Default.GetString(encrypted);
			
			Assert.AreNotEqual(value, encrypted);
			Assert.AreEqual(expectedBytes, encrypted);
			Assert.AreNotEqual(expected, encryptedSting);

			byte[] decrypted = CryptUtil.Decrypt(EncryptionType.TripleDES, encrypted, key.Take(16).ToArray(), iv.Take(8).ToArray());
			Assert.AreNotEqual(decrypted, encrypted);
			Assert.AreEqual(value, decrypted);
			string decryptedSting = Encoding.Default.GetString(decrypted);
			Assert.AreEqual(expected, decryptedSting);

			byte[] iv2 = CryptUtil.GenerateRandom(8);
			byte[] key2 = CryptUtil.GenerateRandom(16);
			byte[] encrypted2 = CryptUtil.Encrypt(EncryptionType.TripleDES, value, key2, iv2);
			Assert.AreNotEqual(value, encrypted);
			Assert.AreNotEqual(encrypted2, encrypted, "Different key and iv, different encryption.");

			byte[] decrypted2 = CryptUtil.Decrypt(EncryptionType.TripleDES, encrypted2, key2, iv2);
			Assert.AreNotEqual(decrypted2, encrypted2);
			Assert.AreEqual(value, decrypted2);
			decryptedSting = Encoding.Default.GetString(decrypted2);
			Assert.AreEqual(expected, decryptedSting);
		}

		[Test]
		public void GenerateRandomBytesTest()
		{
			// Uses RNGCryptoServiceProvider which generates random bytes, every single time. Since we have no control over that, all we can do is to tst to make sure we are getting values.
			byte[] bytes = CryptUtil.GenerateRandom(16);
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
			byte[] salt = CryptUtil.GenerateRandom(24);
			byte[] hash = CryptUtil.GetHash(HashType.None, value, salt);
			Assert.AreEqual(value, hash, "HashType.None should return the value, regardless.");

			hash = CryptUtil.GetHash(HashType.HMACRIPEMD160, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.HMACRIPEMD160, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.HMACRIPEMD160, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.MACTripleDES, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.MACTripleDES, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.MACTripleDES, value, CryptUtil.GenerateRandom(16)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.MD5, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.MD5, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.MD5, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.MD5, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.PBKDF2, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.PBKDF2, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.PBKDF2, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.RIPEMD160, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.RIPEMD160, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA1, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA1, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA1, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA1, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA256, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA256, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA256, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA256, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA384, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA384, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA384, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA384, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			hash = CryptUtil.GetHash(HashType.SHA512, value, salt);
			Assert.IsNotNull(hash);
			Assert.AreNotEqual(value, hash);
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA512, value), "Salted and unsalted versions should always return different values.");
			Assert.AreEqual(hash, CryptUtil.GetHash(HashType.SHA512, value, salt), "Hash should always return the same value");
			Assert.AreNotEqual(hash, CryptUtil.GetHash(HashType.SHA512, value, CryptUtil.GenerateRandom(8)), "Different salt, different result.");

			Assert.Throws<ArgumentNullException>(delegate { CryptUtil.GetHash(HashType.HMACRIPEMD160, value, null); }, "Salt is required.");
		}

		#endregion
	}
}