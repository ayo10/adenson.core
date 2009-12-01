using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Adenson.IO
{
	/// <summary>
	/// Just a wrapper around System.IO.DirectoryInfo with special create methods
	/// </summary>
	public sealed class DirectoryInfo
	{
		#region Variables
		private static CreateDirectoryMode createDirectoryMode = CreateDirectoryMode.Auto;
		private System.IO.DirectoryInfo dir;
		#endregion
		#region Constructors

		static DirectoryInfo()
		{
			System.Collections.Generic.Dictionary<string, string> config = Adenson.Configuration.ConfigSectionHelper.GetDictionary("DirectoryInfo");
			if (config != null)
			{
				if (config.ContainsKey("CreateDirectoryMode")) createDirectoryMode = (CreateDirectoryMode)Enum.Parse(typeof(CreateDirectoryMode), (string)config["CreateDirectoryMode"], true);
			}
		}
		/// <summary>
		/// Initializes a new instance of the System.IO.DirectoryInfo class on the specified path.
		/// </summary>
		/// <param name="path">A string specifying the path on which to create the DirectoryInfo.</param>
		/// <exception cref="ArgumentNullException">path is null.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		/// <exception cref="ArgumentException">path contains invalid characters such as ", <, >, or |.</exception>
		/// <exception cref="PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum 
		/// length. For example, on Windows-based platforms, paths must be less than
		/// 248 characters, and file names must be less than 260 characters. The specified
		/// path, file name, or both are too long.
		/// </exception>
		public DirectoryInfo(string path)
		{
			dir = new System.IO.DirectoryInfo(path);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the full path of the directory.
		/// </summary>
		public string FullName 
		{
			get { return dir.FullName; }
		}
		/// <summary>
		/// Gets if the directory exists
		/// </summary>
		public bool Exists 
		{
			get { return dir.Exists; }
		}
		/// <summary>
		/// Gets the name of this DirectoryInfo instance.
		/// </summary>
		public string Name
		{
			get { return dir.Name; }
		}
		/// <summary>
		/// Gets the parent directory of a specified subdirectory.
		/// </summary>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		public DirectoryInfo Parent
		{
			get { return dir.Parent; }
		}
		/// <summary>
		/// Gets the root portion of a path.
		/// </summary>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		public DirectoryInfo Root
		{
			get { return dir.Root; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// Creates a directory.
		/// </summary>
		/// <exception cref="IOException">The directory cannot be created.</exception>
		public void Create()
		{
			if (createDirectoryMode == CreateDirectoryMode.Auto)
			{
				try
				{
					dir.Create();
				}
				catch
				{
					try
					{
						_mkdir(this.FullName);
					}
					catch (Exception ex)
					{
						throw new IOException("The directory cannot be created", ex);
					}
				}
			}
			else
			{
				if (createDirectoryMode == CreateDirectoryMode.CreateDirectory) dir.Create();
				else if (createDirectoryMode == CreateDirectoryMode.ExternMkDir) _mkdir(this.FullName);
			}
		}
		/// <summary>
		/// Creates a directory using a System.Security.AccessControl.DirectorySecurity object. (Not supported by CreateDirectoryMode)
		/// </summary>
		/// <param name="directorySecurity">The access control to apply to the directory.</param>
		public void Create(DirectorySecurity directorySecurity)
		{
			dir.Create(directorySecurity);
		}
		/// <summary>
		/// Creates a subdirectory or subdirectories on the specified path. The specified path can be relative to this instance of the System.IO.DirectoryInfo class.
		/// </summary>
		/// <param name="path">The specified path. This cannot be a different disk volume or Universal Naming Convention (UNC) name.</param>
		/// <returns>The last directory specified in path.</returns>
		/// <exception cref="ArgumentNullException">path is null.</exception>
		/// <exception cref="ArgumentException">path does not specify a valid file path or contains invalid DirectoryInfo characters.</exception>
		public DirectoryInfo CreateSubdirectory(string path)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path", ExceptionMessages.ArgumentNullOrEmpty);
			if (path.StartsWith("//")) throw new ArgumentException("Path cannot be UNC", "path");
			string dirPath = path;
			if (path.IndexOf("://") == -1) dirPath = Path.Combine(this.FullName, path);
			DirectoryInfo ldir = new DirectoryInfo(dirPath);
			ldir.Create();
			return ldir;
		}
		/// <summary>
		///  Creates a subdirectory or subdirectories on the specified path with the specified security.(Not supported by CreateDirectoryMode)
		/// </summary>
		/// <param name="path">The specified path. This cannot be a different disk volume or Universal Naming Convention (UNC) name.</param>
		/// <param name="directorySecurity"></param>
		/// <returns>The last directory specified in path.</returns>
		public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity)
		{
			return dir.CreateSubdirectory(path, directorySecurity);
		}
		/// <summary>
		/// Deletes this System.IO.DirectoryInfo if it is empty.
		/// </summary>
		/// <exception cref="IOException">The directory is not empty. -or-The directory is the application's current working directory.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		/// <exception cref="DirectoryNotFoundException">The directory described by this System.IO.DirectoryInfo object does not exist or could not be found.</exception>
		public void Delete()
		{
			dir.Delete();
		}
		/// <summary>
		/// Deletes this instance of a System.IO.DirectoryInfo, specifying whether to delete subdirectories and files.
		/// </summary>
		/// <param name="recursive">true to delete this directory, its subdirectories, and all files; otherwise, false</param>
		/// <exception cref="IOException">The directory is read-only.-or- The directory contains one or more files or subdirectories and recursive is false.-or-The directory is the application's current working directory.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		public void Delete(bool recursive)
		{
			dir.Delete(recursive);
		}
		//
		// Summary:
		//     Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates
		//     the access control list (ACL) entries for the directory described by the
		//     current System.IO.DirectoryInfo object.
		//
		// Returns:
		//     A System.Security.AccessControl.DirectorySecurity object that encapsulates
		//     the access control rules for the directory.
		//
		// Exceptions:
		//   System.SystemException:
		//     The directory could not be found or modified.
		//
		//   System.UnauthorizedAccessException:
		//     The current process does not have access to open the directory.
		//
		//   System.IO.IOException:
		//     An I/O error occurred while opening the directory.
		//
		//   System.PlatformNotSupportedException:
		//     The current operating system is not Microsoft Windows 2000 or later.
		//
		//   System.UnauthorizedAccessException:
		//     The directory is read-only.-or- This operation is not supported on the current
		//     platform.-or- The caller does not have the required permission.
		public DirectorySecurity GetAccessControl()
		{
			return dir.GetAccessControl();
		}
		//
		// Summary:
		//     Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates
		//     the specified type of access control list (ACL) entries for the directory
		//     described by the current System.IO.DirectoryInfo object.
		//
		// Parameters:
		//   includeSections:
		//     One of the System.Security.AccessControl.AccessControlSections values that
		//     specifies the type of access control list (ACL) information to receive.
		//
		// Returns:
		//     A System.Security.AccessControl.DirectorySecurity object that encapsulates
		//     the access control rules for the file described by the path parameter.ExceptionsException
		//     typeConditionSystem.SystemExceptionThe directory could not be found or modified.System.UnauthorizedAccessExceptionThe
		//     current process does not have access to open the directory.System.IO.IOExceptionAn
		//     I/O error occurred while opening the directory.System.PlatformNotSupportedExceptionThe
		//     current operating system is not Microsoft Windows 2000 or later.System.UnauthorizedAccessExceptionThe
		//     directory is read-only.-or- This operation is not supported on the current
		//     platform.-or- The caller does not have the required permission.
		public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
		{
			return dir.GetAccessControl(includeSections);
		}
		/// <summary>
		/// Returns the subdirectories of the current directory.
		/// </summary>
		/// <returns>An array of System.IO.DirectoryInfo objects.</returns>
		public DirectoryInfo[] GetDirectories()
		{
			System.IO.DirectoryInfo[] dirs = dir.GetDirectories();
			DirectoryInfo[] ldirs = new DirectoryInfo[dirs.Length];
			for (int i = 0; i < dirs.Length; i++) ldirs[i] = dirs[i];
			return ldirs;
		}
		/// <summary>
		/// Returns an array of directories in the current System.IO.DirectoryInfo matching the given search criteria.
		/// </summary>
		/// <param name="searchPattern">The search string, such as "System*", used to search for all directories beginning with the word "System".</param>
		/// <returns>An array of type DirectoryInfo matching searchPattern.</returns>
		public DirectoryInfo[] GetDirectories(string searchPattern)
		{
			System.IO.DirectoryInfo[] dirs = dir.GetDirectories(searchPattern);
			DirectoryInfo[] ldirs = new DirectoryInfo[dirs.Length];
			for (int i = 0; i < dirs.Length; i++) ldirs[i] = dirs[i];
			return ldirs;
		}
		//
		// Summary:
		//     Returns an array of directories in the current System.IO.DirectoryInfo matching
		//     the given search criteria and using a value to determine whether to search
		//     subdirectories.
		//
		// Parameters:
		//   searchPattern:
		//     The search string, such as "System*", used to search for all directories
		//     beginning with the word "System".
		//
		//   searchOption:
		//     One of the values of the System.IO.SearchOption enumeration that specifies
		//     whether the search operation should include only the current directory or
		//     should include all subdirectories.
		//
		// Returns:
		//     An array of type DirectoryInfo matching searchPattern.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     searchPattern is null.
		//
		//   System.IO.DirectoryNotFoundException:
		//     The path encapsulated in the DirectoryInfo object is invalid, such as being
		//     on an unmapped drive.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission.
		public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
		{
			System.IO.DirectoryInfo[] dirs = dir.GetDirectories(searchPattern, searchOption);
			DirectoryInfo[] ldirs = new DirectoryInfo[dirs.Length];
			for (int i = 0; i < dirs.Length; i++) ldirs[i] = dirs[i];
			return ldirs;
		}
		//
		// Summary:
		//     Returns a file list from the current directory.
		//
		// Returns:
		//     An array of type System.IO.FileInfo.
		//
		// Exceptions:
		//   System.IO.DirectoryNotFoundException:
		//     The path is invalid, such as being on an unmapped drive.
		public FileInfo[] GetFiles()
		{
			return dir.GetFiles();
		}
		//
		// Summary:
		//     Returns a file list from the current directory matching the given searchPattern.
		//
		// Parameters:
		//   searchPattern:
		//     The search string, such as "*.txt".
		//
		// Returns:
		//     An array of type System.IO.FileInfo.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     searchPattern is null.
		//
		//   System.IO.DirectoryNotFoundException:
		//     The path is invalid, such as being on an unmapped drive.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission.
		public FileInfo[] GetFiles(string searchPattern)
		{
			return dir.GetFiles(searchPattern);
		}
		//
		// Summary:
		//     Returns a file list from the current directory matching the given searchPattern
		//     and using a value to determine whether to search subdirectories.
		//
		// Parameters:
		//   searchPattern:
		//     The search string, such as "System*", used to search for all directories
		//     beginning with the word "System".
		//
		//   searchOption:
		//     One of the values of the System.IO.SearchOption enumeration that specifies
		//     whether the search operation should include only the current directory or
		//     should include all subdirectories.
		//
		// Returns:
		//     An array of type System.IO.FileInfo.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     searchPattern is null.
		//
		//   System.IO.DirectoryNotFoundException:
		//     The path is invalid, such as being on an unmapped drive.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission.
		public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
		{
			return dir.GetFiles(searchPattern, searchOption);
		}
		//
		// Summary:
		//     Returns an array of strongly typed System.IO.FileSystemInfo entries representing
		//     all the files and subdirectories in a directory.
		//
		// Returns:
		//     An array of strongly typed System.IO.FileSystemInfo entries.
		//
		// Exceptions:
		//   System.IO.DirectoryNotFoundException:
		//     The path is invalid, such as being on an unmapped drive.
		public FileSystemInfo[] GetFileSystemInfos()
		{
			return dir.GetDirectories();
		}
		//
		// Summary:
		//     Retrieves an array of strongly typed System.IO.FileSystemInfo objects representing
		//     the files and subdirectories matching the specified search criteria.
		//
		// Parameters:
		//   searchPattern:
		//     The search string, such as "System*", used to search for all directories
		//     beginning with the word "System".
		//
		// Returns:
		//     An array of strongly typed FileSystemInfo objects matching the search criteria.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     searchPattern is null.
		//
		//   System.IO.DirectoryNotFoundException:
		//     The specified path is invalid, such as being on an unmapped drive.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission.
		public FileSystemInfo[] GetFileSystemInfos(string searchPattern)
		{
			return dir.GetDirectories();
		}
		//
		// Summary:
		//     Moves a System.IO.DirectoryInfo instance and its contents to a new path.
		//
		// Parameters:
		//   destDirName:
		//     The name and path to which to move this directory. The destination cannot
		//     be another disk volume or a directory with the identical name. It can be
		//     an existing directory to which you want to add this directory as a subdirectory.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     destDirName is null.
		//
		//   System.ArgumentException:
		//     destDirName is an empty string (''").
		//
		//   System.IO.IOException:
		//     An attempt was made to move a directory to a different volume. -or-destDirName
		//     already exists.-or-You are not authorized to access this path.-or- The directory
		//     being moved and the destination directory have the same name.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission.
		//
		//   System.IO.DirectoryNotFoundException:
		//     The destination directory cannot be found.
		public void MoveTo(string destDirName)
		{
			dir.MoveTo(destDirName);
		}
		//
		// Summary:
		//     Applies access control list (ACL) entries described by a System.Security.AccessControl.DirectorySecurity
		//     object to the directory described by the current System.IO.DirectoryInfo
		//     object.
		//
		// Parameters:
		//   directorySecurity:
		//     A System.Security.AccessControl.DirectorySecurity object that describes an
		//     ACL entry to apply to the directory described by the path parameter.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The directorySecurity parameter is null.
		//
		//   System.SystemException:
		//     The file could not be found or modified.
		//
		//   System.UnauthorizedAccessException:
		//     The current process does not have access to open the file.
		//
		//   System.PlatformNotSupportedException:
		//     The current operating system is not Microsoft Windows 2000 or later.
		public void SetAccessControl(DirectorySecurity directorySecurity)
		{
			dir.SetAccessControl(directorySecurity);
		}
		//
		// Summary:
		//     Returns the original path that was passed by the user.
		//
		// Returns:
		//     Returns the original path that was passed by the user.
		public override string ToString()
		{
			return dir.ToString();
		}

		public static implicit operator DirectoryInfo(System.IO.DirectoryInfo dir)
		{
			return new DirectoryInfo(dir.FullName);
		}

		#endregion
		#region Extern

		[DllImport("msvcrt.dll", SetLastError = true)]
		static extern int _mkdir(string path);

		#endregion
	}
}