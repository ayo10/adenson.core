using System;

namespace Adenson.IO
{
	/// <summary>
	/// Default, uses Directory.CreateDirectory
	/// </summary>
	public enum CreateDirectoryMode
	{
		/// <summary>
		/// Uses Directory.CreateDirectory
		/// </summary>
		CreateDirectory = 1,
		/// <summary>
		/// Uses msvcrt.dll This is not 100% clean solution (it works only if "Allow Calls to Unmanaged Code" permission is granted to your code) but it will fly in 90% of real-world ASP.NET hosting scenarios.
		/// </summary> 
		ExternMkDir = 2,
		/// <summary>
		/// First attempts to use Directory.CreateDirectory and if that fails, makes a call to "Unmanaged Code"
		/// </summary>
		Auto
	}
}