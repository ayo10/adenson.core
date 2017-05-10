using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;

namespace Adenson.CoreTest.System
{
	[TestFixture]
	public class StringUtilTest
	{
		#region Tests

		[Test]
		public void FormatTest()
		{
			Assert.AreEqual("Test1 Test2", StringUtil.Format("Test1 {0}", "Test2"));
			Assert.AreEqual("Test1 Test2 {1}", StringUtil.Format("Test1 {0} {1}", "Test2"));
			Assert.AreEqual("Test1 {1}", StringUtil.Format("Test1 {1}", "Test2"));
		}

		[Test]
		public void GenerateRandomStringTest()
		{
			int length = 10;
			var target = StringUtil.GenerateRandomString(length);
			Assert.IsFalse(String.IsNullOrEmpty(target));
			Assert.AreEqual(length, target.Length);
		}

		[Test]
		public void IsNullOrWhiteSpaceTest()
		{
			Assert.IsTrue(StringUtil.IsNullOrWhiteSpace(String.Empty));
			Assert.IsTrue(StringUtil.IsNullOrWhiteSpace(" "));
			Assert.IsFalse(StringUtil.IsNullOrWhiteSpace(" Test "));
		}

		[Test]
		public void ToStringTest()
		{
			object value = null;
			Assert.AreEqual(null, StringUtil.ToString(value));

			value = "test";
			Assert.AreEqual((string)value, StringUtil.ToString(value));

			value = 5;
			Assert.AreEqual(value.ToString(), StringUtil.ToString(value));

			value = new Exception("Test1");
			Assert.AreEqual("System.Exception: Test1", StringUtil.ToString(value));

			value = new byte[] { 1, 2, 3 };
			Assert.AreEqual("byte[]{1,2,3}", StringUtil.ToString(value));

			value = new[] { 1, 2, 3 };
			Assert.AreEqual("int[]{1,2,3}", StringUtil.ToString(value));

			value = new[] { "1", "2", "3" };
			Assert.AreEqual("string[]{1,2,3}", StringUtil.ToString(value));

			value = new[] { "1", "2", null };
			Assert.AreEqual("string[]{1,2,null}", StringUtil.ToString(value));

			value = new[] { new object(), null, new object() };
			Assert.AreEqual("object[]{System.Object,null,System.Object}", StringUtil.ToString(value));

			value = new[] { new Woot() };
			Assert.AreEqual("Woot[]{Wootee}", StringUtil.ToString(value));

			value = new[] { new Woot(), new object() };
			Assert.AreEqual("object[]{Wootee,System.Object}", StringUtil.ToString(value));

			value = new List<int> { 1, 2, 3 };
			Assert.AreEqual("List<int>{1,2,3}", StringUtil.ToString(value));

			value = new List<object> { 1, 2, new byte[] { 1, 2, 3 } };
			Assert.AreEqual("List<object>{1,2,byte[]{1,2,3}}", StringUtil.ToString(value));
		}

		[Test]
		public void ToStringObjectDefaultCultureTest()
		{
			object value = true;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = (byte)2;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = 'c';
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = DateTime.Now;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = 2d;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));
		}

		[Test, SetCulture("fr-CA")]
		public void ToStringObjectFrenchCultureTest()
		{
			object value = true;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = (byte)2;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = 'c';
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = DateTime.Now;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));

			value = 2d;
			Assert.AreEqual(Convert.ToString(value, CultureInfo.CurrentCulture), StringUtil.ToString(value));
		}

		[Test]
		public void ToStringExceptionTest()
		{
			Exception exception = new Exception("Test1");
			Assert.AreEqual("System.Exception: Test1", StringUtil.ToString(exception));

			exception = new Exception("Test1", new Exception("Test2"));
			Assert.AreEqual("System.Exception: Test1\r\nSystem.Exception: Test2", StringUtil.ToString(exception));

			exception.HelpLink = "Woot";
			Assert.AreEqual("System.Exception: Test1\r\n\tHelpLink: Woot, Source: \r\nSystem.Exception: Test2", StringUtil.ToString(exception));

			exception.Source = "Source";
			Assert.AreEqual("System.Exception: Test1\r\n\tHelpLink: Woot, Source: Source\r\nSystem.Exception: Test2", StringUtil.ToString(exception));

			exception.HelpLink = null;
			Assert.AreEqual("System.Exception: Test1\r\n\tHelpLink: , Source: Source\r\nSystem.Exception: Test2", StringUtil.ToString(exception));
			
			exception = new Exception("Test2", new Exception("Test1"));
			Assert.AreEqual("System.Exception: Test2\r\nSystem.Exception: Test1", StringUtil.ToString(exception));

			exception.Data.Add("Key1", "One");
			Assert.AreEqual("System.Exception: Test2\r\n\tData:\r\n\t\tKey1: One\r\nSystem.Exception: Test1", StringUtil.ToString(exception));

			var copy = exception;
			copy.Data.Clear();
			exception = new ReflectionTypeLoadException(new Type[] { typeof(Int32) }, new Exception[] { copy });
			Assert.AreEqual("System.Reflection.ReflectionTypeLoadException: Exception of type 'System.Reflection.ReflectionTypeLoadException' was thrown.\r\n\tTypes: System.Int32\r\n\tLoaderExceptions:\r\n\t\tSystem.Exception: Test2\r\n\t\tSystem.Exception: Test1", StringUtil.ToString(exception));

			exception = new ReflectionTypeLoadException(new Type[] { typeof(Int32) }, new Exception[] { copy }, "Woot wooter");
			Assert.AreEqual("System.Reflection.ReflectionTypeLoadException: Woot wooter\r\n\tTypes: System.Int32\r\n\tLoaderExceptions:\r\n\t\tSystem.Exception: Test2\r\n\t\tSystem.Exception: Test1", StringUtil.ToString(exception));
		}

		#endregion
		#region Classes

		private class Woot
		{
			public override string ToString()
			{
				return "Wootee";
			}
		}

		#endregion
	}
}