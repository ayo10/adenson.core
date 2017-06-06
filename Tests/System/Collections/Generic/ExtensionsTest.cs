using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Adenson.CoreTest.System.Collections.Generic
{
	[TestFixture]
	public class ExtensionsTest
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
		public void GetTest()
		{
			var dic = new Dictionary<string, int> { { "one", 1 }, { "two", 2 } };
			Assert.AreEqual(1, dic.Get("one", 3));
			Assert.AreEqual(2, dic.Get("two", 4));
			Assert.AreEqual(3, dic.Get("three", 3));
			Assert.IsFalse(dic.ContainsKey("three"));
		}

		[Test]
		public void GetOrAddTest()
		{
			var dic = new Dictionary<string, int> { { "one", 1 }, { "two", 2 } };
			Assert.AreEqual(1, dic.GetOrAdd("one", 3));
			Assert.AreEqual(2, dic.GetOrAdd("two", 4));
			Assert.AreEqual(3, dic.GetOrAdd("three", 3));
			Assert.IsTrue(dic.ContainsKey("three"));
			Assert.AreEqual(3, dic["three"]);
		}

		[Test]
		public void GetOrDefaultTest()
		{
			var dic = new Dictionary<string, object> { { "one", 1 }, { "two", 2 } };
			Assert.AreEqual(1, dic.GetOrDefault("one"));
			Assert.AreEqual(2, dic.GetOrDefault("two"));
			Assert.AreEqual(null, dic.GetOrDefault("three"), "No 'three' exists, so returns null");
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

		[Test]
		public void ToDictionaryTest()
		{
			string value = "key1=value1&key2=value2";
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } }, value.ToDictionary());

			value = "key1 = value1&key2 = value2";
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } }, value.ToDictionary());

			value = "key1 = value1 & key2 = value2";
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } }, value.ToDictionary());

			value = "key1=value1;key2=value2";
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } }, value.ToDictionary(";"));

			value = "key1|value1;key2|value2";
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } }, value.ToDictionary(";", "|")); 

			value = "key1=&key2=value2";
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "key1", "" }, { "key2", "value2" } }, value.ToDictionary());
		}
	}
}