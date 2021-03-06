#if !NETSTANDARD1_0
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Encryption/Decryption util class.
	/// </summary>
	public static class CryptUtil
	{
		#region Methods

		/// <summary>
		/// Decrypts the specified byte array using the specified encryption algorithm.
		/// </summary>
		/// <param name="type">The encryption algorithm to use.</param>
		/// <param name="value">The byte array to decrypt.</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Decrypted array.</returns>
		/// <exception cref="CryptographicException">When <paramref name="key"/> and/or <paramref name="iv"/> are not the same that were used to encrypt the value -OR- the value is not an encrypted value to begin with.</exception>
		public static byte[] Decrypt(EncryptionType type, byte[] value, byte[] key, byte[] iv)
		{
			BaseSymmetricalCrypt crypt;
			switch (type)
			{
#if !NETSTANDARD1_6 && !NETSTANDARD1_3
				case EncryptionType.Rijndael:
					crypt = new Rijndael(key, iv);
					break;
				case EncryptionType.TripleDES:
					crypt = new TripleDes(key, iv);
					break;
#endif
				default:
					crypt = new Aes(key, iv);
					break;
			}

			using (crypt)
			{
				return crypt.Decrypt(value);
			}
		}

		/// <summary>
		/// Encrypts the specified byte array using the specified encryption algorithm.
		/// </summary>
		/// <param name="type">The encryption algorithm to use.</param>
		/// <param name="value">The byte array to decrypt.</param>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>Encrypted array.</returns>
		public static byte[] Encrypt(EncryptionType type, byte[] value, byte[] key, byte[] iv)
		{
			BaseSymmetricalCrypt crypt;
			switch (type)
			{
#if !NETSTANDARD1_6 && !NETSTANDARD1_3
				case EncryptionType.Rijndael:
					crypt = new Rijndael(key, iv);
					break;
				case EncryptionType.TripleDES:
					crypt = new TripleDes(key, iv);
					break;
#endif
				default:
					crypt = new Aes(key, iv);
					break;
			}

			using (crypt)
			{
				return crypt.Encrypt(value);
			}
		}

		/// <summary>
		/// Generates random bytes of data using <see cref="RandomNumberGenerator"/>.
		/// </summary>
		/// <param name="size">The size of the data to return.</param>
		/// <returns>Generated bytes.</returns>
		public static byte[] GenerateRandom(int size)
		{
			Arg.IsValid(size, s => s > 0, $"The specified argument '{size}' is invalid, must be greater than zero.");
			using (RandomNumberGenerator g = RandomNumberGenerator.Create())
			{
				byte[] data = new byte[size];
				g.GetBytes(data);
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
				case HashType.MD5:
					algorithm = MD5.Create();
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
#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_4 && !NETSTANDARD1_3
				case HashType.RIPEMD160:
					algorithm = RIPEMD160.Create();
					break;
#endif
				default:
					throw new NotSupportedException($"Getting a {hashType} hash using this method is unsupported. Use a salt supported version, i.e. GetHash(HashType,byte[],byte[]) instead.");
			}

			using (algorithm)
			{
				return algorithm.ComputeHash(value);
			}
		}
		
		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type and salt.
		/// </summary>
		/// <param name="hashType">The hash algorithm to use. If <see cref="HashType.PBKDF2"/>, iterations is set to 5000.</param>
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

			Arg.IsNotNull(salt, nameof(salt));
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
#if !NETSTANDARD2_0 && !NETSTANDARD1_6 && !NETSTANDARD1_3
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
#endif
			else
			{
				return CryptUtil.GetHash(hashType, value.MergeWith(salt));
			}
		}

		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type and salt.
		/// </summary>
		/// <param name="hashType">The hash algorithm to use.</param>
		/// <param name="value">The bit array to hash.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="iterations">The number of times the hash is iterated.</param>
		/// <returns>The bit array hash.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="salt"/> is null.</exception>
		public static byte[] GetHash(HashType hashType, byte[] value, byte[] salt, int iterations)
		{
			if (value == null)
			{
				return null;
			}

			Arg.IsNotNull(salt, "salt");

			if (iterations <= 0)
			{
				throw new ArgumentException($"The specified argument '{iterations}' is invalid, must be greater than zero.", nameof(iterations));
			}

			if (hashType == HashType.None)
			{
				return value;
			}
			else if (hashType == HashType.PBKDF2)
			{
				using (var g = new Rfc2898DeriveBytes(value, salt, iterations))
				{
					return g.GetBytes(32);
				}
			}
			else
			{
				byte[] result = value;
				for (int i = 0; i < iterations; i++)
				{
					result = CryptUtil.GetHash(hashType, result);
				}

				return result;
			}
		}

#endregion
	}
}
#endif