using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
	/// <summary>
	/// Bunch of extensions
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Capitalizes the specified string.
		/// </summary>
		/// <example>Phrase "SOME WORDS" returns "Some Words"</example>
		/// <example>Phrase "some words" returns "Some Words", etc.</example>
		/// <param name="value">The value to capitalize.</param>
		/// <returns>Null or whitespace if value is null or whitespace, the capitalized version otherwise.</returns>
		[SuppressMessage("Microsoft.Globalization", "CA13084", Justification = "That would defeat the purpose of capitalizing.")]
		public static string Capitalize(this string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return value;
			}

			#pragma warning disable CA1308
			char[] result = value.ToLowerInvariant().ToCharArray();
			#pragma warning restore CA1308
			int index = 0;
			while (index < value.Length)
			{
				var c = value[index];
				var prev = index == 0 ? ' ' : value.ToCharArray()[index - 1];
				if (!Char.IsLetter(prev))
				{
					result[index] = Char.ToUpperInvariant(c);
				}

				index++;
			}

			return new string(result);
		}

		/// <summary>
		/// Gets if the specified <paramref name="value"/> is in the specified <paramref name="source"/> using specified <see cref="StringComparison"/> object.
		/// </summary>
		/// <param name="source">The string to look into.</param>
		/// <param name="value">The string to seek.</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
		/// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> is null OR <paramref name="value"/> is null.</exception>
		public static bool Contains(this string source, string value, StringComparison comparisonType)
		{
			Arg.IsNotNull(source, "source");
			Arg.IsNotNull(value, "value");

			return source.IndexOf(value, comparisonType) > -1;
		}

		/// <summary>
		/// Gets the quarter based on the month.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>Returns the quarter the month belongs to.</returns>
		public static short GetQuarter(this DateTime date)
		{
			var month = date.Month;
			short quarter = 1;
			if (month > 3 && month <= 6)
			{
				quarter = 2;
			}
			else if (month > 6 && month <= 9)
			{
				quarter = 3;
			}
			else if (month > 9)
			{
				quarter = 4;
			}

			return quarter;
		}

		/// <summary>
		/// Gets if the specified type has the specified attribute (by calling <see cref="HasAttribute(Type, Type, bool)"/>, where inherit is false).
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="attributeType">The attribute type.</param>
		/// <returns>True if the type has the specified attribute, false otherwise.</returns>
		public static bool HasAttribute(this Type type, Type attributeType)
		{
			return type.HasAttribute(attributeType, false);
		}

		/// <summary>
		/// Gets if the specified type has the specified attribute (by calling <see cref="System.Reflection.MemberInfo.GetCustomAttributes(Type, bool)"/>).
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="attributeType">The attribute type.</param>
		/// <param name="inherit">If to search this member's inheritance chain to find the attributes.</param>
		/// <returns>True if the type has the specified attribute, false otherwise.</returns>
		public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
		{
			Arg.IsNotNull(type);
			#if NETSTANDARD1_6 || NETSTANDARD1_5 || NETSTANDARD1_3
			return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).Any();
			#else
			return type.GetCustomAttributes(attributeType, inherit).Any();
			#endif
		}

		/// <summary>
		/// Rounds a double-precision floating-point value to a specified number of fractional digits.
		/// </summary>
		/// <remarks>All it does is to call Math.Round(value, digits)</remarks>
		/// <param name="value">A double-precision floating-point number to be rounded.</param>
		/// <param name="digits">The number of fractional digits in the return value.</param>
		/// <returns>The number nearest to value that contains a number of fractional digits equal to digits.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="digits"/> is less than 0 or greater than 15.</exception>
		public static double Round(this double value, int digits)
		{
			return Math.Round(value, digits);
		}

		/// <summary>
		/// Converts the specified value to a hex string, using BitConverter.ToString, but without the dashes.
		/// </summary>
		/// <param name="buffer">The byte array.</param>
		/// <returns>The hex string result.</returns>
		public static string ToHex(this byte[] buffer)
		{
			Arg.IsNotNull(buffer);
			return BitConverter.ToString(buffer).Replace("-", String.Empty);
		}

		/// <summary>
		/// Converts specified integer value to roman numeral.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>Returns 'N', if <paramref name="value"/> was 0 (there is no zero in roman numerals) else the value in roman numeral</returns>
		/// <remarks>
		/// <para>Any numerals greater than 5000 will begin with the the overbar character, i.e, 5000 = ¯V, 10000 = ¯X, etc, etc</para>
		/// <para>Based on http://www.dreamcubes.com/b2/software-development/21/to-roman-c-int-to-roman-converter/ </para>
		/// </remarks>
		public static string ToRoman(this int value)
		{
			if (value == 0)
			{
				return "N";
			}

			var wasNegative = value < 0;
			if (wasNegative)
			{
				value = value * -1;
			}

			int[] nums = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000, 5000, 10000, 50000, 100000, 500000, 1000000 };
			string[] romanNums = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M", "¯V", "¯X", "¯L", "¯C", "¯D", "¯M" };

			string result = String.Empty;
			int n = value;
			for (int i = nums.Length - 1; i >= 0; --i)
			{
				while (n >= nums[i])
				{
					n -= nums[i];
					result += romanNums[i];
				}
			}

			return (wasNegative ? "-" : String.Empty) + result;
		}

		/// <summary>
		/// Subtracts the milliseconds duration from the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="date">The source date.</param>
		/// <returns>A new <see cref="DateTime"/> object without its milliseconds bit</returns>
		public static DateTime Trim(this DateTime date)
		{
			return date.Trim("f");
		}

		/// <summary>
		/// Subtracts the specified bit duration from the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="date">The source date.</param>
		/// <param name="specifier">The date format specifying the bit to be trimmed (i.e. 'ff', 'ss', 'mm').</param>
		/// <returns>A new <see cref="DateTime"/> object without its specified bit.</returns>
		public static DateTime Trim(this DateTime date, string specifier)
		{
			Arg.IsNotEmpty(specifier, "specifier");
			return date.Subtract(date.TimeOfDay.Trim(specifier));
		}

		/// <summary>
		/// Subtracts the milliseconds duration from the specified <see cref="TimeSpan"/> object.
		/// </summary>
		/// <param name="span">The source time.</param>
		/// <returns>A new <see cref="TimeSpan"/> object without its milliseconds bit.</returns>
		public static TimeSpan Trim(this TimeSpan span)
		{
			return span.Subtract(new TimeSpan(0, 0, 0, 0, span.Milliseconds));
		}

		/// <summary>
		/// Subtracts the specified bit duration from the specified <see cref="TimeSpan"/> object.
		/// </summary>
		/// <param name="span">The source date.</param>
		/// <param name="specifier">The date format specifying the bit to be trimmed (i.e. 'ff', 'ss', 'mm').</param>
		/// <returns>A new <see cref="TimeSpan"/> object without its specified bit.</returns>
		[SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Fine as is.")]
		public static TimeSpan Trim(this TimeSpan span, string specifier)
		{
			Arg.IsNotEmpty(specifier, "specifier");

			TimeSpan subtract;
			switch (specifier)
			{
				case "f":
				case "ff":
				case "fff":
					subtract = new TimeSpan(0, 0, 0, 0, span.Milliseconds);
					break;
				case "s":
				case "ss":
					subtract = new TimeSpan(0, 0, 0, span.Seconds, span.Milliseconds);
					break;
				case "m":
				case "mm":
					subtract = new TimeSpan(0, 0, span.Minutes, span.Seconds, span.Milliseconds);
					break;
				case "h":
				case "hh":
					subtract = new TimeSpan(0, span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
					break;
				case "d":
				case "dd":
					subtract = new TimeSpan(span.Days, span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
					break;
				default:
					throw new NotSupportedException();
			}

			return span.Subtract(subtract);
		}
	}
}