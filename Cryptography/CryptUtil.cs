using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Encryption/Decryption util class.
	/// </summary>
	public static class CryptUtil
	{
		#region Methods

		/// <summary>
		/// Decrypts the specified byte array using AES.
		/// </summary>
		/// <param name="value">The byte array to decrypt.</param>
		/// <returns>Decrypted array.</returns>
		public static byte[] Decrypt(byte[] value)
		{
			return new Rijndael().Decrypt(value);
		}

		/// <summary>
		/// Decrypts the specified byte array using AES, using the specified secret key and iniitalization vector.
		/// </summary>
		/// <param name="value">The byte array to decrypt.</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Decrypted array.</returns>
		/// <exception cref="CryptographicException">When <paramref name="key"/> and/or <paramref name="iv"/> are not the same that were used to encrypt the value -OR- the value is not an encrypted value to begin with.</exception>
		public static byte[] Decrypt(byte[] value, byte[] key, byte[] iv)
		{
			return new Rijndael(key, iv).Decrypt(value);
		}

		/// <summary>
		/// Encrypts the specified byte array using AES (using built-in key and iv values).
		/// </summary>
		/// <param name="value">The byte array to encrypt.</param>
		/// <returns>Encrypted array.</returns>
		/// <remarks>You should be calling <see cref="Encrypt(byte[], byte[], byte[])"/> suppling your own secret key and iv.</remarks>
		public static byte[] Encrypt(byte[] value)
		{
			return new Rijndael().Encrypt(value);
		}

		/// <summary>
		/// Encrypts the specified byte array using AES, using the specified secret key and iniitalization vector.
		/// </summary>
		/// <param name="value">The byte array to decrypt.</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Encrypted array.</returns>
		public static byte[] Encrypt(byte[] value, byte[] key, byte[] iv)
		{
			return new Rijndael(key, iv).Encrypt(value);
		}

		/// <summary>
		/// Generates random bytes of data using <see cref="RNGCryptoServiceProvider"/>.
		/// </summary>
		/// <param name="size">The size of the data to return.</param>
		/// <returns>Generated bytes.</returns>
		public static byte[] GenerateRandomBytes(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentException(Exceptions.SizeLessOrEqualToZero, "size");
			}

			using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
			{
				byte[] data = new byte[size];
				rngCsp.GetBytes(data);
				return data;
			}
		}

		/// <summary>
		/// Gets the hash of specified bit array using <see cref="HashType.SHA512"/>.
		/// </summary>
		/// <param name="value">The bit array.</param>
		/// <returns>The bit array hash.</returns>
		public static byte[] GetHash(byte[] value)
		{
			return GetHash(HashType.SHA512, value);
		}

		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type.
		/// </summary>
		/// <param name="hashType">The hash algorithm to use.</param>
		/// <param name="value">The bit array to hash.</param>
		/// <returns>The bit array hash.</returns>
		/// <exception cref="ArgumentException">If <paramref name="hashType"/> is either <see cref="HashType.HMACRIPEMD160"/>, <see cref="HashType.MACTripleDES"/> or <see cref="HashType.PBKDF2"/>.</exception>
		public static byte[] GetHash(HashType hashType, byte[] value)
		{
			if (value == null)
			{
				return null;
			}

			if (hashType == HashType.None)
			{
				return value;
			}

			HashAlgorithm algorithm = null;
			switch (hashType)
			{
				case HashType.HMACRIPEMD160:
					throw new ArgumentException(StringUtil.Format(Exceptions.GetHashArgNotSupported, hashType), "hashType");
				case HashType.MACTripleDES:
					throw new ArgumentException(StringUtil.Format(Exceptions.GetHashArgNotSupported, hashType), "hashType");
				case HashType.MD5:
					algorithm = MD5.Create();
					break;
				case HashType.PBKDF2:
					throw new ArgumentException(StringUtil.Format(Exceptions.GetHashArgNotSupported, hashType), "hashType");
				case HashType.RIPEMD160:
					algorithm = RIPEMD160.Create();
					break;
				case HashType.SHA1:
					algorithm = SHA1.Create();
					break;
				case HashType.SHA256:
					algorithm = SHA256.Create();
					break;
				case HashType.SHA384:
					algorithm = SHA384.Create();
					break;
				case HashType.SHA512:
					algorithm = SHA512.Create();
					break;
				default:
					throw new NotSupportedException(hashType.ToString());
			}

			using (algorithm)
			{
				return algorithm.ComputeHash(value);
			}
		}

		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type and salt.
		/// </summary>
		/// <param name="hashType">The hash algorithm to use.</param>
		/// <param name="value">The bit array to hash.</param>
		/// <param name="salt">The salt.</param>
		/// <returns>The bit array hash.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="salt"/> is null.</exception>
		public static byte[] GetHash(HashType hashType, byte[] value, byte[] salt)
		{
			if (value == null)
			{
				return null;
			}

			if (salt == null)
			{
				throw new ArgumentNullException("salt");
			}

			if (hashType == HashType.None)
			{
				return value;
			}
			else if (hashType == HashType.PBKDF2)
			{
				using (var g = new Rfc2898DeriveBytes(value, salt, 5000))
				{
					return g.GetBytes(32);
				}
			}
			else if (hashType == HashType.HMACRIPEMD160)
			{
				using (var g = new HMACRIPEMD160(salt))
				{
					return g.ComputeHash(value);
				}
			}
			else if (hashType == HashType.MACTripleDES)
			{
				using (var g = new MACTripleDES(salt))
				{
					return g.ComputeHash(value);
				}
			}
			else
			{
				return CryptUtil.GetHash(hashType, value.MergeWith(salt));
			}
		}

		#endregion
	}
}