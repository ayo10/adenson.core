using System;
using System.Collections.Generic;
using Adenson.Log;
using Adenson.Configuration;

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
		/// Gets the MD5 hash of specified bit array
		/// </summary>
		/// <param name="buffer">The bit array</param>
		/// <returns>String representation of the md5 hash of array</returns>
		public static string GetMD5Hash(byte[] buffer)
		{
			if (buffer == null) return null;
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			string hash = System.Convert.ToBase64String(md5.ComputeHash(buffer)).Replace("\\", String.Empty).Replace("/", String.Empty).Replace("=", String.Empty);
			return hash;
		}

		#endregion
	}
}