using System;
using System.ComponentModel;

namespace Adenson
{
	/// <summary>
	/// Converts a base data type to another base data type, the difference being it treats DBNull.Value as null
	/// </summary>
	/// <remarks>All methods call ToType(value)</remarks>
	public static class Convert
	{
		/// <summary>
		/// Converts the value of a specified System.Object to an equivalent Boolean value
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>
		///	false if value equals null or DBNull.Value.-or- true or false; the result of invoking the 
		///	IConvertible.ToBoolean method for the underlying type of value.
		///	</returns>
		public static bool ToBoolean(object value)
		{
			if (value == DBNull.Value) return false;
			if (value is string)
			{
				if (object.Equals(value, "1")) return true;
				if (object.Equals(value, "0")) return false;
			}
			return global::System.Convert.ToBoolean(value);
		}
		/// <summary>
		/// Converts the value of a specified System.Object to an equivalent System.Byte value
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>An 8-bit unsigned integer equivalent to the value of value, or byte.MinValue if value is null.</returns>
		public static byte ToByte(object value)
		{
			if (value == DBNull.Value) return default(byte);
			return global::System.Convert.ToByte(value);
		}
		/// <summary>
		/// Converts the value of a specified System.Object to an equivalent System.Byte value
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>The Unicode character equivalent to the value of value.-or- System.Char.MinValue if value equals null.</returns>
		public static char ToChar(object value)
		{
			if (value == DBNull.Value) return default(char);
			return global::System.Convert.ToChar(value);
		}
		/// <summary>
		/// Converts the value of the specified object to a 16-bit signed integer.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>A 16-bit signed integer equivalent to the value of value, or zero if value is null.</returns>
		public static short ToInt16(object value)
		{
			if (value == DBNull.Value) return default(short);
			return global::System.Convert.ToInt16(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to a 32-bit signed integer.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>A 32-bit signed integer equivalent to the value of value, or zero if value is null.</returns>
		public static int ToInt32(object value)
		{
			if (value == DBNull.Value) return default(byte);
			return global::System.Convert.ToInt32(value);
		}
		/// <summary>
		/// Converts the value of the specified object to a 64-bit signed integer.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>A 64-bit signed integer equivalent to the value of value, or zero if value is null.</returns>
		public static long ToInt64(object value)
		{
			if (value == DBNull.Value) return default(long);
			return global::System.Convert.ToInt64(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to a System.Decimal number.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface or null.</param>
		/// <returns>A System.Decimal number equivalent to the value of value, or zero if value is null.</returns>
		public static decimal ToDecimal(object value)
		{
			if (value == DBNull.Value) return default(decimal);
			return global::System.Convert.ToDecimal(value);
		}
		/// <summary>
		/// Converts the value of the specified object to a double-precision floating point number.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>A double-precision floating point number equivalent to the value of value, or zero if value is null.</returns>
		public static double ToDouble(object value)
		{
			if (value == DBNull.Value) return default(double);
			return global::System.Convert.ToDouble(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to its System.String representation.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>The System.String equivalent of the value of value.</returns>
		public static string ToString(object value)
		{
			if (value == DBNull.Value) return default(string);
			return global::System.Convert.ToString(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to a System.Single number.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface or null.</param>
		/// <returns>A System.Single number equivalent to the value of value, or zero if value is null.</returns>
		public static float ToSingle(object value)
		{
			if (value == DBNull.Value) return default(float);
			return global::System.Convert.ToSingle(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to a 16-bit unsigned integer.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface or null.</param>
		/// <returns>A 16-bit unsigned integer equivalent to the value of value, or zero if value is null.</returns>
		/// <exception cref="InvalidCastException">value does not implement System.IConvertible.</exception>
		public static ushort ToUInt16(object value)
		{
			if (value == DBNull.Value) return default(ushort);
			return global::System.Convert.ToUInt16(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to a 32-bit unsigned integer.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface or null.</param>
		/// <returns>A 32-bit unsigned integer equivalent to the value of value, or zero if value is null.</returns>
		public static uint ToUInt32(object value)
		{
			if (value == DBNull.Value) return default(ushort);
			return global::System.Convert.ToUInt32(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to a 64-bit unsigned integer.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface or null.</param>
		/// <returns>A 64-bit unsigned integer equivalent to the value of value, or zero if value is null.</returns>
		public static ulong ToUInt64(object value)
		{
			if (value == DBNull.Value) return default(ulong);
			return global::System.Convert.ToUInt64(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to its System.Byte Array representation.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>null if value is null or DBNull.Value, otherwise.</returns>
		public static byte[] ToByteArray(object value)
		{
			if (value == DBNull.Value) return default(byte[]);
			if (value == null) return null;
			byte[] cv = value as byte[];
			if (cv != null) return cv;
			throw new InvalidCastException(String.Format(ExceptionMessages.CastToTypeException, "byte array"));
		}
		/// <summary>
		/// Converts the value of the specified System.Object to its System.DateTime representation.
		/// </summary>
		/// <param name="value">An object that implements the System.IConvertible interface or null.</param>
		/// <returns>A System.DateTime equivalent to the value of value.-or- A System.DateTime equivalent to System.DateTime.MinValue if value is null.</returns>
		public static DateTime ToDateTime(object value)
		{
			if (value == DBNull.Value) return default(DateTime);
			return global::System.Convert.ToDateTime(value);
		}
		/// <summary>
		/// Converts the value of the specified System.Object to an 8-bit signed integer.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface or null.</param>
		/// <returns>An 8-bit signed integer equivalent to the value of value, or zero if value is null.</returns>
		public static sbyte ToSByte(object value)
		{
			if (value == DBNull.Value) return default(sbyte);
			return global::System.Convert.ToSByte(value);
		}
		/// <summary>
		/// Converts specified object to specified type, the value must implement IConvertible, or have a type converter
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The result of the conversion</returns>
		public static T ChangeType<T>(object value)
		{
			return value == DBNull.Value ? default(T) : (T)Convert.ChangeType(value, typeof(T));
		}
		/// <summary>
		/// Converts a subset of an array of 8-bit unsigned integers to its equivalent 
		/// System.String representation encoded with base 64 digits. Parameters specify 
		/// the subset as an offset in the input array, and the number of elements in 
		/// the array to convert.
		/// </summary>
		/// <param name="inArray">An array of 8-bit unsigned integers.</param>
		/// <param name="offset">An offset in inArray.</param>
		/// <param name="length">The number of elements of inArray to convert.</param>
		/// <returns></returns>
		public static string ToBase64String(byte[] inArray, int offset, int length)
		{
			return global::System.Convert.ToBase64String(inArray, offset, length);
		}
		/// <summary>
		/// Converts the specified System.String, which encodes binary data as base 64
		/// digits, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="s">A System.String.</param>
		/// <returns>An array of 8-bit unsigned integers equivalent to s.</returns>
		public static byte[] FromBase64String(string s)
		{
			return global::System.Convert.FromBase64String(s);
		}
		/// <summary>
		/// Returns an System.Object with the specified System.Type and whose value is
		/// equivalent to the specified object.
		/// </summary>
		/// <param name="value">An System.Object that implements the System.IConvertible interface.</param>
		/// <param name="destinationType">A System.Type.</param>
		/// <returns>An object whose System.Type is conversionType and whose value is equivalent to value.-or-null, if value is null and conversionType is not a value type.</returns>
		public static object ChangeType(object value, Type destinationType)
		{
			if (value == DBNull.Value) return null;
			if (value == null) return null;
			if (value.GetType() == destinationType) return value;

			IFormatProvider provider = System.Threading.Thread.CurrentThread.CurrentUICulture;
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				if (destinationType.IsPrimitive)
				{
					if (destinationType == typeof(bool)) return Convert.ToBoolean(value);
					return convertible.ToType(destinationType, provider);
				}

				try { return convertible.ToType(destinationType, provider); }
				catch { }
			}

			object result;
			if (TryConvert(value, TypeDescriptor.GetConverter(destinationType), destinationType, out result)) return result;
			if (TryConvert(value, TypeDescriptor.GetConverter(value), destinationType, out result)) return result;
			
			throw new NotSupportedException(string.Format(ExceptionMessages.ChangeTypeConfused, value, destinationType));
		}

		private static bool TryConvert(object value, TypeConverter converter, Type destinationType, out object result)
		{
			if (converter != null && converter.CanConvertTo(value.GetType()))
			{
				result = converter.ConvertTo(value, destinationType);
				return true;
			}
			result = null;
			return false;
		}
	}
}