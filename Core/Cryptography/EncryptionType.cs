using System;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Types of encryption algorithms to use.
	/// </summary>
	public enum EncryptionType
	{
		/// <summary>
		/// Performs no hash.
		/// </summary>
		None,

		/// <summary>
		/// MS implemenentation of AES that meets FIPS-197 specification for AES.
		/// </summary>
		AES,

		/// <summary>
		/// Rijndael symmetric encryption.
		/// </summary>
		Rijndael,

		/// <summary>
		/// Triple Data Encryption Standard algorithm.
		/// </summary>
		TripleDES
	}
}