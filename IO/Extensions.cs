using System;
using System.IO;

namespace Adenson.IO
{
	/// <summary>
	/// Bunch of extension methods
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Converts specified stream to a byte array
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <returns>byte array, or null if stream is null</returns>
		public static byte[] ToBytes(this Stream stream)
		{
			if (stream == null) return null;
			stream.Seek(0, SeekOrigin.Begin);
			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, (int)stream.Length);
			return buffer;
		}
	}
}