using System;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Represents the for the BaseEncryptor implementation of the Rijndael (AES) algorithm.
	/// </summary>
	public sealed class Rijndael : BaseSymmetricalCrypt
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the Rijndael class with the specified <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/> property and initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		public Rijndael(byte[] key, byte[] iv) : base(key, iv)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the <see cref="System.Security.Cryptography.Rijndael"/> algorithm.
		/// </summary>
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.Rijndael.Create(); }
		}

		#endregion
	}
}