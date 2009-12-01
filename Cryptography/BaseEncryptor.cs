using System;
using System.IO;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	public abstract class BaseEncryptor
	{
		#region Variables
		private byte[] rgbKey;
		private byte[] rgbIV;
		#endregion
		#region Constructor

		public BaseEncryptor() { }
		internal BaseEncryptor(byte[] key, byte[] iv)
		{
			this.SetKeys(key, iv);
		}

		#endregion
		#region Properties

		public abstract SymmetricAlgorithm Algorithm { get; }

		#endregion
		#region Methods

		public byte[] Encrypt(byte[] toEncrypt)
		{
			ICryptoTransform transform = this.CreateEncryptor();
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
			BinaryWriter sw = new BinaryWriter(cs);
			sw.Write(toEncrypt);
			sw.Flush();

			cs.FlushFinalBlock();
			ms.Flush();

			return ms.GetBuffer();
		}
		public string Encrypt(string toEncrypt)
		{
			ICryptoTransform transform = this.CreateEncryptor();

			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
			StreamWriter sw = new StreamWriter(cs);
			sw.Write(toEncrypt);
			sw.Flush();

			cs.FlushFinalBlock();
			ms.Flush();
			return Convert.ToBase64String(ms.GetBuffer(), 0, int.Parse(ms.Length.ToString()));
		}
		public string Decrypt(string toDecrypt)
		{
			ICryptoTransform transform = this.CreateDecryptor();

			byte[] buffer = Convert.FromBase64String(toDecrypt);
			MemoryStream ms = new MemoryStream(buffer);
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);
			StreamReader sr = new StreamReader(cs);
			return sr.ReadToEnd();
		}
		public byte[] Decrypt(byte[] toDecrypt)
		{
			ICryptoTransform transform = this.CreateDecryptor();

			MemoryStream ms = new MemoryStream(toDecrypt);
			CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);

			byte[] buffer = new byte[cs.Length];
			cs.Write(buffer, 0, buffer.Length);
			return buffer;
		}
		public ICryptoTransform CreateEncryptor()
		{
			if (this.Algorithm == null) throw new ArgumentNullException("this.Algorithm", ExceptionMessages.AlgorithmNull);
			if (rgbIV == null && rgbKey == null) return this.Algorithm.CreateEncryptor();
			return this.Algorithm.CreateEncryptor(rgbKey, rgbIV);
		}
		public ICryptoTransform CreateDecryptor()
		{
			if (this.Algorithm == null) throw new ArgumentNullException("this.Algorithm", ExceptionMessages.AlgorithmNull);
			if (rgbIV == null && rgbKey == null) return this.Algorithm.CreateDecryptor();
			return this.Algorithm.CreateDecryptor(rgbKey, rgbIV);
		}
		public void SetKeys(byte[] key, byte[] iv)
		{
			rgbKey = key;
			rgbIV = iv;
		}

		#endregion
	}
}