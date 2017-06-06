using Adenson.Data;
using NUnit.Framework;

namespace Adenson.CoreTest.Data
{
	[TestFixture]
	public class ParameterTest
	{
		[Test]
		public void ParameterConstructorTest()
		{
			string name = "Name";
			object value = "Value";
			Parameter target = new Parameter(name, value);
			Assert.AreEqual(name, target.Name);
			Assert.AreEqual(value, target.Value);
		}

		[Test]
		public void EqualsTest1()
		{
			Parameter target = new Parameter("name1", "value1");
			object other = new Parameter("name2", "value2");
			Assert.IsTrue(target.Equals(target));
			Assert.IsFalse(target.Equals(other));
		}

		[Test]
		public void EqualsTest2()
		{
			Parameter target = new Parameter("name1", "value1");
			Parameter other = new Parameter("name2", "value2");
			Assert.IsTrue(target.Equals(target));
			Assert.IsFalse(target.Equals(other));
		}

		[Test]
		public void GetHashCodeTest()
		{
			Parameter target = new Parameter("name1", "value1");
			object reference = target;
			Assert.AreEqual(reference.GetHashCode(), target.GetHashCode());
		}
	}
}