using System;
using System.Text;

namespace System.IO
{
	/// <summary>
	/// Bunch of extensions on the System.IO namespace
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Converts specified stream to a byte array.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>A byte array, or null if stream is null.</returns>
		public static byte[] ToBytes(this Stream stream)
		{
			if (stream == null)
			{
				return null;
			}

			return FileUtil.ReadStream(stream);
		}

		/// <summary>
		/// Converts specified string to a byte array using <see cref="System.Text.Encoding.Default"/>.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>A byte array, or null if string is null.</returns>
		public static byte[] ToBytes(this string value)
		{
			if (value == null)
			{
				return null;
			}

			return Encoding.Default.GetBytes(value);
		}
	}
}