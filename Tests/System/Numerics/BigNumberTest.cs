using System;
using System.Numerics;
using NUnit.Framework;

namespace Adenson.CoreTests.System.Numerics
{
	[TestFixture]
	public class BigNumberTest
	{
		[Test]
		public void EqualsTest()
		{
			Assert.That(BigNumber.Parse("1234").Equals(BigNumber.Parse("1234")), Is.True);
			Assert.That(BigNumber.Parse("01234").Equals(BigNumber.Parse("1234")), Is.True);
			Assert.That(BigNumber.Parse("1234").Equals(BigNumber.Parse("01234")), Is.True);
			Assert.That(BigNumber.Parse("1234.0").Equals(BigNumber.Parse("1234")), Is.True);
			Assert.That(BigNumber.Parse("1234").Equals(BigNumber.Parse("1234.0")), Is.True);
			Assert.That(BigNumber.Parse("1234").Equals(BigNumber.Parse("1234.01")), Is.False);

			object target3 = BigNumber.Parse("1234");
			Assert.That(BigNumber.Parse("1234").Equals(target3), Is.True);

			target3 = new object();
			Assert.That(BigNumber.Parse("1234").Equals(target3), Is.False);
		}

		////[Test]
		public void OverflowTest()
		{
			Random r = new Random();
			for (int i = 2; i < Int32.MaxValue; i *= 2)
			{
				try
				{
					byte[] left = new byte[i];
					byte[] right = new byte[i];
					for (int x = 0; x < i; x++)
					{
						left[x] = (byte)r.Next(9);
						right[x] = (byte)r.Next(9);
					}

					string s = String.Concat(left) + "." + String.Concat(right);
					BigNumber target = BigNumber.Parse(s);
					Assert.That(target.Value[0], Is.EqualTo(left));
					Assert.That(target.Value[1], Is.EqualTo(right));
				}
				catch (OutOfMemoryException)
				{
					Assert.Pass($"Fails at {i}");
				}
				catch (Exception)
				{
					Assert.Fail($"The only thing that should break this is being out of memory, nothing else should break it, but it did, at {i}");
				}
			}
		}

		[Test]
		public void Op_Add_Test()
		{
			Assert.That(BigNumber.Parse("1234") + BigNumber.Parse("1234"), Is.EqualTo(BigNumber.Parse("2468")));
			Assert.That(BigNumber.Parse("1234") + BigNumber.Parse("56789"), Is.EqualTo(BigNumber.Parse("58023")));
			Assert.That(BigNumber.Parse("1234.56") + BigNumber.Parse("56789.0"), Is.EqualTo(BigNumber.Parse("58023.56")));
			Assert.That(BigNumber.Parse("54321.03") + BigNumber.Parse("54321.03"), Is.EqualTo(BigNumber.Parse("108642.06")));
			Assert.That(BigNumber.Parse("54321.03") + BigNumber.Parse("-54321.03"), Is.EqualTo(BigNumber.Zero));
			Assert.That(BigNumber.Parse("-54321.03") + BigNumber.Parse("54321.03"), Is.EqualTo(BigNumber.Zero));
			Assert.That(BigNumber.Parse("54321.03") + BigNumber.Parse("-108642.06"), Is.EqualTo(BigNumber.Parse("-54321.03")));
			Assert.That(BigNumber.Parse("1234.666666") + BigNumber.Parse("1234.666666"), Is.EqualTo(BigNumber.Parse("2469.333332")));
		}

