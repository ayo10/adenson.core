using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adenson.Cryptography;
using Adenson.Log;

namespace Adenson.IO
{
	/// <summary>
	/// Collection of File System utility methods
	/// </summary>
	public static class Util
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(Util));
		private readonly static Dictionary<char, char> fileInvalidCharsReplacements = GetFileInvalidCharsReplacements();
		#endregion
		#region Methods

		/// <summary>
		/// Creates all directories and subdirectories as specified by path.
		/// </summary>
		/// <param name="path">The directory path to create.</param>
		/// <returns>A System.IO.DirectoryInfo as specified by path.</returns>
		/// <exception cref="ArgumentNullException">path is null.</exception>
		/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains one or more invalid characters as defined by System.IO.Path.InvalidPathChars.-or-path is prefixed with, or contains only a colon character (:).</exception>
		/// <exception cref="System.IO.IOException">The directory specified by path is read-only.</exception>
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
		public static DirectoryInfo CreateDirectory(string path)
		{
			try
			{
				return Directory.CreateDirectory(path);
			}
			catch
			{
				try
				{
					_mkdir(path);
					return new DirectoryInfo(path);
				}
				catch (Exception ex)
				{
					throw new IOException("The directory cannot be created", ex);
				}
			}
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
			if (Adenson.Util.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
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
			if (Adenson.Util.IsNullOrWhiteSpace(directory)) throw new ArgumentNullException("directory");
			if (buffer == null) return null;
			if (buffer.Length == 0) return null;

			string filePath = Path.Combine(directory, Encryptor.GetMD5Hash(buffer));
			return Util.CreateFile(filePath, buffer, overwrite);
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
			if (Adenson.Util.IsNullOrWhiteSpace(directory)) throw new ArgumentNullException("directory");
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
			if (Adenson.Util.IsNullOrWhiteSpace(fullPath)) throw new ArgumentNullException("fullPath");
			return Path.GetFileName(fullPath) == Path.GetFileNameWithoutExtension(fullPath);//Prob a directory
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string FixFilePath(string path)
		{
			return Util.FixFilePath(path, null);
		}
		/// <summary>
		/// Fixes path by removing characters that are invalid
		/// </summary>
		/// <param name="path"></param>
		/// <param name="invalidFileNameChars"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">if path is null or empty or is just white space</exception>
		public static string FixFilePath(string path, char[] invalidFileNameChars)
		{
			if (Adenson.Util.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");

			if (invalidFileNameChars == null) invalidFileNameChars = Util.fileInvalidCharsReplacements.Keys.ToArray();
			else invalidFileNameChars = invalidFileNameChars.Union(Util.fileInvalidCharsReplacements.Keys).ToArray();
			IEnumerable<char> intersect = path.ToCharArray().Intersect(invalidFileNameChars);
			if (intersect.Count() > 0)
			{
				string result = path;
				foreach (char c in intersect)
				{
					char r = Util.fileInvalidCharsReplacements[c];
					result = result.Replace(c.ToString(), r == char.MinValue ? "" : r.ToString());
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
			if (Adenson.Util.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");
			if (!File.Exists(filePath)) throw new System.IO.FileNotFoundException("File specified does not exist.", filePath);
			return Util.ReadStream(File.Open(filePath, FileMode.Open)); 
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
			try
			{
				System.Net.WebRequest request = System.Net.WebRequest.Create(url);
				Stream stream = request.GetResponse().GetResponseStream();
				MemoryStream ms = new MemoryStream();
				byte[] buffer = new byte[1024];
				int len;
				while ((len = stream.Read(buffer, 0, 1024)) > 0) ms.Write(buffer, 0, len);
				var result = ms.ToArray();
				ms.Dispose();
				return result;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return null;
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

			try
			{
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
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return null;
		}

		#endregion
		#region Extern

		[System.Runtime.InteropServices.DllImport("msvcrt.dll", SetLastError = true)]
		static extern int _mkdir(string path);

		#endregion
		#region Helper Methods

		private static Dictionary<char, char> GetFileInvalidCharsReplacements()
		{
			var result = new Dictionary<char, char>();
			result.Add(':', '-');
			result.Add('/', ',');
			return result;
		}

		#endregion
	}
}