using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Adenson.Collections;
using Adenson.Cryptography;

namespace System
{
	/// <summary>
	/// Collection of file/directory/io utility methods
	/// </summary>
	public static class FileUtil
	{
		#region Variables
		private readonly static Hashtable<char, char> fileInvalidCharsReplacements = GetFileInvalidCharsReplacements();
		#endregion
		#region Methods

		/// <summary>
		/// Converts a hex string to bytes
		/// </summary>
		/// <param name="hexValue">The string to convert</param>
		/// <returns>Created byte array</returns>
		/// <remarks>Borrowed from http://www.codeproject.com/KB/recipes/hexencoding.aspx?msg=2494780 </remarks>
		/// <exception cref="NotSupportedException">if the length of the string is not a multiple of zero</exception>
		/// <exception cref="FormatException">value contains a character that is not a valid digit in the 16 base. The exception message indicates that there are no digits to convert if the first character in value is invalid; otherwise, the message indicates that value contains invalid trailing characters</exception>
		/// <exception cref="OverflowException">value, which represents a base 10 unsigned number, is prefixed with a negative  sign.  -or- The return value is less than System.Byte.MinValue or larger than System.Byte.MaxValue.</exception>
		public static byte[] GetBytes(string hexValue)
		{
			if (hexValue == null) return null;
			if (hexValue.Length % 2 != 0) throw new NotSupportedException();
			if (hexValue.Length == 0) return new byte[] { };

			byte[] buffer = new byte[hexValue.Length / 2];
			for (int i = 0; i < buffer.Length; i++) buffer[i] = Convert.ToByte(hexValue.Substring(i * 2, 2), 16);
			return buffer;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="buffer"></param>
		/// <param name="overwrite"></param>
		/// <returns></returns>
		public static string CreateFile(string filePath, byte[] buffer, bool overwrite)
		{
			if (StringUtil.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
			if (buffer == null) return null;
			if (buffer.Length == 0) return null;

			if (!File.Exists(filePath) || overwrite)
			{
				var directory = Path.GetDirectoryName(filePath);
				if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
				FileStream stream = File.Create(filePath);
				stream.Write(buffer, 0, buffer.Length);
				stream.Close();
			}
			return filePath;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="buffer"></param>
		/// <param name="overwrite"></param>
		/// <returns></returns>
		public static string CreateMD5HashedFile(string directory, byte[] buffer, bool overwrite)
		{
			if (StringUtil.IsNullOrWhiteSpace(directory)) throw new ArgumentNullException("directory");
			if (buffer == null) return null;
			if (buffer.Length == 0) return null;

			string filePath = Path.Combine(directory, StringUtil.ToString(Encryptor.GetHash(buffer, HashAlgorithmType.MD5)));
			return FileUtil.CreateFile(filePath, buffer, overwrite);
		}
		/// <summary>
		/// Returns the names of files in the specified directory that match the specified patterns, see <see cref="System.IO.Directory.GetFiles(string, string, System.IO.SearchOption)"/>
		/// </summary>
		/// <param name="directory">The directory to search.</param>
		/// <param name="extensions">The list of extensions to match against the names of files in directory</param>
		/// <returns> A String array containing the names of files in the specified directory that match the specified search pattern. File names include the full path.</returns>
		/// <exception cref="ArgumentNullException">if directory is null or empty or is just white space</exception>
		public static string[] GetFiles(string directory, IEnumerable<string> extensions)
		{
			if (StringUtil.IsNullOrWhiteSpace(directory)) throw new ArgumentNullException("directory");
			List<string> filesToProcess = new List<string>();
			foreach (string ext in extensions)
			{
				filesToProcess.AddRange(Directory.GetFiles(directory, ext.Trim(), SearchOption.AllDirectories).ToList());
			}
			return filesToProcess.ToArray();
		}
		/// <summary>
		/// Gets if the specified path is a file or a directory, operation not guaranteed
		/// </summary>
		/// <param name="fullPath">The path</param>
		/// <returns>true if directory, false other wise</returns>
		/// <exception cref="ArgumentNullException">if fullPath is null or empty or is just white space</exception>
		public static bool GetIsDirectory(string fullPath)
		{
			if (StringUtil.IsNullOrWhiteSpace(fullPath)) throw new ArgumentNullException("fullPath");
			return Path.GetFileName(fullPath) == Path.GetFileNameWithoutExtension(fullPath);//Prob a directory
		}
		/// <summary>
		/// Fixes path by removing characters that are invalid
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string FixFilePath(string path)
		{
			if (StringUtil.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");

			var invalidCharacters = FileUtil.fileInvalidCharsReplacements.Keys.ToArray();
			IEnumerable<char> intersect = path.ToCharArray().Intersect(invalidCharacters);
			if (intersect.Count() > 0)
			{
				string result = path;
				foreach (char c in intersect)
				{
					char replaceChar = fileInvalidCharsReplacements[c];
					result = result.Replace(c.ToString(), replaceChar == Char.MinValue ? "_" : replaceChar.ToString());
				}
				return result;
			}
			return path;
		}
		/// <summary>
		/// Creates a byte array, by reading the response stream of the specified url 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>byte array, or null if stream is null</returns>
		/// <exception cref="ArgumentNullException">if filePath is null or empty or is just white space</exception>
		/// <exception cref="FileNotFoundException">if the specified filePath does not exist</exception>
		public static byte[] ReadStream(string filePath)
		{
			if (StringUtil.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
			if (!File.Exists(filePath)) throw new System.IO.FileNotFoundException("File specified does not exist.", filePath);
			return FileUtil.ReadStream(new Uri(filePath, UriKind.Absolute));
		}
		/// <summary>
		/// Creates a byte array, by reading the response stream of the specified url
		/// </summary>
		/// <param name="url">The url</param>
		/// <returns>byte array, or null if resulting stream is null</returns>
		/// <exception cref="ArgumentNullException">if url is null</exception>
		public static byte[] ReadStream(Uri url)
		{
			if (url == null) throw new ArgumentNullException("url");

			Stream stream;
			if (File.Exists(url.AbsolutePath)) stream = File.Open(url.AbsolutePath, FileMode.Open);
			else
			{
				System.Net.WebRequest request = System.Net.WebRequest.Create(url);
				stream = request.GetResponse().GetResponseStream();
			}
			return FileUtil.ReadStream(stream);
		}
		/// <summary>
		/// Creates a byte array, by reading the specified stream
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <returns>byte array, or null if resulting stream is null</returns>
		/// <exception cref="ArgumentNullException">if stream is null</exception>
		/// <exception cref="NotSupportedException">if stream does not support reading</exception>
		public static byte[] ReadStream(Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");

			MemoryStream ms = stream as MemoryStream;
			bool ours = false;
			if (ms == null)
			{
				ms = new MemoryStream();
				ours = true;
				stream.Seek(0, SeekOrigin.Begin);
				byte[] buffer = new byte[1024];
				int len;
				while ((len = stream.Read(buffer, 0, 1024)) > 0) ms.Write(buffer, 0, len);
				stream.Close();
			}
			var result = ms.ToArray();
			if (ours) ms.Dispose();
			return result;
		}
		/// <summary>
		/// Calls GetBytes(hexString) in a try catch
		/// </summary>
		/// <param name="hexValue">The string to convert</param>
		/// <param name="result">The result</param>
		/// <returns>false if an exception occurred true, true otherwise</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public static bool TryGetBytes(string hexValue, out byte[] result)
		{
			result = null;
			try
			{
				result = FileUtil.GetBytes(hexValue);
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

		#endregion
		#region Helper Methods

		private static Hashtable<char, char> GetFileInvalidCharsReplacements()
		{
			var result = new Hashtable<char, char>();
			foreach (var c in Path.GetInvalidPathChars()) result.Add(c, ' ');
			result[':'] = '-';
			result['/'] = ',';
			return result;
		}

		#endregion
	}
}