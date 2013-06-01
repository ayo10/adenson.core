using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Adenson.Cryptography
{
	/// <summary>
	/// String encryption and decription utilities
	/// </summary>
	public static class Encryptor
	{
		#region Variables
		private static byte[] randomBytes = GenerateRandomBytes();
		private static byte[] fixedBytes24 = new byte[] { 82, 204, 235, 105, 52, 160, 47, 204, 213, 109, 114, 52, 186, 53, 87, 5, 138, 42, 144, 193, 177, 172, 227, 21 };
		#endregion
		#region Methods

		/// <summary>
		/// Decrypts the specified string using Rijndael (AES)
		/// </summary>
		/// <param name="value">The string to decrypt</param>
		/// <returns>Decrypted string</returns>
		public static string Decrypt(string value)
		{
			return Encryptor.Decrypt(EncryptorType.Rijndael, value);
		}

		/// <summary>
		/// Decrypts the specified string using specified type
		/// </summary>
		/// <param name="value">The string to decrypt</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Decrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Decrypt(string value, byte[] key, byte[] iv)
		{
			return Encryptor.Decrypt(EncryptorType.Rijndael, value, key, iv);
		}

		/// <summary>
		/// Decrypts the specified string using specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="value">The string to decrypt</param>
		/// <returns>Decrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Decrypt(EncryptorType type, string value)
		{
			if (type == EncryptorType.None)
			{
				throw new ArgumentNullException("type");
			}

			switch (type)
			{
				case EncryptorType.DES: return new DES().Decrypt(value);
				case EncryptorType.Rijndael: return new Rijndael().Decrypt(value);
				case EncryptorType.TripleDES: return new TripleDES().Decrypt(value);
			}

			throw new ArgumentException(Exceptions.NoEncryptorExists, "type");
		}

		/// <summary>
		/// Decrypts the specified string using specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="value">The string to decrypt</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Decrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Decrypt(EncryptorType type, string value, byte[] key, byte[] iv)
		{
			if (type == EncryptorType.None)
			{
				throw new ArgumentNullException("type");
			}

			switch (type)
			{
				case EncryptorType.DES: return new DES(key, iv).Decrypt(value);
				case EncryptorType.Rijndael: return new Rijndael(key, iv).Decrypt(value);
				case EncryptorType.TripleDES: return new TripleDES(key, iv).Decrypt(value);
			}

			throw new ArgumentException(Exceptions.NoEncryptorExists, "type");
		}

		/// <summary>
		/// Encrypts the specified string using Rijndael (AES)
		/// </summary>
		/// <param name="value">The string to encrypt</param>
		/// <returns>Encrypted string</returns>
		public static string Encrypt(string value)
		{
			return Encryptor.Encrypt(EncryptorType.Rijndael, value);
		}

		/// <summary>
		/// Encrypts the specified string using Rijndael (AES)
		/// </summary>
		/// <param name="value">The string to encrypt</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Encrypted string</returns>
		public static string Encrypt(string value, byte[] key, byte[] iv)
		{
			return Encryptor.Encrypt(EncryptorType.Rijndael, value, key, iv);
		}

		/// <summary>
		/// Encrypts the specified string using specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="value">The string to encrypt</param>
		/// <returns>Encrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Encrypt(EncryptorType type, string value)
		{
			if (type == EncryptorType.None)
			{
				throw new ArgumentNullException("type");
			}

			switch (type)
			{
				case EncryptorType.DES: return new DES().Encrypt(value);
				case EncryptorType.Rijndael: return new Rijndael().Encrypt(value);
				case EncryptorType.TripleDES: return new TripleDES().Encrypt(value);
			}

			throw new ArgumentException(Exceptions.NoEncryptorExists, "type");
		}

		/// <summary>
		/// Encrypts the specified string using specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="value">The string to encrypt</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Encrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Encrypt(EncryptorType type, string value, byte[] key, byte[] iv)
		{
			if (type == EncryptorType.None)
			{
				throw new ArgumentNullException("type");
			}

			switch (type)
			{
				case EncryptorType.DES: return new DES(key, iv).Encrypt(value);
				case EncryptorType.Rijndael: return new Rijndael(key, iv).Encrypt(value);
				case EncryptorType.TripleDES: return new TripleDES(key, iv).Encrypt(value);
			}

			throw new ArgumentException(Exceptions.NoEncryptorExists, "type");
		}

		/// <summary>
		/// Gets the hash of specified bit array using HashAlgorithmType.SHA512.
		/// </summary>
		/// <param name="value">The bit array</param>
		/// <returns>String representation of the hash of array</returns>
		public static byte[] GetHash(byte[] value)
		{
			return GetHash(value, HashAlgorithmType.SHA512);
		}

		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type.
		/// </summary>
		/// <param name="value">The bit array to hash.</param>
		/// <param name="hashAlgorithmType">The hash algorithm to use.</param>
		/// <returns>String representation of the hash of array</returns>
		public static byte[] GetHash(byte[] value, HashAlgorithmType hashAlgorithmType)
		{
			if (value == null)
			{
				return null;
			}

			if (hashAlgorithmType == HashAlgorithmType.None)
			{
				return value;
			}

			switch (hashAlgorithmType)
			{
				case HashAlgorithmType.HMACRIPEMD160:
					using (var g = new HMACRIPEMD160(fixedBytes24))
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.MACTripleDES:
					using (var g = new MACTripleDES(fixedBytes24))
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.MD5:
					using (var g = MD5.Create())
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.PBKDF2:
					using (var g = new Rfc2898DeriveBytes(value, randomBytes, 1000))
					{
						return g.GetBytes(32);
					}

				case HashAlgorithmType.RIPEMD160:
					using (var g = RIPEMD160.Create())
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.SHA1:
					using (var g = SHA1.Create())
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.SHA256:
					using (var g = SHA256.Create())
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.SHA384:
					using (var g = SHA384.Create())
					{
						return g.ComputeHash(value);
					}

				case HashAlgorithmType.SHA512:
					using (var g = SHA512.Create())
					{
						return g.ComputeHash(value);
					}

				default:
					throw new NotSupportedException(hashAlgorithmType.ToString());
			}
		}

		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type and salt.
		/// </summary>
		/// <param name="value">The bit array to hash.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="hashAlgorithmType">The hash algorithm to use.</param>
		/// <returns>String representation of the hash of array</returns>
		public static byte[] GetHash(byte[] value, byte[] salt, HashAlgorithmType hashAlgorithmType)
		{
			if (value == null)
			{
				return null;
			}

			if (salt == null)
			{
				throw new ArgumentNullException("salt");
			}

			if (hashAlgorithmType == HashAlgorithmType.None)
			{
				return value;
			}

			switch (hashAlgorithmType)
			{
				case HashAlgorithmType.HMACRIPEMD160:
					using (var g = new HMACRIPEMD160(salt))
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.MACTripleDES:
					using (var g = new MACTripleDES(salt))
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.MD5:
					using (var g = MD5.Create())
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.PBKDF2:
					using (var g = new Rfc2898DeriveBytes(value, salt, 1000))
					{
						return g.GetBytes(32);
					}

				case HashAlgorithmType.RIPEMD160:
					using (var g = RIPEMD160.Create())
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.SHA1:
					using (var g = SHA1.Create())
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.SHA256:
					using (var g = SHA256.Create())
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.SHA384:
					using (var g = SHA384.Create())
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				case HashAlgorithmType.SHA512:
					using (var g = SHA512.Create())
					{
						return g.ComputeHash(value.MergeWith(salt));
					}

				default:
					throw new NotSupportedException(hashAlgorithmType.ToString());
			}
		}

		/// <summary>
		/// Gets the hash of specified string, using HashAlgorithmType.SHA512 and default text encoding.
		/// </summary>
		/// <param name="value">The string to hash</param>
		/// <returns>Hashed version of string</returns>
		/// <exception cref="ArgumentNullException">If encoding is null</exception>
		/// <remarks>Calls Encryptor.GetHash(toEncrypt, HashAlgorithmType, Encoding.Default).</remarks>
		public static byte[] GetHash(string value)
		{
			return Encryptor.GetHash(value, HashAlgorithmType.SHA512, Encoding.Default);
		}

		/// <summary>
		/// Gets the hash of specified string, using specified hash algorithm type, using default text encoding.
		/// </summary>
		/// <param name="value">The string to hash</param>
		/// <param name="hashAlgorithmType">The hash algorithm to use.</param>
		/// <returns>Hashed version of string</returns>
		/// <exception cref="ArgumentNullException">If encoding is null</exception>
		/// <remarks>Calls Encryptor.GetHash(toEncrypt, hashAlgorithmType, Encoding.Default"/>.</remarks>
		public static byte[] GetHash(string value, HashAlgorithmType hashAlgorithmType)
		{
			return Encryptor.GetHash(value, hashAlgorithmType, Encoding.Default);
		}

		/// <summary>
		/// Gets the hash of specified string, using specified hash algorithm type, using specified text encoding.
		/// </summary>
		/// <param name="value">The string to hash</param>
		/// <param name="hashAlgorithmType">The hash algorithm to use.</param>
		/// <param name="encoding">The encoding to use to convert specified string into a byte array.</param>
		/// <returns>Hashed version of string</returns>
		public static byte[] GetHash(string value, HashAlgorithmType hashAlgorithmType, Encoding encoding)
		{
			if (value == null)
			{
				return null;
			}

			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}

			byte[] buffer = encoding.GetBytes(value);
			return Encryptor.GetHash(buffer, hashAlgorithmType);
		}
		
		private static byte[] GenerateRandomBytes()
		{
			using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
			{
				byte[] data = new byte[32];
				rngCsp.GetBytes(data);
				return data;
			}
		}

		#endregion
	}
}