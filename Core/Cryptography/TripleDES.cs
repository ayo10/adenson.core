#if !NETSTANDARD1_0
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Represents the for the BaseEncryptor implementation of the Triple Data Encryption Standard algorithm.
	/// </summary>
	public sealed class TripleDes : BaseSymmetricalCrypt
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="TripleDes"/> class with the specified <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key"/> property and initialization vector (<see cref="System.Security.Cryptography.SymmetricAlgorithm.IV"/>).
		/// </summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		public TripleDes(byte[] key, byte[] iv) : base(key, iv)
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the <see cref="System.Security.Cryptography.TripleDES"/> algorithm.
		/// </summary>
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.TripleDES.Create(); }
		}

		#endregion
	}
}
#endif