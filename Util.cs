using System;
using System.ComponentModel;

namespace Adenson
{
	/// <summary>
	/// Collection of utility methods
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor.
		/// </summary>
		/// <param name="typeString"></param>
		/// <returns></returns>
		public static object CreateInstance(string typeString)
		{
			if (String.IsNullOrWhiteSpace(typeString)) throw new ArgumentNullException("typeString");
			Type type = Util.GetType(typeString);
			if (type == null) throw new TypeLoadException(String.Format(Exceptions.TypeArgCouldNotBeLoaded, typeString));
			return Activator.CreateInstance(type);
		}
		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor, and casts it to specified generic type parameter
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeString"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeString)
		{
			if (String.IsNullOrWhiteSpace(typeString)) throw new ArgumentNullException("typeString");
			return (T)Util.CreateInstance(typeString);
		}
		/// <summary>
		/// Creates an instance of the specified type using that type's default constructor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return (T)Activator.CreateInstance(type);
		}
		/// <summary>
		/// Converts a hex string to bytes
		/// </summary>
		/// <param name="hexString">The string to convert</param>
		/// <returns>Created byte array</returns>
		/// <remarks>Borrowed from http://www.codeproject.com/KB/recipes/hexencoding.aspx?msg=2494780 </remarks>
		/// <exception cref="NotSupportedException">if the length of the string is not a multiple of zero</exception>
		/// <exception cref="FormatException">value contains a character that is not a valid digit in the 16 base. The exception message indicates that there are no digits to convert if the first character in value is invalid; otherwise, the message indicates that value contains invalid trailing characters</exception>
		/// <exception cref="OverflowException">value, which represents a base 10 unsigned number, is prefixed with a negative  sign.  -or- The return value is less than System.Byte.MinValue or larger than System.Byte.MaxValue.</exception>
		public static byte[] GetBytes(string hexString)
		{
			if (hexString == null) return null;
			if (hexString.Length % 2 != 0) throw new NotSupportedException();
			if (hexString.Length == 0) return new byte[] { };

			byte[] buffer = new byte[hexString.Length / 2];
			for (int i = 0; i < buffer.Length; i++) buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			return buffer;
		}
		/// <summary>
		/// Gets the System.Type with the specified name.
		/// </summary>
		/// <param name="typeString">The assembly-qualified name of the type to get.</param>
		/// <returns>The type with the specified name.</returns>
		public static Type GetType(string typeString)
		{
			if (String.IsNullOrWhiteSpace(typeString)) throw new ArgumentNullException("typeString");

			Type type = Type.GetType(typeString, false, true);
			if (type == null)
			{
				string[] splits = typeString.Split(';');
				if (splits.Length >= 2)
				{
					string assembly = splits[0].Trim();
					string typeName = splits[1].Trim();
					type = Type.GetType(typeName + ", " + assembly, false, true);
				}
			}
			return type;
		}
		/// <summary>
		/// Trys to convert the specified value
		/// </summary>
		/// <param name="value">the value to convert, must be in the form {[Type Full Name], Value}</param>
		/// <param name="result">the output of the result</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert(string value, out object result)
		{
			if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");
			string[] splits = value.Split(',');
			if (splits.Length != 2) throw new ArgumentException(String.Format(Exceptions.TypeExpectedMustBeArg, "[Type Full Name], Value"));
			Type type = Util.GetType(splits[1]);
			if (type == null) throw new TypeLoadException(String.Format(Exceptions.TypeArgCouldNotBeLoaded, splits[0]));
			return Util.TryConvert(splits[0], type, out result);
		}
		/// <summary>
		/// Trys to convert the specified value
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <param name="type">The type of the object we need to convert</param>
		/// <param name="result">the output of the result</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert(object value, Type type, out object result)
		{
			TypeConverter typeConverter = TypeDescriptor.GetConverter(type);

			if ((value == null && typeConverter == null) || !typeConverter.IsValid(value))
			{
				result = null;
				return false;
			}

			if (value != null && type == value.GetType())
			{
				result = value;
				return true;
			}

			result = null;
			if (type == typeof(Boolean)) result = Convert.ToBoolean(value);
			else if (type == typeof(int)) result = Convert.ToInt32(value);
			else
			{
				if (typeConverter != null && value != null && typeConverter.CanConvertFrom(value.GetType())) result = typeConverter.ConvertFrom(value);
			}
			if (result == null) return false;
			return true;
		}
		/// <summary>
		/// Calls GetBytes(hexString) in a try catch
		/// </summary>
		/// <param name="hexString">The string to convert</param>
		/// <param name="result">The result</param>
		/// <returns>false if an exception occurred true, true otherwise</returns>
		public static bool TryGetBytes(string hexString, out byte[] result)
		{
			result = null;
			try
			{
				result = Util.GetBytes(hexString);
				return true;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch
			{
				return false;
			}
		}
	}
}