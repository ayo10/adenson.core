using System;
using Adenson.Cryptography;
using NUnit.Framework;

namespace Adenson.CoreTest.Cryptography
{
	[TestFixture]
	public class DesTest : BaseCryptTest<Des>
	{
		protected override Des GetCrypt()
		{
			return new Des();
		}

		protected override Des GetCrypt(byte[] key, byte[] iv)
		{
			return new Des(key, iv);
		}
	}
}