using System;
using Adenson.Cryptography;
using NUnit.Framework;

namespace Adenson.CoreTest.Cryptography
{
	[TestFixture]
	public class TripleDesTest : BaseCryptTest<TripleDes>
	{
		[OneTimeSetUp]
		public void Setup()
		{
			this.IVSize = 8;
			this.KeySize = 16;
		}

		protected override TripleDes GetCrypt(byte[] key, byte[] iv)
		{
			return new TripleDes(key, iv);
		}
	}
}