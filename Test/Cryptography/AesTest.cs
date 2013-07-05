using System;
using Adenson.Cryptography;
using NUnit.Framework;

namespace Adenson.CoreTest.Cryptography
{
	[TestFixture]
	public class AesTest : BaseCryptTest<Aes>
	{
		protected override Aes GetCrypt()
		{
			return new Aes();
		}

		protected override Aes GetCrypt(byte[] key, byte[] iv)
		{
			return new Aes(key, iv);
		}
	}
}