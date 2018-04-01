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
			
			MemoryStream memoryStream = stream as MemoryStream;
			if (memoryStream == null)
			{
				using (memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
			else
			{
				return memoryStream.ToArray();
			}
		}
	}
}