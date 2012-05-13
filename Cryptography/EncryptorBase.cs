using System;
using System.IO;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Base class for built in encryptors
	/// </summary>
	public abstract class EncryptorBase
	{
		#region Variables
		private byte[] rgbKey = new byte[] { 143, 48, 7, 241, 35, 6, 35, 236, 123, 93, 240, 244, 62, 229, 41, 246, 49, 154, 85, 106, 14, 65, 208, 202, 228, 38, 253, 171, 52, 219, 22, 175 };
		private byte[] rgbIV = new byte[] { 181, 230, 54, 105, 12, 203, 61, 109, 211, 133, 34, 177, 76, 29, 245, 43 };
		#endregion
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EncryptorBase class.
		/// </summary>
		protected EncryptorBase()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EncryptorBase class with the specified <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/> property and initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		protected EncryptorBase(byte[] key, byte[] iv) : this()
		{
			this.SetKeys(key, iv);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the algorithm the encryptor is based on
		/// </summary>
		public abstract SymmetricAlgorithm Algorithm
		{
			get;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Encrypts the specified byte array
		/// </summary>
		/// <param name="toEncrypt">The byte array to encrypt</param>
		/// <returns>The encrypted value</returns>
		public byte[] Encrypt(byte[] toEncrypt)
		{
			if (toEncrypt == null)
			{
				return null;
			}

			ICryptoTransform transform = this.CreateEncryptor();
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
			BinaryWriter sw = new BinaryWriter(cs);
			sw.Write(toEncrypt);
			sw.Flush();
			cs.Dispose();

			var result = ms.GetBuffer();
			ms.Dispose();
			ms.Flush();
			return result;
		}

		/// <summary>
		/// Encrypts the specified string
		/// </summary>
		/// <param name="toEncrypt">The string to encrypt</param>
		/// <returns>The resulting encrypted value, in base64</returns>
		public string Encrypt(string toEncrypt)
		{
			if (toEncrypt == null)
			{
				return null;
			}

			ICryptoTransform transform = this.CreateEncryptor();

			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
			StreamWriter sw = new StreamWriter(cs);
			sw.Write(toEncrypt);
			sw.Flush();
			cs.Dispose();

			var result = ms.GetBuffer();
			ms.Dispose();
			ms.Flush();
			return Convert.ToBase64String(result, 0, result.Length);
		}

		/// <summary>
		/// Decrypts specified encrypted string
		/// </summary>
		/// <param name="toDecrypt">The string to decrypt</param>
		/// <returns>Decrypted file</returns>
		public string Decrypt(string toDecrypt)
		{
			ICryptoTransform transform = this.CreateDecryptor();

			byte[] buffer = Convert.FromBase64String(toDecrypt);
			MemoryStream ms = new MemoryStream(buffer);
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);
			StreamReader sr = new StreamReader(cs);
			var result = sr.ReadToEnd();
			sr.Dispose();
			return result;
		}

		/// <summary>
		/// Decrypts specified encrypted byte array
		/// </summary>
		/// <param name="toDecrypt">The byte array to decrypt</param>
		/// <returns>Decrypted byte array</returns>
		public byte[] Decrypt(byte[] toDecrypt)
		{
			ICryptoTransform transform = this.CreateDecryptor();

			MemoryStream ms = new MemoryStream(toDecrypt);
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);

			byte[] buffer = new byte[cs.Length];
			cs.Write(buffer, 0, buffer.Length);
			cs.Dispose();
			return buffer;
		}

		/// <summary>
		/// Creates a symmetric enryptor object with the configured algorithm key and initialization vector.
		/// </summary>
		/// <returns>A symmetric encryptor object.</returns>
		/// <exception cref="ArgumentException">If the <see cref="Algorithm"/> property is null</exception>
		public ICryptoTransform CreateEncryptor()
		{
			if (this.Algorithm == null)
			{
				throw new ArgumentException(Exceptions.AlgorithmNull);
			}

			if (rgbIV == null && rgbKey == null)
			{
				return this.Algorithm.CreateEncryptor();
			}

			return this.Algorithm.CreateEncryptor(rgbKey, rgbIV);
		}

		/// <summary>
		/// Creates a symmetric decryptor object with the configured algorithm key and initialization vector.
		/// </summary>
		/// <returns>A symmetric decryptor object.</returns>
		/// <exception cref="ArgumentException">If the <see cref="Algorithm"/> property is null</exception>
		public ICryptoTransform CreateDecryptor()
		{
			if (this.Algorithm == null)
			{
				throw new ArgumentException(Exceptions.AlgorithmNull);
			}

			if (rgbIV == null && rgbKey == null)
			{
				return this.Algorithm.CreateDecryptor();
			}

			return this.Algorithm.CreateDecryptor(rgbKey, rgbIV);
		}

		/// <summary>
		/// Sets the algorithm key and initialization vector to use in the encryptor
		/// </summary>
		/// <param name="key">The algorithm key</param>
		/// <param name="iv">The initialization vector</param>
		public void SetKeys(byte[] key, byte[] iv)
		{
			rgbKey = key;
			rgbIV = iv;
		}

		#endregion
	}
}