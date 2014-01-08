using System;
using System.Configuration;
using Adenson.Configuration;
using NUnit.Framework;

namespace Adenson.CoreTest.Configuration
{
	[TestFixture]
	public class ConnectionStringsTest
	{
		[Test]
		public void DefaultTest()
		{
			Assert.AreEqual("Data Source=(local);Initial Catalog=TEST;Integrated Security=True;MultipleActiveResultSets=true;", ConnectionStrings.Default.ConnectionString);
		}

		[Test]
		public void GetTest()
		{
			Assert.AreEqual(ConnectionStrings.Default.ConnectionString, ConnectionStrings.Get(ConnectionStrings.DefaultKey).ConnectionString);
			Assert.AreEqual(ConnectionStrings.Default.ConnectionString, ConnectionStrings.Get("doesnotexist", true).ConnectionString);
			Assert.AreEqual(ConnectionStrings.Default.ConnectionString, ConnectionStrings.Get("Data Source=(local);Initial Catalog=TEST;", true).ConnectionString);
			Assert.Throws<ConfigurationErrorsException>(delegate { ConnectionStrings.Get("gagagan", false); });
		}
	}
}