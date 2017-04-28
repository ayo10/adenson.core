using System;
using Adenson.Log;
using NUnit.Framework;

namespace Adenson.CoreTest.System
{
	[TestFixture]
	public class TypeUtilTest
	{
		[Test]
		public void CreateInstanceTest1()
		{
			var result = TypeUtil.CreateInstance<TestClass>();
			Assert.IsNotNull(result);
			Assert.IsInstanceOf(typeof(TestClass), result);
		}

		[Test]
		public void CreateInstanceTest2()
		{
			var typeName = typeof(TestClass).AssemblyQualifiedName;
			var result = TypeUtil.CreateInstance<TestClass>(typeName);
			Assert.IsNotNull(result);
			Assert.IsInstanceOf(typeof(TestClass), result);
		}

		[Test]
		public void CreateInstanceTest3()
		{
			var type = typeof(TestClass);
			var result = TypeUtil.CreateInstance<TestClass>(type);
			Assert.IsNotNull(result);
			Assert.IsInstanceOf(typeof(TestClass), result);
		}

		[Test]
		public void EnumParseTest()
		{
			Assert.AreEqual(TestEnum.Enum1, TypeUtil.EnumParse<TestEnum>(TestEnum.Enum1.ToString()));
		}

		[Test]
		public void GetTypeTest()
		{
			var type = typeof(TestClass);
			Assert.AreEqual(type, TypeUtil.GetType(type.AssemblyQualifiedName));
			Assert.AreEqual(type, TypeUtil.GetType(String.Format("{0};{1}", type.Assembly.GetName().Name, type.FullName)));
		}

		[Test]
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

		[Test]
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

		[Test]
		public void GetPropertyValuesTest()
		{
		}

		[Test]
		public void TryConvertTest()
		{
			object output = null;
			Assert.IsTrue(TypeUtil.TryConvert(typeof(int), "1", out output));
			Assert.AreEqual(1, output);

			Assert.IsTrue(TypeUtil.TryConvert(typeof(TestEnum), TestEnum.Enum1.ToString(), out output));
			Assert.AreEqual(TestEnum.Enum1, output);

			Assert.IsTrue(TypeUtil.TryConvert(typeof(TestEnum), "Enum1 | Enum2 | Enum4 | Enum16", out output));
			Assert.AreEqual(TestEnum.All, output);
		}

		[Test]
		public void TryConvertTest1()
		{
			int intResult = 1;
			Assert.IsTrue(TypeUtil.TryConvert<int>("1", out intResult));
			Assert.AreEqual(1, intResult);

			TestEnum damResult;
			Assert.IsTrue(TypeUtil.TryConvert<TestEnum>(TestEnum.Enum1.ToString(), out damResult));
			Assert.AreEqual(TestEnum.Enum1, damResult);

			TestEnum ltResult;
			Assert.IsTrue(TypeUtil.TryConvert<TestEnum>("Enum1 | Enum2 | Enum4 | Enum16", out ltResult));
			Assert.AreEqual(TestEnum.All, ltResult);
		}

		[Flags]
		public enum TestEnum
		{
			None = 0,
			Enum1 = 1,
			Enum2 = 2,
			Enum4 = 4,
			Enum16 = 16,
			All = Enum1 | Enum2 | Enum4 | Enum16
		}

		public class TestClass
		{
		}
	}
}