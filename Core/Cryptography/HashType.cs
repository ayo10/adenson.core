using System;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Types of hash algorithms to use
	/// </summary>
	public enum HashType
	{
		/// <summary>
		/// Performs no hash.
		/// </summary>
		None,

		/// <summary>
		/// Message-Digest algorithm 5.
		/// </summary>
		MD5,

		/// <summary>
		/// Secure Hash Algorithm 1.
		/// </summary>
		SHA1,

		/// <summary>
		/// Computes a hash using the Secure Hash Algorithm 256.
		/// </summary>
		SHA256,

		/// <summary>
		/// Computes a hash using the Secure Hash Algorithm 384.
		/// </summary>
		SHA384,

		/// <summary>
		/// Computes a hash using the Secure Hash Algorithm 512.
		/// </summary>
		SHA512,

#if !NETSTANDARD1_3
		/// <summary>
		/// Computes a hash using the RACE Integrity Primitives Evaluation Message Digest 160.
		/// </summary>
		RIPEMD160,
#endif

		/// <summary>
		/// Computes a Message Authentication Code (MAC) using TripleDES.
		/// </summary>
		/// <remarks>Use a byte array of at least length of 16.</remarks>
		MACTripleDES,

		/// <summary>
		/// Computes a Hash-based Message Authentication Code (HMAC) using the RIPEMD160 hash function.
		/// </summary>
		HMACRIPEMD160,

		/// <summary>
		/// Implements password-based key derivation functionality, PBKDF2, by using a pseudo-random number generator based on HMACSHA1 and interations of 5000.
		/// </summary>
		PBKDF2
	}
}