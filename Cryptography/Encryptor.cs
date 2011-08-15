using System;
using System.Collections.Generic;
using Adenson.Log;
using Adenson.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Adenson.Cryptography
{
	/// <summary>
	/// String encryption and decription utilities
	/// </summary>
	public static class Encryptor
	{
		#region Methods

		/// <summary>
		/// Decrypts the specified string using Rijndael (AES)
		/// </summary>
		/// <param name="toDecrypt">The string to decrypt</param>
		/// <returns>Decrypted string</returns>
		public static string Decrypt(string toDecrypt)
		{
			return Encryptor.Decrypt(EncryptorType.Rijndael, toDecrypt);
		}
		/// <summary>
		/// Decrypts the specified string using specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="toDecrypt">The string to decrypt</param>
		/// <returns>Decrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Decrypt(EncryptorType type, string toDecrypt)
		{
			if (type == EncryptorType.None) throw new ArgumentNullException("type");

			switch (type)
			{
				case EncryptorType.DES: return new DES().Decrypt(toDecrypt);
				case EncryptorType.Rijndael: return new Rijndael().Decrypt(toDecrypt);
				case EncryptorType.TripleDES: return new TripleDES().Decrypt(toDecrypt);
			}
			throw new ArgumentException(Exceptions.NoEncryptorExists, "type");
		}
		/// <summary>
		/// Encrypts the specified string using Rijndael (AES)
		/// </summary>
		/// <param name="toEncrypt">The string to encrypt</param>
		/// <returns>Encrypted string</returns>
		public static string Encrypt(string toEncrypt)
		{
			return Encryptor.Encrypt(EncryptorType.Rijndael, toEncrypt);
		}
		/// <summary>
		/// Encrypts the specified string using specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="toEncrypt"></param>
		/// <returns>Encrypted string</returns>
		/// <exception cref="ArgumentException">If no encryption of specified type exists</exception>
		public static string Encrypt(EncryptorType type, string toEncrypt)
		{
			if (type == EncryptorType.None) throw new ArgumentNullException("type");

			switch (type)
			{
				case EncryptorType.DES: return new DES().Encrypt(toEncrypt);
				case EncryptorType.Rijndael: return new Rijndael().Encrypt(toEncrypt);
				case EncryptorType.TripleDES: return new TripleDES().Encrypt(toEncrypt);
			}
			throw new ArgumentException(Exceptions.NoEncryptorExists, "type");
		}
		/// <summary>
		/// Gets the hash of specified bit array using specified hash algorithm type
		/// </summary>
		/// <param name="buffer">The bit array</param>
		/// <param name="hashAlgorithmType">The hash algorithm to use.</param>
		/// <returns>String representation of the md5 hash of array</returns>
		public static byte[] GetHash(byte[] buffer, HashAlgorithmType hashAlgorithmType)
		{
			if (buffer == null) return null;
			if (hashAlgorithmType == HashAlgorithmType.None) return buffer;

			HashAlgorithm algorithm = null;
			switch (hashAlgorithmType)
			{
				case HashAlgorithmType.HMAC:
					algorithm = HMAC.Create();
					break;
				case HashAlgorithmType.MACTripleDES:
					algorithm = MACTripleDES.Create();
					break;
				case HashAlgorithmType.MD5:
					algorithm = MD5.Create();
					break;
				case HashAlgorithmType.RIPEMD160:
					algorithm = RIPEMD160.Create();
					break;
				case HashAlgorithmType.SHA1:
					algorithm = SHA1.Create();
					break;
				case HashAlgorithmType.SHA256:
					algorithm = SHA256.Create();
					break;
				case HashAlgorithmType.SHA384:
					algorithm = SHA384.Create();
					break;
				case HashAlgorithmType.SHA512:
					algorithm = SHA512.Create();
					break;
			}

			byte[] result = algorithm.ComputeHash(buffer);
			return result;
		}
		/// <summary>
		/// Gets the hash of specified string, using specified hash algorithm type, using default text encoding.
		/// </summary>
		/// <remarks>Calls <see cref="Encryptor.GetHash(String, HashAlgorithmType, Encoding)"/>.</remarks>
		/// <param name="toEncrypt">The string to hash</param>
		/// <param name="hashAlgorithmType">The hash algorithm to use.</param>
		/// <returns>Hashed version of string</returns>
		/// <exception cref="ArgumentNullException">if encoding is null</exception>
		public static byte[] GetHash(string toEncrypt, HashAlgorithmType hashAlgorithmType)
		{
			return Encryptor.GetHash(toEncrypt, hashAlgorithmType, Encoding.Default);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toEncrypt"></param>
		/// <param name="hashAlgorithmType"></param>
		/// <param name="encoding">The encoding to use to convert specified string into a byte array.</param>
		/// <returns></returns>
		public static byte[] GetHash(string toEncrypt, HashAlgorithmType hashAlgorithmType, Encoding encoding)
		{
			if (toEncrypt == null) return null;
			if (encoding == null) throw new ArgumentNullException("encoding");

			byte[] buffer = encoding.GetBytes(toEncrypt);
			return Encryptor.GetHash(buffer, hashAlgorithmType);
		}

		#endregion
	}
}