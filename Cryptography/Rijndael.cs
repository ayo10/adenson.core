using System;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Implementation of the Rijndael (AES) encryption
	/// </summary>
	public sealed class Rijndael : BaseEncryptor
	{
		#region Constructor

		public Rijndael() : base()
		{
		}
		public Rijndael(byte[] key, byte[] iv) : base(key, iv)
		{
		}

		#endregion
		#region Properties

		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.Rijndael.Create(); }
		}

		#endregion
	}
}