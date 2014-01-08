using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Adenson.CoreTest.System.Collections.Generic
{
	[TestFixture]
	public class GenericCollectionsExtensionsTest
	{
		[Test]
		public void AddOrSetTest()
		{
			Dictionary<string, string> target = new Dictionary<string,string>();
			target.AddOrSet("test1", "value1");
			Assert.IsTrue(target.ContainsKey("test1"));
			Assert.AreEqual("value1", target["test1"]);

			target.Add("test2", "value2");
			target.AddOrSet("test2", "value3");
			Assert.AreEqual("value3", target["test2"]);
		}
		
		[Test]
		public void ContainsKeyTest()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int> { { "one", 1 }, { "two", 2 } };
			Assert.IsTrue(dictionary.ContainsKey("one", StringComparison.CurrentCulture));
			Assert.IsTrue(dictionary.ContainsKey("ONE", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(dictionary.ContainsKey("tHREe", StringComparison.CurrentCultureIgnoreCase));
		}

		[Test]
		public void GetValueTest()
		{
			var dic = new Dictionary<string, int> { { "one", 1 }, { "TWO", 2 } };
			Assert.AreEqual(1, dic.Get("ONE", StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(2, dic.Get("two", StringComparison.CurrentCultureIgnoreCase));
			Assert.Throws<KeyNotFoundException>(delegate { dic.Get("ONE", StringComparison.CurrentCulture); });
			Assert.Throws<KeyNotFoundException>(delegate { dic.Get("two", StringComparison.CurrentCulture); });
		}

		[Test, ExpectedException(typeof(KeyNotFoundException))]
		public void GetValueFailTest()
		{
			var dic = new Dictionary<string, int> { { "one", 1 }, { "TWO", 2 } };
			Assert.IsNull(dic.Get("ONE", StringComparison.CurrentCulture));
			Assert.IsNull(dic.Get("two", StringComparison.CurrentCulture));
		}

		[Test]
		public void IsNullOrEmptyTest()
		{
			IEnumerable<int> values = null;
			Assert.IsTrue(values.IsNullOrEmpty());

			values = new int[] { };
			Assert.IsTrue(values.IsNullOrEmpty());

			values = new int[] { 2 };
			Assert.IsFalse(values.IsNullOrEmpty());
		}
		
		[Test]
		public void MergeWithTest()
		{
			int[] value1 = new int[] { 1, 2 };
			int[] value2 = new int[] { 3, 4 };
			int[] merged = value1.MergeWith(value2);
			Assert.AreEqual(new int[] { 1, 2, 3, 4 }, merged);
		}
	}
}