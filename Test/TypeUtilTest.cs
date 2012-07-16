using System;
using Adenson.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adenson.CoreTest
{
	[TestClass]
	public class TypeUtilTest
	{
		[TestMethod]
		public void CreateInstanceTest1()
		{
			var result = TypeUtil.CreateInstance<GenericParameterHelper>();
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(GenericParameterHelper));
		}

		[TestMethod]
		public void CreateInstanceTest2()
		{
			var typeName = typeof(GenericParameterHelper).AssemblyQualifiedName;
			var result = TypeUtil.CreateInstance<GenericParameterHelper>(typeName);
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(GenericParameterHelper));
		}

		[TestMethod]
		public void CreateInstanceTest3()
		{
			var type = typeof(GenericParameterHelper);
			var result = TypeUtil.CreateInstance<GenericParameterHelper>(type);
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(GenericParameterHelper));
		}

		[TestMethod]
		public void EnumParseTest()
		{
			Assert.AreEqual(DataAccessMethod.Random, TypeUtil.EnumParse<DataAccessMethod>(DataAccessMethod.Random.ToString()));
		}

		[TestMethod]
		public void GetTypeTest()
		{
			var type = typeof(GenericParameterHelper);
			Assert.AreEqual(type, TypeUtil.GetType(type.AssemblyQualifiedName));
			Assert.AreEqual(type, TypeUtil.GetType(String.Format("{0};{1}", type.Assembly.GetName().Name, type.FullName)));
		}

		[TestMethod]
		public void GetPropertyDescriptorTest()
		{
			var anon1 = new { Prop1 = "Test" };
			var descriptor = TypeUtil.GetPropertyDescriptor(anon1, "Prop1");
			Assert.IsNotNull(descriptor);
			Assert.AreEqual("Prop1", descriptor.Name);

			var anon2 = new { Prop2 = "Test", Item = anon1 };
			descriptor = TypeUtil.GetPropertyDescriptor(anon2, "Prop2");
			Assert.IsNotNull(descriptor);
			Assert.AreEqual("Prop2", descriptor.Name);
			
			descriptor = TypeUtil.GetPropertyDescriptor(anon2, "Item");
			Assert.IsNotNull(descriptor);
			Assert.AreEqual("Item", descriptor.Name);

			descriptor = TypeUtil.GetPropertyDescriptor(anon2, "Item.Prop1");
			Assert.IsNotNull(descriptor);
			Assert.AreEqual("Prop1", descriptor.Name);
		}

		[TestMethod]
		public void GetPropertyValueTest()
		{
			var anon1 = new { Prop1 = "Test1" };
			var value = TypeUtil.GetPropertyValue(anon1, "Prop1");
			Assert.IsNotNull(value);
			Assert.AreEqual("Test1", value);

			var anon2 = new { Prop2 = "Test2", Item = anon1 };
			value = TypeUtil.GetPropertyValue(anon2, "Prop2");
			Assert.IsNotNull(value);
			Assert.AreEqual("Test2", value);

			value = TypeUtil.GetPropertyValue(anon2, "Item");
			Assert.IsNotNull(value);
			Assert.AreEqual(anon1, value);

			value = TypeUtil.GetPropertyValue(anon2, "Item.Prop1");
			Assert.IsNotNull(value);
			Assert.AreEqual("Test1", value);
		}

		[TestMethod]
		public void GetPropertyValuesTest()
		{
		}

		[TestMethod]
		public void TryConvertTest()
		{
			object output = null;
			Assert.IsTrue(TypeUtil.TryConvert(typeof(int), "1", out output));
			Assert.AreEqual(1, output);

			Assert.IsTrue(TypeUtil.TryConvert(typeof(DataAccessMethod), DataAccessMethod.Random.ToString(), out output));
			Assert.AreEqual(DataAccessMethod.Random, output);

			Assert.IsTrue(TypeUtil.TryConvert(typeof(LogTypes), "Database | File | EventLog | Trace | Email", out output));
			Assert.AreEqual(LogTypes.All, output);
		}

		[TestMethod]
		public void TryConvertTest1()
		{
			int intResult = 1;
			Assert.IsTrue(TypeUtil.TryConvert<int>("1", out intResult));
			Assert.AreEqual(1, intResult);

			DataAccessMethod damResult;
			Assert.IsTrue(TypeUtil.TryConvert<DataAccessMethod>(DataAccessMethod.Random.ToString(), out damResult));
			Assert.AreEqual(DataAccessMethod.Random, damResult);

			LogTypes ltResult;
			Assert.IsTrue(TypeUtil.TryConvert<LogTypes>("Database | File | EventLog | Trace | Email", out ltResult));
			Assert.AreEqual(LogTypes.All, ltResult);
		}
	}
}