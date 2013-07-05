using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace Adenson.CoreTest.System.Collections
{
	[TestFixture]
	public class CollectionsExtensionsTest
	{
		[Test]
		public void MergeWithTest()
		{
			int[] value1 = new int[] { 1, 2 };
			int[] value2 = new int[] { 3, 4 };
			int[] merged = value1.MergeWith(value2).Cast<int>().ToArray();
			Assert.AreEqual(new int[] { 1, 2, 3, 4 }, merged);
		}
	}
}
