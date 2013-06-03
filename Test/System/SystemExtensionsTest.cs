using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Adenson.CoreTest.Sys
{
	[TestFixture]
	public class SystemExtensionsTest
	{
		[Test]
		public void ContainsTest()
		{
			string source = "TestOneTwoThree";
			Assert.IsTrue(source.Contains("One", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsTrue(source.Contains("one", StringComparison.CurrentCultureIgnoreCase));
			Assert.IsFalse(source.Contains("Four", StringComparison.CurrentCultureIgnoreCase));
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
			Assert.IsTrue(buffer.IsEquivalentTo(stream.ToBytes()));
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
		public void TrimDateTest1()
		{
			DateTime date = new DateTime(2011, 1, 1, 1, 1, 1, 1);
			DateTime expected = date.Subtract(new TimeSpan(date.TimeOfDay.Days, date.TimeOfDay.Hours, date.TimeOfDay.Minutes, date.TimeOfDay.Seconds, 0));
			DateTime actual = date.Trim();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TrimDateTest2()
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
	}
}