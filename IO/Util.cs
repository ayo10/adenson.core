using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Adenson.ListenQuest.Converters;
using Adenson.Log;

namespace Adenson.ListenQuest
{
	public static class Util
	{
		#region Variables
		private static Version _version;
		private static string _versionAsString;
		private static string _company;
		private static string _copyright;
		public readonly static Hashtable<char, char> FileInvalidCharsReplacements = new Hashtable<char, char>();
		#endregion
		#region Constructor

		static Util()
		{
			FileInvalidCharsReplacements.Add(':', '-');
			FileInvalidCharsReplacements.Add('/', ',');
		}

		#endregion
		#region Properties

		public static string AssemblyCompany
		{
			get { return _company ?? (_company = typeof(Util).Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true).OfType<AssemblyCompanyAttribute>().First().Company); }
		}
		public static string AssemblyCopyright
		{
			get { return _copyright ?? (_copyright = typeof(Util).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).OfType<AssemblyCopyrightAttribute>().First().Copyright); }
		}
		public static bool IsBeta
		{
			get
			{ 
				#if BETA
				return true;
				#else
				return false;
				#endif
			}
		}
        public static bool IsX64
        {
            get { return IntPtr.Size == 8; }
        }
		public static bool IsInDesignMode
		{
			get { return (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue; }
		}
		public static string LocalAppPath
		{
			get
			{
				DirectoryInfo localAppPath = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
				string dir = Path.Combine(Path.Combine(localAppPath.FullName, Strings.CompanyName), Strings.ApplicationName);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
				return dir;
			}
		}
		public static string LocalAppCoverArtPath
		{
			get { return Path.Combine(Util.LocalAppPath, "CoverArt\\"); }
		}
		public static string LocalAppPluginsPath
		{
			get { return Path.Combine(Util.LocalAppPath, "Plugins\\"); }
		}
		public static Version Version
		{
			get { return _version ?? (_version = typeof(Util).Assembly.GetName().Version); }
		}
		public static string VersionAsString
		{
			get
			{
				if (_versionAsString == null)
				{
					System.Text.StringBuilder result = new System.Text.StringBuilder();
					result.Append(Util.Version.ToString());
					//#if BETA
					//result.Append("b");
					//#endif
					//result.AppendFormat(" x{0}", Util.IsX64 ? "64" : "32");
					//#if DEBUG
					//result.Append("(d)");
					//#endif
					_versionAsString = result.ToString();
				}
				return _versionAsString;
			}
		}

		#endregion
		#region Methods

		public static object CreateInstance(string typeString)
		{
			Type type = Util.GetType(typeString);
			if (type == null) throw new TypeLoadException(String.Format(Exceptions.TypeArgCouldNotBeLoaded, typeString));
			return Activator.CreateInstance(type);
		}
		public static T CreateInstance<T>(string typeString)
		{
			return (T)Util.CreateInstance(typeString);
		}
		public static T CreateInstance<T>(Type type)
		{
			return (T)Activator.CreateInstance(type);
		}
		public static int Compare(IComparable c1, IComparable c2)
		{
			if (c1 == null && c2 == null) return 0;
			else if (c1 != null && c2 == null) return 1;
			else if (c1 == null && c2 != null) return -1;
			return c1.CompareTo(c2);
		}
		public static string CreateLocalFile(string directory, byte[] buffer, bool overwrite)
		{
			if (Util.IsNullOrEmpty(directory)) throw new ArgumentNullException("directory");
			if (!directory.StartsWith(Util.LocalAppPath)) throw new NotSupportedException();
			if (buffer == null) return null;
			if (buffer.Length == 0) return null;

			string filePath = Path.Combine(directory, Util.GetHash(buffer));
			if (!File.Exists(filePath) || overwrite)
			{
				if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
				FileStream stream = File.Create(filePath);
				stream.Write(buffer, 0, buffer.Length);
				stream.Close();
			}
			return filePath;
		}
		public static string[] GetFiles(string folder, IEnumerable<string> extensions)
		{
			List<string> filesToProcess = new List<string>();
			foreach (string ext in extensions)
			{
				filesToProcess.AddRange(Directory.GetFiles(folder, ext.Trim(), SearchOption.AllDirectories).ToList());
			}
			return filesToProcess.ToArray();
		}
		public static string GetHash(byte[] buffer)
		{
			if (buffer == null) return null;
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			string hash = System.Convert.ToBase64String(md5.ComputeHash(buffer)).Replace("\\", String.Empty).Replace("/", String.Empty).Replace("=", String.Empty);
			return hash;
		}
		public static bool GetIsDirectory(string fullPath)
		{
			return Path.GetFileName(fullPath) == Path.GetFileNameWithoutExtension(fullPath);//Prob a directory
		}
		public static Type GetType(string typeString)
		{
			Type type = Type.GetType(typeString);
			if (type == null)
			{
				string[] splits = typeString.Split(';');
				string assembly = splits.Length == 1 ? "ListenQuest" : splits[0].Trim();
				string typeName = splits.Length == 1 ? splits[0].Trim() : splits[1].Trim();
				if (typeName.StartsWith("ListenQuest")) typeName = "Adenson." + typeName;
				type = Type.GetType(typeName + ", " + assembly);
				if (type == null && splits.Length == 1) type = Type.GetType(typeName + ", ListenQuest.Controls");
				if (type == null && splits.Length == 1) type = Type.GetType(typeName + ", ListenQuest.Core");
			}
			return type;
		}
		public static string GetTypeName(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			string assembly = type.Assembly.FullName.Split(',')[0];
			return type.FullName + ", " + assembly;
		}
		public static T EnumParse<T>(string value)
		{
			if (!Enum.IsDefined(typeof(T), value)) throw new ArgumentException();
			return (T)Enum.Parse(typeof(T), value);
		}
		public static bool Equals(IEnumerable<string> array1, IEnumerable<string> array2)
		{
			return Util.Equals(array1, array2, StringComparison.CurrentCultureIgnoreCase);
		}
		/// <summary>
		/// Does equality comparism of both arrays, if not same instance, then item by item comparism
		/// </summary>
		/// <typeparam name="T">The type of items</typeparam>
		/// <param name="array1">The frst array</param>
		/// <param name="array2">The second array</param>
		/// <returns>true if elements in array1 are equal and at the same index as elements in array2, false otherwise</returns>
		public static bool Equals<T>(IEnumerable<T> array1, IEnumerable<T> array2)
		{
			if (Object.ReferenceEquals(array1, array2)) return true;
			if (array1 == array2) return true;
			if (array1 == null && array2 != null || array1 != null && array2 == null) return false;
			if (array1.Count() != array2.Count()) return false;
			for (int i = 0; i < array1.Count(); i++)
			{
				T e1 = array1.ElementAt(i);
				T e2 = array2.ElementAt(i);
				if (!Object.Equals(e1, e2)) return false;
			}
			return true;
		}
		public static bool Equals(IEnumerable<string> array1, IEnumerable<string> array2, StringComparison comparism)
		{
			if (array1 == array2) return true;
			if (array1 == null && array2 != null || array1 != null && array2 == null) return false;
			if (array1.Count() != array2.Count()) return false;
			for (int i = 0; i < array1.Count(); i++)
			{
				string e1 = array1.ElementAt(i);
				string e2 = array2.ElementAt(i);
				if (e1 == null && e2 != null || e1 != null && e2 == null) return false;
				if (!e1.Equals(e2, comparism)) return false;
			}
			return true;
		}
		public static string FixFilePath(string path)
		{
			IEnumerable<char> intersect = path.ToCharArray().Intersect(Settings.Default.InvalidFileNameChars.Split(',').Cast<char>());
			if (intersect.Count() > 0)
			{
				string result = path;
				foreach (char c in intersect)
				{
					char r = Util.FileInvalidCharsReplacements[c];
					result = result.Replace(c.ToString(), r == char.MinValue ? "" : r.ToString());
				}
				return result;
			}
			return path;
		}
		public static System.Drawing.Icon GetIcon(string iconName)
		{
			Uri url = Util.GetResourceUrl(iconName);
			System.Windows.Resources.StreamResourceInfo sri = Application.GetResourceStream(url);
			if (sri != null)
			{
				System.Drawing.Icon icon = new System.Drawing.Icon(sri.Stream);
				WeakReference wr = new WeakReference(icon);
				icon = null;
				return (System.Drawing.Icon)wr.Target;
			}
			return null;
		}
		public static System.Drawing.Icon GetIcon(ImageSource imageSource)
		{
			if (Uri.IsWellFormedUriString(imageSource.ToString(), UriKind.Absolute)) return GetIcon(imageSource.ToString());
			else throw new NotSupportedException();
		}
		public static Uri GetResourceUrl(string resourcePath)
		{
			return Util.GetResourceUrl(resourcePath, String.Empty);
		}
		public static Uri GetResourceUrl(string resourcePath, string assemblyName)
		{
			if (Util.IsNullOrEmpty(resourcePath)) throw new ArgumentNullException("resourcePath");
			if (resourcePath.Contains("://", StringComparison.CurrentCultureIgnoreCase)) return new Uri(resourcePath);
			string url = String.Format("pack://application:,,,/{0}{1}", String.IsNullOrEmpty(assemblyName) ? String.Empty : assemblyName + ";component/", (resourcePath.StartsWith("/") ? resourcePath.Substring(1) : resourcePath));
			return new Uri(url, UriKind.Absolute);
		}
		public static Uri GetResourceUrl(string resourcePath, System.Reflection.Assembly assembly)
		{
			return Util.GetResourceUrl(resourcePath, assembly == null ? null : assembly.FullName.Split(',')[0]);
		}
		public static Stream GetResourceStream(string resourcePath)
		{
			StreamResourceInfo sri = Application.GetResourceStream(Util.GetResourceUrl(resourcePath));
			if (sri != null) return sri.Stream;
			return null;
		}
		public static ImageSource CreateImageSource(string resourcePath, double? decodeWidth)
		{
			return Util.CreateImageSource(resourcePath, String.Empty, decodeWidth);
		}
		public static ImageSource CreateImageSource(string resourcePath, string assemblyName, double? decodeWidth)
		{
			return Util.CreateImageSource(Util.GetResourceUrl(resourcePath, assemblyName), decodeWidth);
		}
		public static ImageSource CreateImageSource(string resourcePath, System.Reflection.Assembly assembly, double? decodeWidth)
		{
			return Util.CreateImageSource(Util.GetResourceUrl(resourcePath, assembly), decodeWidth);
		}
		public static ImageSource CreateImageSource(Uri url, double? decodeWidth)
		{
			if (url == null) return null;
			if (url.IsFile && !File.Exists(url.LocalPath)) return null;

			try
			{
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				if (decodeWidth != null) bi.DecodePixelWidth = (int)decodeWidth;
				bi.CacheOption = BitmapCacheOption.None;
				bi.UriSource = url;
				bi.EndInit();
				return bi;
			}
			catch (InvalidOperationException ex)
			{
				Logger.GetLogger(typeof(Util)).Error(ex);
				return null;
			}
		}
		public static bool IsNullOrEmpty(string str)
		{
			return str == null ? true : String.IsNullOrEmpty(str.Trim());
		}
		public static byte[] ReadStream(string filePath)
		{
			try
			{
				if (!File.Exists(filePath)) throw new System.IO.FileNotFoundException(Exceptions.FileNotFound, filePath);
				Stream stream = File.Open(filePath, FileMode.Open);
				MemoryStream ms = new MemoryStream();
				byte[] buffer = new byte[1024];
				int len;
				while ((len = stream.Read(buffer, 0, 1024)) > 0) ms.Write(buffer, 0, len);
				stream.Close();
				return ms.ToBytes();
			}
			catch (Exception ex)
			{
				Logger.Error(typeof(Util), ex);
			}
			return null;
		}
		public static byte[] ReadStream(Uri url)
		{
			try
			{
				System.Net.WebRequest request = System.Net.WebRequest.Create(url);
				Stream stream = request.GetResponse().GetResponseStream();
				MemoryStream ms = new MemoryStream();
				byte[] buffer = new byte[1024];
				int len;
				while ((len = stream.Read(buffer, 0, 1024)) > 0) ms.Write(buffer, 0, len);
				return ms.ToBytes();
			}
			catch (Exception ex)
			{
				Logger.Error(typeof(Util), ex);
			}
			return null;
		}
		public static string[] Replace(string[] array, int index, string value)
		{
			if (array == null) throw new ArgumentNullException("array");
			if (index < 0) throw new ArgumentOutOfRangeException("index");
			if (index < array.Length)
			{
				array[index] = value;
				return array;
			}
			else
			{
				List<string> list = new List<string>(array);
				while (list.Count < index) list.Add(null);
				list.Insert(index, value);
				return list.ToArray();
			}
		}
		/// <summary>
		/// Trys to convert the specified value
		/// </summary>
		/// <param name="value">the value to convert, must be in the form {[Type Full Name], Value}</param>
		/// <param name="result">the output of the result</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert(string value, out object result)
		{
			if (Util.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
			string[] splits = value.Split(',');
			if (splits.Length != 2) throw new ArgumentException(String.Format(Exceptions.TypeExpectedMustBeArg, "[Type Full Name], Value"));
			Type type = Util.GetType(splits[1]);
			if (type == null) throw new ArgumentException(String.Format(Exceptions.TypeArgCouldNotBeLoaded, splits[0]));
			return Util.TryConvert(splits[0], type, out result);
		}
		/// <summary>
		/// Trys to convert the specified value
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <param name="type">The type of the object we need to convert</param>
		/// <param name="result">the output of the result</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert(object value, Type type, out object result)
		{
			TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
			IListToArrayTypeConverter listConverter = typeConverter as IListToArrayTypeConverter;

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
				if (listConverter != null) result = listConverter.ConvertFrom(value);
				else if (typeConverter != null && value != null && typeConverter.CanConvertFrom(value.GetType()))
				{
					result = typeConverter.ConvertFrom(value);
				}
			}
			if (result == null) return false;
			return true;
		}

		#endregion
	}
}