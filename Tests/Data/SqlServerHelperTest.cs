using System;
using System.Configuration;
using Adenson.Data;
using NUnit.Framework;

namespace Adenson.CoreTest.Data
{
	[TestFixture]
	public class SqlServerHelperTest : SqlHelperBaseTest<SqlServerHelper>
	{
		#region Init

		[OneTimeSetUp]
		public void TestInitialize()
		{
			this.Target = new SqlServerHelper();
			this.Target.DropDatabase();
			this.Target.CreateDatabase();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			this.Target.DropDatabase();
		}

		#endregion
		#region Tests

		[Test]
		public void ConstructorEmptyStringFailTest()
		{
			Assert.That(() => new SqlServerHelper(String.Empty), Throws.ArgumentNullException);
		}

		[Test]
		public void ConstructorNullStringFailTest()
		{
			Assert.That(() => new SqlServerHelper((string)null), Throws.ArgumentNullException);
		}

		#endregion
		#region Helpers

		protected override SqlServerHelper CreateTarget()
		{
			this.ConnectionString = "Data Source=(local);Initial Catalog=TEST;Integrated Security=True;MultipleActiveResultSets=true;";
			return new SqlServerHelper(ConnectionString);
		}

		#endregion
	}
}