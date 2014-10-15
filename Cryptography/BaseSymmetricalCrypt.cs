using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Base class for built in encryptors
	/// </summary>
	public abstract class BaseSymmetricalCrypt : IDisposable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseSymmetricalCrypt"/> class with the specified <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/> property and initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		protected BaseSymmetricalCrypt(byte[] key, byte[] iv)
		{
			this.Key = key;
			this.IV = iv;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the algorithm the encryptor is based on.
		/// </summary>
		public abstract SymmetricAlgorithm Algorithm
		{
			get;
		}
		
		/// <summary>
		/// Gets or sets the initialization vector.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "In this case, yes, this should return an array")]
		public byte[] IV
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the secret key.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "In this case, yes, this should return an array")]
		public byte[] Key
		{
			get;
			protected set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Encrypts the specified byte array.
		/// </summary>
		/// <param name="value">The byte array to encrypt.</param>
		/// <returns>The encrypted value</returns>
		public byte[] Encrypt(byte[] value)
		{
			if (value == null)
			{
				return null;
			}

			ICryptoTransform transform = this.CreateTransform(true);
			using (MemoryStream msout = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(msout, transform, CryptoStreamMode.Write))
				{
					using (BinaryWriter bw = new BinaryWriter(cs))
					{
						bw.Write(value);
					}

					return msout.ToArray();
				}
			}
		}

		/// <summary>
		/// Decrypts specified encrypted byte array.
		/// </summary>
		/// <param name="value">The byte array to decrypt.</param>
		/// <returns>Decrypted byte array.</returns>
		public byte[] Decrypt(byte[] value)
		{
			ICryptoTransform transform = this.CreateTransform(false);

			using (MemoryStream msin = new MemoryStream(value))
			{
				using (CryptoStream cs = new CryptoStream(msin, transform, CryptoStreamMode.Read))
				{
					return FileUtil.ReadStream(cs);
				}
			}
		}

		/// <summary>
		/// Disposes <see cref="Algorithm"/>.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Disposes <see cref="Algorithm"/>.
		/// </summary>
		/// <param name="disposing">Doesn't matter.</param>
		protected virtual void Dispose(bool disposing)
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

			if (this.IV == null && this.Key == null)
			{
				return encrypt ? this.Algorithm.CreateEncryptor() : this.Algorithm.CreateDecryptor();
			}
			else
			{
				this.Algorithm.IV = this.IV;
				this.Algorithm.Key = this.Key;
				return encrypt ? this.Algorithm.CreateEncryptor(this.Key, this.IV) : this.Algorithm.CreateDecryptor(this.Key, this.IV);
			}
		}

		#endregion
	}
}