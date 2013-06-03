using System;
using System.IO;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Base class for built in encryptors
	/// </summary>
	public abstract class BaseCrypt : IDisposable
	{
		#region Variables
		private byte[] _key = new byte[] { 143, 48, 7, 241, 35, 6, 35, 236, 123, 93, 240, 244, 62, 229, 41, 246, 49, 154, 85, 106, 14, 65, 208, 202, 228, 38, 253, 171, 52, 219, 22, 175 };
		private byte[] _iv = new byte[] { 181, 230, 54, 105, 12, 203, 61, 109, 211, 133, 34, 177, 76, 29, 245, 43 };
		#endregion
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EncryptorBase class.
		/// </summary>
		protected BaseCrypt()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EncryptorBase class with the specified <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/> property and initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		protected BaseCrypt(byte[] key, byte[] iv) : this()
		{
			_key = key;
			_iv = iv;
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
		
		/// <summary>
		/// Gets the initialization vector.
		/// </summary>
		public byte[] IV
		{
			get { return _iv; }
			protected set { _iv = value; }
		}

		/// <summary>
		/// Gets the secret key.
		/// </summary>
		public byte[] Key
		{
			get { return _key; }
			protected set { _key = value; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Encrypts the specified byte array
		/// </summary>
		/// <param name="toEncrypt">The byte array to encrypt</param>
		/// <returns>The encrypted value</returns>
		public byte[] Encrypt(byte[] value)
		{
			if (value == null)
			{
				return null;
			}

			ICryptoTransform transform = this.CreateTransform(true);
			using (MemoryStream msEncrypt = new MemoryStream())
			{
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, transform, CryptoStreamMode.Write))
				{
					using (BinaryWriter swEncrypt = new System.IO.BinaryWriter(csEncrypt))
					{
						swEncrypt.Write(value);
					}

					return msEncrypt.ToArray();
				}
			}
		}

		/// <summary>
		/// Decrypts specified encrypted byte array
		/// </summary>
		/// <param name="toDecrypt">The byte array to decrypt</param>
		/// <returns>Decrypted byte array</returns>
		public byte[] Decrypt(byte[] value)
		{
			ICryptoTransform transform = this.CreateTransform(false);

			using (MemoryStream msDecrypt = new MemoryStream(value))
			{
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, transform, CryptoStreamMode.Read))
				{
					using (MemoryStream output = new MemoryStream())
					{
						var buffer = new byte[1024];
						var read = csDecrypt.Read(buffer, 0, buffer.Length);
						while (read > 0)
						{
							output.Write(buffer, 0, read);
							read = csDecrypt.Read(buffer, 0, buffer.Length);
						}

						csDecrypt.Flush();
						return output.ToArray();
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if (this.Algorithm != null)
			{
				this.Algorithm.Dispose();
			}
		}

		private ICryptoTransform CreateTransform(bool encrypt)
		{
			if (this.Algorithm == null)
			{
				throw new ArgumentException(Exceptions.AlgorithmNull);
			}

			if (_iv == null && _key == null)
			{
				Console.WriteLine("{0}: Key={1}, IV={2}", this.GetType().Name, this.Algorithm.Key.Length, this.Algorithm.IV.Length);
				return encrypt ? this.Algorithm.CreateEncryptor() : this.Algorithm.CreateDecryptor();
			}
			else
			{
				this.Algorithm.IV = _iv;
				this.Algorithm.Key = _key;
				return encrypt ? this.Algorithm.CreateEncryptor(_key, _iv) : this.Algorithm.CreateDecryptor(_key, _iv);
			}
		}

		#endregion
	}
}