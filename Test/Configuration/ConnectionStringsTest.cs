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
			Assert.AreEqual(ConnectionStrings.Default.ConnectionString, ConnectionStrings.Get(ConnectionStrings.DefaultKey));
			Assert.AreEqual(ConnectionStrings.Default.ConnectionString, ConnectionStrings.Get("doesnotexist", true));
			Assert.AreEqual(ConnectionStrings.Default.ConnectionString, ConnectionStrings.Get("Data Source=(local);Initial Catalog=TEST;", true));
			Assert.AreEqual("Data Source=(local);Initial Catalog=TEST;", ConnectionStrings.Get("Data Source=(local);Initial Catalog=TEST;", false));
			Assert.Throws<ConfigurationErrorsException>(delegate { ConnectionStrings.Get("gagagan", false); });
		}
	}
}