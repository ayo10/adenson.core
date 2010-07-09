using System;

namespace Adenson
{
	/// <summary>
	/// Collection of utility methods
	/// </summary>
	public static class Util
	{
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