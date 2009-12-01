using System;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	internal sealed class TripleDES : BaseEncryptor
	{
		public TripleDES(byte[] key, byte[] iv) : base(key, iv)
		{
		}
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.TripleDES.Create(); }
		}
	}
}