using System;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Types of hash algorithms to use
	/// </summary>
	public enum HashAlgorithmType
	{
		/// <summary>
		/// Perform no hash
		/// </summary>
		None,
		/// <summary>
		/// Message-Digest algorithm 5
		/// </summary>
		MD5,
		/// <summary>
		/// Secure Hash Algorithm 1
		/// </summary>
		SHA1,
		/// <summary>
		/// Secure Hash Algorithm 256
		/// </summary>
		SHA256,
		/// <summary>
		/// Secure Hash Algorithm 384
		/// </summary>
		SHA384,
		/// <summary>
		/// Secure Hash Algorithm 512
		/// </summary>
		SHA512,
		/// <summary>
		/// RACE Integrity Primitives Evaluation Message Digest 160
		/// </summary>
		RIPEMD160,
		/// <summary>
		/// Hash-based Message Authentication Code
		/// </summary>
		HMAC,
		/// <summary>
		/// Message Authentication Code (MAC) using TripleDES
		/// </summary>
		MACTripleDES
	}
}