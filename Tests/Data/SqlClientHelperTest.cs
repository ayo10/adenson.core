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

		[OneTimeSetUp]
		public void TestInitialize()
		{
			target = new SqlClientHelper();
			target.DropDatabase();
			target.CreateDatabase();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			target.DropDatabase();
		}

		#endregion
		#region Tests

		[Test]
		public void ConstructorEmptyStringFailTest()
		{
			Assert.That(() => new SqlClientHelper(String.Empty), Throws.ArgumentNullException);
		}

		[Test]
		public void ConstructorNullStringFailTest()
		{
			Assert.That(() => new SqlClientHelper((string)null), Throws.ArgumentNullException);
		}

		[Test]
		public void ConstructorNullSettingsTest()
		{
			Assert.That(() => new SqlClientHelper((ConnectionStringSettings)null), Throws.ArgumentNullException);
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