using System;
using System.Configuration;
using Adenson.Data;
using NUnit.Framework;

namespace Adenson.Tests.Data
{
	[TestFixture]
	public class SqlHelperProviderTest
	{
		[Test]
		public void CreateTest()
		{
			ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["Default"];
			SqlHelperBase result1 = SqlHelperProvider.Create(cs);
			Assert.That(result1, Is.Not.Null.And.InstanceOf<SqlServerHelper>());
			SqlHelperBase result2 = SqlHelperProvider.Create();
			Assert.That(result2, Is.Not.Null.And.InstanceOf<SqlServerHelper>(), "Empty Create() loads default.");
		}

		[Test]
		public void CreateCustomTest()
		{
			ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["Custom"];
			SqlHelperBase result = SqlHelperProvider.Create(cs);
			Assert.That(result, Is.Not.Null.And.InstanceOf<CustomSqlHelper>());
		}
	}
}