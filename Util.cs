using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Adenson.Cryptography;
using Adenson.Log;

namespace Adenson
{
	/// <summary>
	/// Collection of utility methods
	/// </summary>
	public static class Util
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(Util));
		private readonly static Dictionary<char, char> fileInvalidCharsReplacements = GetFileInvalidCharsReplacements();
		#endregion
		#region Methods

		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static object CreateInstance(string typeName)
		{
			if (Util.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");
			Type type = Util.GetType(typeName);
			if (type == null) throw new TypeLoadException(String.Format(Exceptions.TypeArgCouldNotBeLoaded, typeName));
			return Activator.CreateInstance(type);
		}
		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor, and casts it to specified generic type parameter
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeName)
		{
			if (Util.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");
			return (T)Util.CreateInstance(typeName);
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
			return Util.ReadStream(new Uri(filePath, UriKind.Absolute));
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
				Stream stream;
				if (File.Exists(url.AbsolutePath)) stream = File.Open(url.AbsolutePath, FileMode.Open);
				else
				{
					System.Net.WebRequest request = System.Net.WebRequest.Create(url);
					stream = request.GetResponse().GetResponseStream();
				}
				return Util.ReadStream(stream);
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
		/// <summary>
		/// Gets the System.Type with the specified name.
		/// </summary>
		/// <param name="typeName">The assembly-qualified name of the type to get.</param>
		/// <returns>The type with the specified name.</returns>
		public static Type GetType(string typeName)
		{
			if (Util.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");

			Type type = Type.GetType(typeName, false, true);
			if (type == null)
			{
				string[] splits = typeName.Split(';');
				if (splits.Length >= 2)
				{
					string assembly = splits[0].Trim();
					string typestr = splits[1].Trim();
					type = Type.GetType(typestr + ", " + assembly, false, true);
				}
			}
			return type;
		}
		/// <summary>
		/// Tries to convert the specified value
		/// </summary>
		/// <param name="value">the value to convert, must be in the form {[Type Full Name], Value}</param>
		/// <param name="result">the output of the result</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert(string value, out object result)
		{
			if (Util.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");
			string[] splits = value.Split(',');
			if (splits.Length != 2) throw new ArgumentException(String.Format(Exceptions.TypeExpectedMustBeArg, "[Type Full Name], Value"));
			Type type = Util.GetType(splits[1]);
			if (type == null) throw new TypeLoadException(String.Format(Exceptions.TypeArgCouldNotBeLoaded, splits[0]));
			return Util.TryConvert(splits[0], type, out result);
		}
		/// <summary>
		/// Tries to convert the specified value
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