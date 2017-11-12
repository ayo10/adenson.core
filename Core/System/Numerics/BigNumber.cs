using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Numerics
{
	/// <summary>
	/// Represents an abnormally large number, constrained only by size of available memory (brrr!!!).
	/// </summary>
	/// <remarks>Because this class is immutable, beware, any operations on it both take n linear time and n space. Uses two arrays to store the digits and the exponents, though it occurred to me I could have used two BigInteger to do the same, but, feh!</remarks>
	public struct BigNumber : IComparable, IComparable<BigNumber>, IEquatable<BigNumber>
	{
		#region Constants

		/// <summary>
		/// Returns a value that represents 0;
		/// </summary>
		public static readonly BigNumber Zero = new BigNumber(new byte[0], new byte[0], false);

		/// <summary>
		/// Returns a value that represents 1.
		/// </summary>
		public static readonly BigNumber One = new BigNumber(new byte[] { 1 }, new byte[0], false);

		#endregion
		#region Fields
		private byte[] _digits;
		private byte[] _fac;
		private bool _negative;
		#endregion
		#region Constructor

		private BigNumber(byte[] digits, byte[] exponent, bool negative) : this()
		{
			_digits = digits;
			_fac = exponent;
			_negative = negative;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the double-length-ed array that represents the value the object represents.
		/// </summary>
		public byte[][] Value
		{
			get { return new[] { _digits, _fac }; }
		}

		/// <summary>
		/// Gets a value indicating whether the number is a negative number.
		/// </summary>
		public bool IsNegative
		{
			get { return _negative; }
		}

		/// <summary>
		/// Gets a value indicating whether the number is 0.
		/// </summary>
		public bool IsZero
		{
			get { return _digits.Length == 0 && _fac.Length == 0; }
		}

		#endregion
		#region Operators

		/// <summary>
		/// Sums the values of the two.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>The sum of the two values.</returns>
		public static BigNumber operator +(BigNumber left, BigNumber right)
		{
			return left.Add(right);
		}

		/// <summary>
		/// Gets the difference of the two.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>The difference of the two.</returns>
		public static BigNumber operator -(BigNumber left, BigNumber right)
		{
			return left.Subtract(right);
		}

		/// <summary>
		/// Checks if <paramref name="left"/> is greater than <paramref name="right"/>.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>, false otherwise.</returns>
		public static bool operator >(BigNumber left, BigNumber right)
		{
			return left.GreaterThan(right);
		}

		/// <summary>
		/// Checks if <paramref name="left"/> is greater than <paramref name="right"/>.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>, false otherwise.</returns>
		public static bool operator >=(BigNumber left, BigNumber right)
		{
			return left.Equals(right) || left.GreaterThan(right);
		}

		/// <summary>
		/// Checks if <paramref name="left"/> is less than <paramref name="right"/>.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>True if <paramref name="left"/> is less than <paramref name="right"/>, false otherwise.</returns>
		public static bool operator <(BigNumber left, BigNumber right)
		{
			return left.LessThan(right);
		}

		/// <summary>
		/// Checks if <paramref name="left"/> is greater than <paramref name="right"/>.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>, false otherwise.</returns>
		public static bool operator <=(BigNumber left, BigNumber right)
		{
			return left.Equals(right) || left.LessThan(right);
		}

		/// <summary>
		/// Gets if the two objects contain the same values.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>True if their values are the same, false otherwise.</returns>
		public static bool operator ==(BigNumber left, BigNumber right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Gets if the two objects have different values.
		/// </summary>
		/// <param name="left">The left side of the expression to check.</param>
		/// <param name="right">The right side of the expression to check.</param>
		/// <returns>True if their values are NOT the same, false otherwise.</returns>
		public static bool operator !=(BigNumber left, BigNumber right)
		{
			return !left.Equals(right);
		}

		#endregion
		#region Methods

		/// <summary>
		/// Converts a string into a <see cref=" BigNumber"/>.
		/// </summary>
		/// <param name="number">The string to convert.</param>
		/// <returns>A <see cref="BigNumber"/> instance.</returns>
		/// <exception cref="FormatException">If the number is NOT numeric.</exception>
		public static BigNumber Parse(string number)
		{
			Arg.IsNotEmpty(number);
			if (number.Length >= Int32.MaxValue)
			{
				throw new OverflowException("The value being parsed is too big.");
			}

			if (number == "0")
			{
				return BigNumber.Zero;
			}

			// Since Lists auto expand, that is what we will be using.
			List<byte> digits = new List<byte>();
			List<byte> exponent = new List<byte>();
			List<byte> curr = digits;
			bool negative = false;
			for (int i = 0; i < number.Length; i++)
			{
				char c = number[i];
				if (i == 0 && c == '-')
				{
					negative = true;
				}
				else if (c == '.' && curr == digits)
				{
					curr = exponent;
				}
				else if (c == '.' || !Char.IsDigit(c))
				{
					throw new FormatException("Cannot parse the specified value as a BigNumber");
				}
				else
				{
					byte b = (byte)(c - '0');
					curr.Add(b);
				}
			}
			
			return BigNumber.Convert(digits, exponent, negative);
		}

		/// <inheritdoc />
		public int CompareTo(BigNumber other)
		{
			return BigNumber.Compare(this, other);
		}

		/// <inheritdoc />
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			else if (obj is BigNumber)
			{
				return this.CompareTo((BigNumber)obj);
			}
			else
			{
				throw new ArgumentException("Value must be of type BigNumber.");
			}
		}

		/// <inheritdoc />
		public bool Equals(BigNumber other)
		{
			return BigNumber.Compare(this, other) == 0;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (!(obj is BigNumber))
			{
				return false;
			}

			return this.Equals((BigNumber)obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			char[] c = new char[_digits.Length + (_fac.Length > 0 ? (_fac.Length + 1) : 0) + (this.IsNegative ? 1 : 0)];
			int x = 0;
			bool neg = this.IsNegative;
			if (neg)
			{
				c[x++] = '-';
			}

			for (int i = 0; i < _digits.Length; i++)
			{
				c[x++] = (char)(((neg ? -1 : 1) * _digits[i]) + 48);
			}

			if (_fac.Length > 0)
			{
				c[x++] = '.';
				for (int i = 0; i < _fac.Length; i++)
				{
					c[x++] = (char)(((neg ? -1 : 1) * _fac[i]) + 48);
				}
			}

			return new string(c);
		}

		/// <summary>
		/// Adds the current instance and specified value, returning a brand new value.
		/// </summary>
		/// <param name="value">The value to add.</param>
		/// <returns>A new value containining the sum.</returns>
		public BigNumber Add(BigNumber value)
		{
			if (this.IsZero && value.IsZero)
			{
				return BigNumber.Zero;
			}

			int length = Math.Max(this._fac.Length, value._fac.Length);

			bool thisnegative = this.IsNegative;
			List<byte> thisval = new List<byte>(this._fac);

			bool thatnegative = value.IsNegative;
			List<byte> thatval = new List<byte>(value._fac);

			List<byte> newfac = new List<byte>(new byte[length]);
			byte carry = 0;
			for (int i = length - 1; i >= 0; i--)
			{
				byte s = (byte)((((thisnegative ? -1 : 1) * thisval[i]) + ((thatnegative ? -1 : 1) * thatval[i])) + carry);
				carry = (byte)(s >= 10 ? 1 : 0);
				newfac[i] = (byte)(s >= 10 ? s - 10 : s);
			}

			length = Math.Max(this._digits.Length, value._digits.Length);
			thisval = new List<byte>(length);
			thisval.InsertRange(length - this._digits.Length, this._digits);
			thatval = new List<byte>(length);
			thatval.InsertRange(length - value._digits.Length, value._digits);
			List<byte> newdigit = new List<byte>(new byte[length]);
			for (int i = length - 1; i >= 0; i--)
			{
				int s = (byte)(((thisnegative ? -1 : 1) * thisval[i]) + ((thatnegative ? -1 : 1) * thatval[i]) + carry);
				carry = (byte)(s >= 10 ? 1 : 0);
				newdigit[i] = (byte)(s >= 10 ? s - 10 : s);
			}

			if (carry > 0)
			{
				newdigit.Insert(0, carry);
			}

			return BigNumber.Convert(newdigit, newfac, true);
		}

		/// <summary>
		/// Subtracts the specified value from the current instance, returning a brand new value.
		/// </summary>
		/// <param name="value">The value to add.</param>
		/// <returns>A new value containining the difference.</returns>
		public BigNumber Subtract(BigNumber value)
		{
			if (this.IsZero && value.IsZero)
			{
				return BigNumber.Zero;
			}

			int length = Math.Max(this._fac.Length, value._fac.Length);
			bool negative = this.IsNegative && value.IsNegative;
			byte carry = 0;
			List<byte> thisval = new List<byte>(this._fac);
			List<byte> thatval = new List<byte>(value._fac);
			List<byte> newfac = new List<byte>(new byte[length]);
			for (int i = 0; i < length; i++)
			{
				byte s = (byte)(thisval[i] + thatval[i] + carry);
				carry = (byte)(s >= 10 ? 1 : 0);
				newfac[i] = (byte)(s >= 10 ? s - 10 : s);
			}

			length = Math.Max(this._digits.Length, value._digits.Length);
			thisval = new List<byte>(length);
			thisval.InsertRange(length - this._digits.Length, this._digits);
			thatval = new List<byte>(length);
			thatval.InsertRange(length - value._digits.Length, value._digits);
			List<byte> newdigit = new List<byte>(new byte[length]);
			for (int i = length - 1; i >= 0; i--)
			{
				int s = (byte)(thisval[i] + thisval[i]) + carry;
				carry = (byte)(s >= 10 ? 1 : 0);
				newdigit[i] = (byte)(s >= 10 ? s - 10 : s);
			}

			if (carry > 0)
			{
				newdigit.Insert(0, carry);
			}

			return BigNumber.Convert(newdigit, newfac, true);
		}

		private static int Compare(byte[] array1, byte[] array2, bool n, bool digits)
		{
			// Assumes 
			// - being invoked by Compare(BigNumber, BigNumber), which checks that both arrays are either going to both be positive or both be negative, thus the single negative argument
			// - Compare(BigNumber, BigNumber) has already checked for Zero
			// - an empty array is exactly the same thing as [0];
			int length1 = array1.Length == 0 ? 1 : array1.Length;
			int length2 = array2.Length == 0 ? 1 : array2.Length;
			int flip = digits ? (n ? -1 : 1) : 1; // -9 > -11 but -9.11 < -9.9;
			int result = length1.CompareTo(length2) * flip;
			if (digits && result == 0)
			{
				for (int i = 0; i < array1.Length; i++)
				{
					int a1v = array1[i] * (n ? -1 : 1);
					int a2v = array2[i] * (n ? -1 : 1);
					result = a1v.CompareTo(a2v);
					if (result != 0)
					{
						break;
					}
				}
			}
			else
			{
				int max = Math.Max(array1.Length, array2.Length);
				for (int i = 0; i < max; i++)
				{
					int a1v = i <= array1.Length - 1 ? array1[i] : 0 * (n ? -1 : 1);
					int a2v = i <= array2.Length - 1 ? array2[i] : 0 * (n ? -1 : 1);
					result = a1v.CompareTo(a2v);
					if (result != 0)
					{
						break;
					}
				}
			}

			return result;
		}

		private static int Compare(BigNumber value1, BigNumber value2)
		{
			if (value1.IsZero && value1.IsZero)
			{
				return 0;
			}
			else if (value1.IsZero)
			{
				return value2.IsNegative ? -1 : 1; // 0 > -1
			}
			else if (value2.IsZero)
			{
				return value1.IsNegative ? 1 : -1; // -1 < 0;
			}
			else if (value1.IsNegative && !value2.IsNegative)
			{
				return -1;
			}
			else if (!value1.IsNegative && value2.IsNegative)
			{
				return 1;
			}

			Diagnostics.Debug.Assert(value1._negative == value2._negative, "They will both either be positive or both negative.");
			int compare = BigNumber.Compare(value1._digits, value2._digits, value1._negative, true);
			if (compare == 0)
			{
				compare = BigNumber.Compare(value1._fac, value2._fac, value1._negative, false);
			}
			
			return compare;
		}

		private static BigNumber Convert(List<byte> v, List<byte> f, bool n)
		{
			return BigNumber.Convert(Trim(v, false), Trim(f, true), n);
		}

		private static BigNumber Convert(byte[] v, byte[] f, bool neg)
		{
			if (v.Length == 0 && f.Length == 0)
			{
				return BigNumber.Zero;
			}
			else
			{
				return new BigNumber(v.Length == 0 ? new byte[] { 0 } : v, f, neg);
			}
		}

		private static byte[] Trim(List<byte> list, bool trimEnd)
		{
			list.TrimExcess();
			if (list.Count > 0)
			{
				if (trimEnd)
				{
					int count = list.Count - 1;
					while (list[count] == 0)
					{
						list.RemoveAt(count--);
						if (list.Count == 0 || list[count] > 0)
						{
							break;
						}
					}
				}
				else
				{
					while (list[0] == 0)
					{
						list.RemoveAt(0);
						if (list.Count == 0 || list[0] > 0)
						{
							break;
						}
					}
				}
			}

			return list.ToArray();
		}

		private bool GreaterThan(BigNumber value)
		{
			return BigNumber.Compare(this, value) == 1;
		}

		private bool LessThan(BigNumber value)
		{
			return BigNumber.Compare(this, value) == -1;
		}

		#endregion
	}
}