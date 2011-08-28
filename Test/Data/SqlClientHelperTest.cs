using Adenson.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data;

namespace Adenson.CoreTest.Data
{
	[TestClass]
	public class SqlClientHelperTest : SqlHelperBaseTest<SqlClientHelper>
	{
		#region Init

		[TestInitialize]
		public void TestInitialize()
		{
			target = new SqlClientHelper();
		}

		#endregion
		#region Tests

		[TestMethod]
		public void ConstructorTest1()
		{
			SqlClientHelper target = new SqlClientHelper();
		}

		[TestMethod]
		public void ConstructorTest2()
		{
			SqlClientHelper target = new SqlClientHelper("");
		}

		[TestMethod]
		public void ConstructorTest3()
		{
			ConnectionStringSettings connectionString = null; // TODO: Initialize to an appropriate value
			SqlClientHelper target = new SqlClientHelper(connectionString);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest1()
		{
			SqlClientHelper target = new SqlClientHelper(String.Empty);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest2()
		{
			SqlClientHelper target = new SqlClientHelper((string)null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest3()
		{
			SqlClientHelper target = new SqlClientHelper((string)null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest4()
		{
			SqlClientHelper target = new SqlClientHelper((ConnectionStringSettings)null);
		}

		#endregion
		#region Helpers

		protected override SqlClientHelper CreateTarget(string connectionString)
		{
			return new SqlClientHelper(connectionString);
		}

		#endregion
	}
}