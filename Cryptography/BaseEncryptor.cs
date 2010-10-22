using System;
using System.IO;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Base class for built in encryptors
	/// </summary>
	public abstract class BaseEncryptor
	{
		#region Variables
		private byte[] rgbKey;
		private byte[] rgbIV;
		#endregion
		#region Constructor

		/// <summary>
		/// 
		/// </summary>
		public BaseEncryptor()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="iv"></param>
		internal BaseEncryptor(byte[] key, byte[] iv)
		{
			this.SetKeys(key, iv);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the algorithm the encryptor is based on
		/// </summary>
		public abstract SymmetricAlgorithm Algorithm { get; }

		#endregion
		#region Methods

		/// <summary>
		/// Encrypts the specified byte array
		/// </summary>
		/// <param name="toEncrypt">The byte array to encrypt</param>
		/// <returns>The encrypted value</returns>
		public byte[] Encrypt(byte[] toEncrypt)
		{
			if (toEncrypt == null) return null;

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
			if (toEncrypt == null) return null;

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
		/// The 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <returns></returns>
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
		/// 
		/// </summary>
		/// <returns></returns>
		public ICryptoTransform CreateEncryptor()
		{
			if (this.Algorithm == null) throw new ArgumentNullException("this.Algorithm", Exceptions.AlgorithmNull);
			if (rgbIV == null && rgbKey == null) return this.Algorithm.CreateEncryptor();
			return this.Algorithm.CreateEncryptor(rgbKey, rgbIV);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ICryptoTransform CreateDecryptor()
		{
			if (this.Algorithm == null) throw new ArgumentNullException("this.Algorithm", Exceptions.AlgorithmNull);
			if (rgbIV == null && rgbKey == null) return this.Algorithm.CreateDecryptor();
			return this.Algorithm.CreateDecryptor(rgbKey, rgbIV);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="iv"></param>
		public void SetKeys(byte[] key, byte[] iv)
		{
			rgbKey = key;
			rgbIV = iv;
		}

		#endregion
	}
}