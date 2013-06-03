using System;
using System.Collections.Generic;
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
		public void EqualsToTest1()
		{
			IEnumerable<string> array1 = new string[] { "one", "TWO", "three" };
			IEnumerable<string> array2 = new string[] { "one", "TWO", "three" };
			IEnumerable<string> array3 = new string[] { "ONE", "TWO", "THREE" };

			Assert.IsTrue(array1.IsEquivalentTo(array2, StringComparison.CurrentCulture));
			Assert.IsFalse(array1.IsEquivalentTo(array3, StringComparison.CurrentCulture));
			Assert.IsTrue(array1.IsEquivalentTo(array3, StringComparison.CurrentCultureIgnoreCase));
		}

		[Test]
		public void EqualsToTest2()
		{
			EqualsTest2Helper<byte>(1, 2, 3);
			EqualsTest2Helper<string>("one", "TWO", "three");
			EqualsTest2Helper<int>(1, 2, 3);
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

		private void EqualsTest2Helper<T>(params T[] items)
		{
			IEnumerable<T> array1 = new List<T>(items);
			IEnumerable<T> array2 = new List<T>(items);

			Assert.IsTrue(array1.IsEquivalentTo(array2));
			Assert.IsTrue(array2.IsEquivalentTo(array1));

			List<T> array3 = new List<T>(items);
			array3.Add(default(T));

			Assert.IsFalse(array1.IsEquivalentTo(array3));
			Assert.IsFalse(array3.IsEquivalentTo(array1));
		}
	}
}