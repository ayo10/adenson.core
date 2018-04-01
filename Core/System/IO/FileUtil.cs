#if !NETSTANDARD1_0
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace System.IO
{
	/// <summary>
	/// Collection of file/directory/io utility methods
	/// </summary>
	public static class FileUtil
	{
		#region Variables
		private static Dictionary<char, char> fileInvalidCharsReplacements = GetFileInvalidCharsReplacements();
		#endregion
		#region Methods

		/// <summary>
		/// Creates a new (or overwrites) a file at the specified path using the specified buffer.
		/// </summary>
		/// <param name="filePath">The path and name of the file to create.</param>
		/// <param name="buffer">The buffer containing data to write to the stream.</param>
		/// <returns>Newly created path.</returns>
		public static string CreateFile(string filePath, byte[] buffer)
		{
			Arg.IsNotEmpty(filePath, "filePath");

			string directory = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if (buffer != null)
			{
				using (FileStream stream = File.Create(filePath))
				{
					stream.Write(buffer, 0, buffer.Length);
				}
			}

			return filePath;
		}

		/// <summary>
		/// Converts a hex string to bytes.
		/// </summary>
		/// <param name="hexValue">The string to convert.</param>
		/// <returns>Created byte array</returns>
		/// <remarks>Borrowed from http://www.codeproject.com/KB/recipes/hexencoding.aspx?msg=2494780 </remarks>
		/// <exception cref="NotSupportedException">If the length of the string is not a multiple of zero</exception>
		/// <exception cref="FormatException"><paramref name="hexValue"/> contains a character that is not a valid digit in the 16 base. The exception message indicates that there are no digits to convert if the first character in value is invalid; otherwise, the message indicates that value contains invalid trailing characters</exception>
		/// <exception cref="OverflowException"><paramref name="hexValue"/>, which represents a base 10 unsigned number, is prefixed with a negative  sign.  -or- The return value is less than System.Byte.MinValue or larger than System.Byte.MaxValue.</exception>
		public static byte[] GetBytes(string hexValue)
		{
			if (hexValue == null)
			{
				return null;
			}

			if (hexValue.Length % 2 != 0)
			{
				throw new NotSupportedException();
			}

			if (hexValue.Length == 0)
			{
				return new byte[] { };
			}

			byte[] buffer = new byte[hexValue.Length / 2];
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = Convert.ToByte(hexValue.Substring(i * 2, 2), 16);
			}

			return buffer;
		}

		/// <summary>
		/// Returns the names of files in the specified directory that match the specified patterns, see <see cref="System.IO.Directory.GetFiles(string, string, System.IO.SearchOption)"/>.
		/// </summary>
		/// <param name="directory">The directory to search.</param>
		/// <param name="extensions">The list of extensions to match against the names of files in directory.</param>
		/// <returns> A String array containing the names of files in the specified directory that match the specified search pattern. File names include the full path.</returns>
		/// <exception cref="ArgumentNullException">If directory is null or empty or is just white space</exception>
		public static string[] GetFiles(string directory, IEnumerable<string> extensions)
		{
			Arg.IsNotEmpty(directory);
			Arg.IsNotNull(extensions);
			List<string> filesToProcess = new List<string>();
			foreach (string ext in extensions)
			{
				filesToProcess.AddRange(Directory.GetFiles(directory, ext.Trim(), SearchOption.AllDirectories).ToList());
			}

			return filesToProcess.ToArray();
		}

		/// <summary>
		/// Gets if the specified path is a file or a directory, operation not guaranteed.
		/// </summary>
		/// <param name="fullPath">The path.</param>
		/// <returns>true if directory, false other wise</returns>
		/// <exception cref="ArgumentNullException">If fullPath is null or empty or is just white space</exception>
		public static bool GetIsDirectory(string fullPath)
		{
			Arg.IsNotEmpty(fullPath);
			return Path.GetFileName(fullPath) == Path.GetFileNameWithoutExtension(fullPath); // Prob a directory
		}

		/// <summary>
		/// Fixes the specified name by removing characters that are invalid.
		/// </summary>
		/// <param name="fileName">The name to clean up.</param>
		/// <returns>Cleaned up name.</returns>
		public static string FixFileName(string fileName)
		{
			Arg.IsNotEmpty(fileName);

			var invalidCharacters = FileUtil.fileInvalidCharsReplacements.Keys.ToArray();
			IEnumerable<char> intersect = fileName.ToCharArray().Intersect(invalidCharacters);
			if (intersect.Count() > 0)
			{
				string result = fileName;
				foreach (char c in intersect)
				{
					char replaceChar = fileInvalidCharsReplacements[c];
					result = result.Replace(c.ToString(), replaceChar == char.MinValue ? "_" : replaceChar.ToString());
				}

				return result;
			}

			return fileName;
		}

#if !NETSTANDARD1_6 && !NETSTANDARD1_3

		/// <summary>
		/// Creates a byte array, by reading the response stream of the specified url.
		/// </summary>
		/// <param name="url">The url.</param>
		/// <returns>A byte array, or null if resulting stream is null</returns>
		/// <exception cref="ArgumentNullException">If url is null</exception>
		public static Stream ReadStream(Uri url)
		{
			Arg.IsNotNull(url, nameof(url));
			if (File.Exists(url.AbsolutePath))
			{
				return File.Open(url.AbsolutePath, FileMode.Open);
			}
			else
			{
				System.Net.WebRequest request = System.Net.WebRequest.Create(url);
				return request.GetResponse().GetResponseStream();
			}
		}
#endif
#if !NET40

		/// <summary>
		/// Creates a byte array, by reading the response stream of the specified url.
		/// </summary>
		/// <param name="url">The url.</param>
		/// <returns>A byte array, or null if resulting stream is null</returns>
		/// <exception cref="ArgumentNullException">If url is null</exception>
		public static async Task<Stream> ReadStreamAsync(Uri url)
		{
			Arg.IsNotNull(url);
			
			if (File.Exists(url.AbsolutePath))
			{
				return File.Open(url.AbsolutePath, FileMode.Open);
			}
			else
			{
				WebRequest request = WebRequest.Create(url);
				WebResponse response = await request.GetResponseAsync();
				return response.GetResponseStream();
			}
		}

#endif

		/// <summary>
		/// Calls GetBytes(hexString) in a try catch.
		/// </summary>
		/// <param name="hexValue">The string to convert.</param>
		/// <param name="result">The result.</param>
		/// <returns>false if an exception occurred true, true otherwise</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Its a try method, it should succeed or fail.")]
		public static bool TryGetBytes(string hexValue, out byte[] result)
		{
			result = null;
			try
			{
				result = FileUtil.GetBytes(hexValue);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion
		#region Helper Methods

		private static Dictionary<char, char> GetFileInvalidCharsReplacements()
		{
			var result = new Dictionary<char, char>();
			foreach (var c in Path.GetInvalidPathChars())
			{
				result.Add(c, ' ');
			}

			result[':'] = '-';
			result['/'] = ',';
			return result;
		}

		#endregion
	}
}
#endif