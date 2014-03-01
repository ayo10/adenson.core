using System;
using System.Configuration;
using Adenson.Configuration;
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
			target.DropDatabase();
			target.CreateDatabase();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			target.DropDatabase();
		}

		#endregion
		#region Tests

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorEmptyStringFailTest()
		{
			SqlClientHelper target = new SqlClientHelper(String.Empty);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNullStringFailTest()
		{
			SqlClientHelper target = new SqlClientHelper((string)null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNullSettingsTest()
		{
			SqlClientHelper target = new SqlClientHelper((ConnectionStringSettings)null);
		}

		#endregion
		#region Helpers

		protected override SqlClientHelper CreateTarget()
		{
			connectionString = "Data Source=(local);Initial Catalog=TEST;Integrated Security=True;MultipleActiveResultSets=true;";
			return new SqlClientHelper(connectionString);
		}

		#endregion
	}
}