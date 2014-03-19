using System;
using Adenson.Cryptography;
using NUnit.Framework;

namespace Adenson.CoreTest.Cryptography
{
	[TestFixture]
	public class RijndaelTest : BaseCryptTest<Rijndael>
	{
		protected override Rijndael GetCrypt()
		{
			return new Rijndael();
		}

		protected override Rijndael GetCrypt(byte[] key, byte[] iv)
		{
			return new Rijndael(key, iv);
		}
	}
}