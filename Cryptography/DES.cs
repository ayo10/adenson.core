using System;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	internal sealed class DES : BaseEncryptor
	{
		public DES(byte[] key, byte[] iv) : base(key, iv)
		{
		}
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.DES.Create(); }
		}
	}
}