using System;
using System.Globalization;

namespace Adenson
{
	/// <summary>
	/// Collection of string utility methods (mainly to specify IFormatProvider via CultureInfo)
	/// </summary>
	/// <remarks>FxCop Happy!!!</remarks>
	public static class StringUtil
	{
		#region Methods

		/// <summary>
		/// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
		/// </summary>
		/// <remarks>Calls String.Format(<see cref="CultureInfo.CurrentCulture"/>, format, args)</remarks>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		public static string Format(string format, params object[] args)
		{
			return String.Format(CultureInfo.CurrentCulture, format, args);
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
		/// Converts the value of the specified object to its equivalent string representation using <see cref="CultureInfo.InvariantCulture"/>.
		/// </summary>
		/// <remarks>calls Convert.ToString(value, CultureInfo.InvariantCulture)</remarks>
		/// <param name="value">An object that supplies the value to convert, or null.</param>
		/// <returns>The string representation of value, or System.String.Empty if value is null.</returns>
		public static string ToString(object value)
		{
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		#endregion
	}
}
