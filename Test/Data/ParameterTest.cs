using Adenson.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Adenson.CoreTest.Data
{
	[TestClass]
	public class ParameterTest
	{
		[TestMethod]
		public void ParameterConstructorTest()
		{
			string name = "Name";
			object value = "Value";
			Parameter target = new Parameter(name, value);
			Assert.AreEqual(name, target.Name);
			Assert.AreEqual(value, target.Value);
		}

		[TestMethod]
		public void EqualsTest1()
		{
			string name = string.Empty; // TODO: Initialize to an appropriate value
			object value = null; // TODO: Initialize to an appropriate value
			Parameter target = new Parameter(name, value); // TODO: Initialize to an appropriate value
			object obj = null; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.Equals(obj);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void EqualsTest2()
		{
			string name = string.Empty; // TODO: Initialize to an appropriate value
			object value = null; // TODO: Initialize to an appropriate value
			Parameter target = new Parameter(name, value); // TODO: Initialize to an appropriate value
			Parameter other = null; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.Equals(other);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetHashCodeTest()
		{
			string name = string.Empty; // TODO: Initialize to an appropriate value
			object value = null; // TODO: Initialize to an appropriate value
			Parameter target = new Parameter(name, value); // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.GetHashCode();
			Assert.AreEqual(expected, actual);
		}
	}
}