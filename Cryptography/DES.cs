using System;
using System.Linq;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Represents the for the BaseEncryptor implementation of the Data Encryption Standard (DES) algorithm.
	/// </summary>
	public sealed class Des : BaseCrypt
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Des"/> class using fixed key and iv (not a good idea, you should provide your own).
		/// </summary>
		public Des() : base()
		{
			this.IV = this.IV.ToList().Take(8).ToArray();
			this.Key = this.Key.ToList().Take(8).ToArray();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Des"/> class with the specified <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/> property and initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		public Des(byte[] key, byte[] iv) : base(key, iv)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the <see cref="System.Security.Cryptography.DES"/> algorithm
		/// </summary>
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.DES.Create(); }
		}

		#endregion
	}
}