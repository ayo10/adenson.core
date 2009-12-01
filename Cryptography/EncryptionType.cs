using System;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Types of entryptors
	/// </summary>
	public enum EncryptorType : byte
	{
		/// <summary>
		/// Also known as Rijndael
		/// </summary>
		Rijndael = 1,
		/// <summary>
		/// Also known as Rijndael
		/// </summary>
		AES = 1,
		/// <summary>
		/// TripleDes
		/// </summary>
		TripleDES = 2,
		/// <summary>
		/// Just Des
		/// </summary>
		DES = 3,
		/// <summary>
		/// Custom Entryptor
		/// </summary>
		Custom = 4
	}
}