using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace Adenson.CoreTest
{
	[TestFixture]
	public class ExtensionsTest
	{
		[Test]
		public void ContainsKeyTest()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int> { { "one", 1 }, { "two", 2 } };
			Assert.IsTrue(dictionary.ContainsKey("one", StringComparison.CurrentCulture));
			Assert.IsTrue(dictionary.ContainsKey("ONE", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(dictionary.ContainsKey("tHREe", StringComparison.CurrentCultureIgnoreCase));
		}

		[Test]
		public void ContainsTest()
		{
			string source = "TestOneTwoThree";
			Assert.IsTrue(source.Contains("One", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsTrue(source.Contains("one", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(source.Contains("Four", StringComparison.CurrentCultureIgnoreCase));
		}

		[Test]
		public void ElementTest()
		{
			XContainer source = XDocument.Parse("<eLem>1</eLem>");
			Assert.AreEqual("eLem", source.Element("ELEM", StringComparison.CurrentCultureIgnoreCase).Name);
		}

		[Test]
		public void EqualsToTest1()
		{
			IEnumerable<string> array1 = new string[] { "one", "TWO", "three" };
			IEnumerable<string> array2 = new string[] { "one", "TWO", "three" };
			IEnumerable<string> array3 = new string[] { "ONE", "TWO", "THREE" };

			Assert.IsTrue(array1.SameAs(array2, StringComparison.CurrentCulture));
			Assert.IsFalse(array1.SameAs(array3, StringComparison.CurrentCulture));
			Assert.IsTrue(array1.SameAs(array3, StringComparison.CurrentCultureIgnoreCase));
		}

		[Test]
		public void EqualsToTest2()
		{
			EqualsTest2Helper<byte>(1, 2, 3);
			EqualsTest2Helper<string>("one", "TWO", "three");
			EqualsTest2Helper<int>(1, 2, 3);
		}

		[Test]
		public void GetValueTest1()
		{
			var dic = new Dictionary<string, int> { { "one", 1 }, { "TWO", 2 } };
			Assert.AreEqual(1, dic.Get("ONE", StringComparison.CurrentCultureIgnoreCase));
			Assert.AreEqual(2, dic.Get("two", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsNull(dic.Get("ONE", StringComparison.CurrentCulture));
			Assert.IsNull(dic.Get("two", StringComparison.CurrentCulture));
		}

		[Test, ExpectedException(typeof(KeyNotFoundException))]
		public void GetValueFailTest()
		{
			var dic = new Dictionary<string, int> { { "one", 1 }, { "TWO", 2 } };
			Assert.IsNull(dic.Get("ONE", StringComparison.CurrentCulture));
			Assert.IsNull(dic.Get("two", StringComparison.CurrentCulture));
		}

		[Test]
		public void GetValueTest2()
		{
			XElement source = XDocument.Parse("<root><elem>1</elem></root>").Root;
			Assert.AreEqual(1, source.GetValue<int>("elem"));

			source = XDocument.Parse("<elem>1</elem>").Root;
			Assert.AreEqual(1, source.GetValue<int>("elem"));
			
			source = XDocument.Parse("<elem attribute=\"1\" />").Root;
			Assert.AreEqual(1, source.GetValue<int>("attribute"));
		}

		[Test]
		public void HasElementTest1()
		{
			XContainer source = XDocument.Parse("<elem>1</elem>");
			Assert.IsTrue(source.HasElement("elem"));
			Assert.IsFalse(source.HasElement("ELEM"));
		}

		[Test]
		public void HasElementTest2()
		{
			XContainer source = XDocument.Parse("<elem>1</elem>");
			Assert.IsTrue(source.HasElement("elem", StringComparison.CurrentCulture));
			Assert.IsTrue(source.HasElement("ELEM", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(source.HasElement("ELEM", StringComparison.CurrentCulture));
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
		public void RoundTest()
		{
			double value = 9.00233;
			Assert.AreEqual(Math.Round(value, 0), value.Round(0));
			Assert.AreEqual(Math.Round(value, 1), value.Round(1));
			Assert.AreEqual(Math.Round(value, 2), value.Round(2));
			Assert.AreEqual(Math.Round(value, 3), value.Round(3));
		}

		[Test]
		public void ToBytesTest()
		{
			byte[] buffer = new byte[] { 1, 2, 3 };
			Stream stream = new MemoryStream(buffer);
			Assert.IsTrue(buffer.SameAs(stream.ToBytes()));
		}

		[Test]
		public void ToHexTest()
		{
			byte[] buffer = new byte[] { 1, 2, 3 };
			var expected = BitConverter.ToString(buffer).Replace("-", String.Empty);
			Assert.AreEqual(expected, buffer.ToHex());
		}

		[Test]
		public void ToRomanTest()
		{
			Assert.AreEqual("", 0.ToRoman());
			Assert.AreEqual("I", 1.ToRoman());
			Assert.AreEqual("II", 2.ToRoman());
			Assert.AreEqual("III", 3.ToRoman());
			Assert.AreEqual("IV", 4.ToRoman());
			Assert.AreEqual("V", 5.ToRoman());
			Assert.AreEqual("IX", 9.ToRoman());
			Assert.AreEqual("X", 10.ToRoman());
			Assert.AreEqual("L", 50.ToRoman());
			Assert.AreEqual("LI", 51.ToRoman());
			Assert.AreEqual("LXX", 70.ToRoman());
			Assert.AreEqual("C", 100.ToRoman());
			Assert.AreEqual("D", 500.ToRoman());
			Assert.AreEqual("DI", 501.ToRoman());
			Assert.AreEqual("DCCCXC", 890.ToRoman());
			Assert.AreEqual("CM", 900.ToRoman());
			Assert.AreEqual("CML", 950.ToRoman());
			Assert.AreEqual("CMLI", 951.ToRoman());
			Assert.AreEqual("M", 1000.ToRoman());
			Assert.AreEqual("MD", 1500.ToRoman());
			Assert.AreEqual("MDCVI", 1606.ToRoman());
			Assert.AreEqual("MDCCC", 1800.ToRoman());
			Assert.AreEqual("MCMX", 1910.ToRoman());
			Assert.AreEqual("MCMLIV", 1954.ToRoman());
			Assert.AreEqual("MCMXCVIII", 1998.ToRoman());
			Assert.AreEqual("MCMXCIX", 1999.ToRoman());
			Assert.AreEqual("MM", 2000.ToRoman());
			Assert.AreEqual("MMXI", 2011.ToRoman());
			Assert.AreEqual("MMDCCLI", 2751.ToRoman());
			Assert.AreEqual("MMM", 3000.ToRoman());
			Assert.AreEqual("¯V", 5000.ToRoman());
		}

		[Test]
		public void Trim1()
		{
			DateTime date = new DateTime(2011, 1, 1, 1, 1, 1, 1);
			DateTime expected = date.Subtract(new TimeSpan(date.TimeOfDay.Days, date.TimeOfDay.Hours, date.TimeOfDay.Minutes, date.TimeOfDay.Seconds, 0));
			DateTime actual = date.Trim();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Trim2()
		{
			DateTime date = new DateTime(2011, 1, 1, 1, 1, 1, 1);
			DateTime expected = date.Subtract(new TimeSpan(date.TimeOfDay.Days, date.TimeOfDay.Hours, date.TimeOfDay.Minutes, date.TimeOfDay.Seconds, 0));
			DateTime actual = date.Trim("ff");
			Assert.AreEqual(expected, actual);

			expected = date.Subtract(new TimeSpan(date.TimeOfDay.Days, date.TimeOfDay.Hours, date.TimeOfDay.Minutes, 0, 0));
			actual = date.Trim("ss");
			Assert.AreEqual(expected, actual);

			expected = date.Subtract(new TimeSpan(date.TimeOfDay.Days, date.TimeOfDay.Hours, 0, 0, 0));
			actual = date.Trim("mm");
			Assert.AreEqual(expected, actual);

			expected = date.Subtract(new TimeSpan(date.TimeOfDay.Days, 0, 0, 0, 0));
			actual = date.Trim("hh");
			Assert.AreEqual(expected, actual);

			expected = date.Subtract(new TimeSpan(0, 0, 0, 0, 0));
			actual = date.Trim("dd");
			Assert.AreEqual(expected, actual);
		}

		private void EqualsTest2Helper<T>(params T[] items)
		{
			IEnumerable<T> array1 = new List<T>(items);
			IEnumerable<T> array2 = new List<T>(items);

			Assert.IsTrue(array1.SameAs(array2));
			Assert.IsTrue(array2.SameAs(array1));

			List<T> array3 = new List<T>(items);
			array3.Add(default(T));

			Assert.IsFalse(array1.SameAs(array3));
			Assert.IsFalse(array3.SameAs(array1));
		}
	}
}