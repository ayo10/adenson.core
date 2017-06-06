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
			Target = new SqlClientHelper();
			Target.DropDatabase();
			Target.CreateDatabase();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			Target.DropDatabase();
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
			ConnectionString = "Data Source=(local);Initial Catalog=TEST;Integrated Security=True;MultipleActiveResultSets=true;";
			return new SqlClientHelper(ConnectionString);
		}

		#endregion
	}
}