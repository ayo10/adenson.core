using System;

namespace Adenson
{
	/// <summary>
	/// Collection of string utility methods
	/// </summary>
	public static class StringUtil
	{
		#region Methods

		/// <summary>
		/// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
		/// </summary>
		/// <param name="value">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		/// <exception cref="ArgumentNullException">format or args is null.</exception>
		public static string Format(string value, params object[] args)
		{
			if (StringUtil.IsNullOrWhiteSpace(value)) return String.Empty;

			try
			{
				return String.Format(value, args);
			}
			catch (FormatException)
			{
				var str = value;
				for (int i = 0; i < args.Length; i++) str = str.Replace("{" + i + "}", (args[i] == null ? "null" : StringUtil.ToString(args[i])));
				return str;
			}
		}

		/// <summary>
		/// Uses the specified object to try and format the specified string
		/// </summary>
		/// <param name="value">A composite format string.</param>
		/// <param name="arg">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding arg object.</returns>
		/// <exception cref="ArgumentNullException">format or args is null.</exception>
		public static string Format(string value, object arg)
		{
			if (StringUtil.IsNullOrWhiteSpace(value)) return String.Empty;
			if (arg == null) throw new ArgumentNullException("arg");

			return StringUtil.Format(value, arg);
		}
		
		/// <summary>
		/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		/// <remarks>For .NET 4, simply calls String.IsNullOrWhiteSpace</remarks>
		/// <param name="value">The string to test.</param>
		/// <returns>true if the value parameter is null or System.String.Empty, or if value consists exclusively of white-space characters.</returns>
		public static bool IsNullOrWhiteSpace(string value)
		{
			#if NET35
			return String.IsNullOrEmpty(value == null ? null : value.Trim());
			#else
			return String.IsNullOrWhiteSpace(value);
			#endif
		}
		
		/// <summary>
		/// Converts the specified byte array to its equivalent string representation in Base64, minus the slashes and equal signs
		/// </summary>
		/// <param name="buffer">The byte array to convert.</param>
		/// <returns>The string representation of the specified byte array, or null if its null.</returns>
		public static string ToString(byte[] buffer)
		{
			if (buffer == null) return null;
			var base64 = System.Convert.ToBase64String(buffer);
			return base64.Replace("\\", String.Empty).Replace("/", String.Empty).Replace("=", String.Empty);
		}
		
		/// <summary>
		/// Converts the value of the specified object to its equivalent string representation.
		/// </summary>
		/// <remarks>calls Convert.ToString(value)</remarks>
		/// <param name="value">An object that supplies the value to convert, or null.</param>
		/// <returns>The string representation of value, or System.String.Empty if value is null.</returns>
		public static string ToString(object value)
		{
			if (value == null) return null;
			
			byte[] arr = value as byte[];
			if (arr != null) return StringUtil.ToString(arr);

			Exception ex = value as Exception;
			if (ex != null) return Log.Logger.ConvertToString(ex, true);

			return Convert.ToString(value);
		}

		#endregion
	}
}