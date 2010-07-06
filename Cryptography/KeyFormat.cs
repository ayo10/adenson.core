using System;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Types of crypto keys
	/// </summary>
	public enum KeyFormat : byte
	{
		/// <summary>
		/// Key is stored in base 64
		/// </summary>
		Base64,
		/// <summary>
		/// Key is comma-delimited bytes
		/// </summary>
		CommaDelimitedBytes
	}
}