		[Test]
		public void Op_Equal_Test()
		{
			Assert.That(BigNumber.Parse("1234") == BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("01234") == BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234") == BigNumber.Parse("01234"), Is.True);
			Assert.That(BigNumber.Parse("1234.0") == BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234") == BigNumber.Parse("1234.0"), Is.True);
			Assert.That(BigNumber.Parse("1234") == BigNumber.Parse("1234.01"), Is.False);
		}

		[Test]
		public void Op_GreaterThan_Test()
		{
			Assert.That(BigNumber.Parse("1234") > BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234") > BigNumber.Parse("-1234"), Is.True);
			Assert.That(BigNumber.Parse("-1234") > BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234.1") > BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234.000000001") > BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234") > BigNumber.Parse("1234.000000001"), Is.False);
			Assert.That(BigNumber.Parse("-1234.000000001") > BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("-1234") > BigNumber.Parse("-1234.000000001"), Is.True);
			Assert.That(BigNumber.Parse("-1234.000000001") > BigNumber.Parse("-1234"), Is.False);
		}

		[Test]
		public void Op_GreaterThanOrEqual_Test()
		{
			Assert.That(BigNumber.Parse("1234") >= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("-1234") >= BigNumber.Parse("-1234"), Is.True);
			Assert.That(BigNumber.Parse("1234") >= BigNumber.Parse("-1234"), Is.True);
			Assert.That(BigNumber.Parse("-1234") >= BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234.1") >= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234.000000001") >= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234") >= BigNumber.Parse("1234.000000001"), Is.False);
			Assert.That(BigNumber.Parse("-1234.000000001") >= BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("-1234") >= BigNumber.Parse("-1234.000000001"), Is.False);
			Assert.That(BigNumber.Parse("-1234.000000001") >= BigNumber.Parse("-1234"), Is.False);
		}

		[Test]
		public void Op_NotEqual_Test()
		{
			BigNumber target1 = BigNumber.Parse("1234");
			BigNumber target2 = BigNumber.Parse("1234.5");
			Assert.That(target1 != target2, Is.True);

			target1 = BigNumber.Parse("1234.00000001");
			target2 = BigNumber.Parse("1234.00000000");
			Assert.That(target1 != target2, Is.True);
		}

		[Test]
		public void Op_LessThan_Test()
		{
			Assert.That(BigNumber.Parse("1234") < BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234") < BigNumber.Parse("-1234"), Is.False);
			Assert.That(BigNumber.Parse("-1234") < BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234.1") < BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234.000000001") < BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234") < BigNumber.Parse("1234.000000001"), Is.True);
			Assert.That(BigNumber.Parse("-1234.000000001") < BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("-1234") < BigNumber.Parse("-1234.000000001"), Is.False);
			Assert.That(BigNumber.Parse("-1234.000000001") < BigNumber.Parse("-1234"), Is.True);
		}

		[Test]
		public void Op_LessThanOrEqual_Test()
		{
			Assert.That(BigNumber.Parse("1234") <= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("-1234") <= BigNumber.Parse("-1234"), Is.True);
			Assert.That(BigNumber.Parse("1234") <= BigNumber.Parse("-1234"), Is.False);
			Assert.That(BigNumber.Parse("-1234") <= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234.1") <= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("1234.000000001") <= BigNumber.Parse("1234"), Is.False);
			Assert.That(BigNumber.Parse("1234") <= BigNumber.Parse("1234.000000001"), Is.True);
			Assert.That(BigNumber.Parse("-1234.000000001") <= BigNumber.Parse("1234"), Is.True);
			Assert.That(BigNumber.Parse("-1234") <= BigNumber.Parse("-1234.000000001"), Is.True);
			Assert.That(BigNumber.Parse("-1234.000000001") <= BigNumber.Parse("-1234"), Is.True);
		}

		[Test]
		public void Op_Subtract_Test()
		{
			Assert.That(BigNumber.Parse("1234") - BigNumber.Parse("1234"), Is.EqualTo(BigNumber.Zero));
			Assert.That(BigNumber.Parse("1234") - BigNumber.Parse("54321"), Is.EqualTo(BigNumber.Parse("-107408")));
			Assert.That(BigNumber.Parse("-1234") - BigNumber.Parse("-54321"), Is.EqualTo(BigNumber.Parse("55555")));
		}

		[Test]
		public void Parse_String_Test()
		{
			BigNumber target = BigNumber.Parse("1234");
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.Empty);

			target = BigNumber.Parse("01234");
			Assert.That(target.IsNegative, Is.False);
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.Empty);

			target = BigNumber.Parse("1234.5678");
			Assert.That(target.IsNegative, Is.False);
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.EqualTo(new byte[] { 5, 6, 7, 8 }));

			target = BigNumber.Parse("1234.05678");
			Assert.That(target.IsNegative, Is.False);
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.EqualTo(new byte[] { 0, 5, 6, 7, 8 }));

			target = BigNumber.Parse("1234.56780");
			Assert.That(target.IsNegative, Is.False);
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.EqualTo(new byte[] { 5, 6, 7, 8 }));

			target = BigNumber.Parse("01234.56780");
			Assert.That(target.IsNegative, Is.False);
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.EqualTo(new byte[] { 5, 6, 7, 8 }));

			target = BigNumber.Parse("-1234.000000001");
			Assert.That(target.IsNegative, Is.True);
			Assert.That(target.Value[0], Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
			Assert.That(target.Value[1], Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 }));

			Assert.That(BigNumber.Parse("0"), Is.EqualTo(BigNumber.Zero));

			Assert.Throws<FormatException>(() => target = BigNumber.Parse("a.5678"));
			Assert.Throws<FormatException>(() => target = BigNumber.Parse("123b.5678"));
			Assert.Throws<FormatException>(() => target = BigNumber.Parse("1234.5x78.9"));
			Assert.Throws<FormatException>(() => target = BigNumber.Parse("1234.5678.9"));
		}

		[Test]
		public void ToStringTest()
		{
			Assert.That(BigNumber.Parse("0").ToString(), Is.EqualTo("0"));
			Assert.That(BigNumber.Parse("1").ToString(), Is.EqualTo("1"));
			Assert.That(BigNumber.Parse("1234567890").ToString(), Is.EqualTo("1234567890"));
			Assert.That(BigNumber.Parse("1234567890.0987654321").ToString(), Is.EqualTo("1234567890.0987654321"));
		}

		[Test]
		public void ZeroTest()
		{
			Assert.That(BigNumber.Zero, Is.SameAs(BigNumber.Zero));
			Assert.That(BigNumber.Parse("0"), Is.SameAs(BigNumber.Zero).And.EqualTo(BigNumber.Zero));
			Assert.That(BigNumber.Zero, Is.SameAs(BigNumber.Parse("0")).And.EqualTo(BigNumber.Parse("0")));
		}
	}
}
