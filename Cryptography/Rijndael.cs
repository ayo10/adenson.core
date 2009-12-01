using System;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	internal sealed class Rijndael : BaseEncryptor
	{
		public Rijndael(byte[] key, byte[] iv) : base(key, iv)
		{
		}
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.Rijndael.Create(); }
		}
	}
}