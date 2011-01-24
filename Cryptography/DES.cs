using System;
using System.Security.Cryptography;

namespace Adenson.Cryptography
{
	public sealed class DES : BaseEncryptor
	{
		public DES() : base()
		{
		}
		public DES(byte[] key, byte[] iv) : base(key, iv)
		{
		}
		public override SymmetricAlgorithm Algorithm
		{
			get { return System.Security.Cryptography.DES.Create(); }
		}
	}
}