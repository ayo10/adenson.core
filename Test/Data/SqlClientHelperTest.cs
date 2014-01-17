using System;
using System.Configuration;
using Adenson.Data;
using NUnit.Framework;

namespace Adenson.CoreTest.Data
{
	[TestFixture]
	public class SqlClientHelperTest : SqlHelperBaseTest<SqlClientHelper>
	{
		#region Init

		[TestFixtureSetUp]
		public void TestInitialize()
		{
			target = new SqlClientHelper();
		}

		#endregion
		#region Tests

		[Test]
		public void ConstructorTest1()
		{
			SqlClientHelper target = new SqlClientHelper();
		}

		[Test]
		public void ConstructorTest2()
		{
			SqlClientHelper target = new SqlClientHelper("");
		}

		[Test]
		public void ConstructorTest3()
		{
			ConnectionStringSettings connectionString = null; // TODO: Initialize to an appropriate value
			SqlClientHelper target = new SqlClientHelper(connectionString);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest1()
		{
			SqlClientHelper target = new SqlClientHelper(String.Empty);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest2()
		{
			SqlClientHelper target = new SqlClientHelper((string)null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorFailTest3()
		{
			SqlClientHelper target = new SqlClientHelper((string)null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
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