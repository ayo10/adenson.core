using Adenson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;

namespace Adenson.CoreTest
{
	[TestClass]
	public class TypeUtilTest
	{
		[TestMethod]
		public void CreateInstanceTest1()
		{
			Assert.AreEqual(0, TypeUtil.CreateInstance<int>());
			Assert.AreEqual(String.Empty, TypeUtil.CreateInstance<string>());
		}

		[TestMethod]
		public void CreateInstanceTest2()
		{
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod]
		public void CreateInstanceTest3()
		{
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod]
		public void CreateInstanceTest4()
		{
			Assert.AreEqual(0, (int)TypeUtil.CreateInstance(typeof(int).FullName));
			Assert.AreEqual(String.Empty, (int)TypeUtil.CreateInstance(typeof(string).FullName));
		}

		/// <summary>
		///A test for EnumParse
		///</summary>
		public void EnumParseTestHelper<T>()
		{
			string value = string.Empty; // TODO: Initialize to an appropriate value
			T expected = default(T); // TODO: Initialize to an appropriate value
			T actual;
			actual = TypeUtil.EnumParse<T>(value);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod]
		public void EnumParseTest()
		{
			EnumParseTestHelper<GenericParameterHelper>();
		}

		/// <summary>
		///A test for GetType
		///</summary>
		[TestMethod]
		public void GetTypeTest()
		{
			string typeName = string.Empty; // TODO: Initialize to an appropriate value
			Type expected = null; // TODO: Initialize to an appropriate value
			Type actual;
			actual = TypeUtil.GetType(typeName);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TryConvert
		///</summary>
		[TestMethod]
		public void TryConvertTest()
		{
			Type type = null; // TODO: Initialize to an appropriate value
			object value = null; // TODO: Initialize to an appropriate value
			object output = null; // TODO: Initialize to an appropriate value
			object outputExpected = null; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = TypeUtil.TryConvert(type, value, out output);
			Assert.AreEqual(outputExpected, output);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TryConvert
		///</summary>
		public void TryConvertTest1Helper<T>()
		{
			object value = null; // TODO: Initialize to an appropriate value
			T result = default(T); // TODO: Initialize to an appropriate value
			T resultExpected = default(T); // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = TypeUtil.TryConvert<T>(value, out result);
			Assert.AreEqual(resultExpected, result);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		[TestMethod]
		public void TryConvertTest1()
		{
			TryConvertTest1Helper<GenericParameterHelper>();
		}
	}
}
