using System;

namespace Adenson.Cryptography
{
	/// <summary>
	/// Types of entryptors
	/// </summary>
	public enum EncryptorType : int
	{
		/// <summary>
		/// None specified
		/// </summary>
		None = 0,
		/// <summary>
		/// Also known as AES
		/// </summary>
		Rijndael = 1,
		/// <summary>
		/// TripleDes
		/// </summary>
		TripleDES = 2,
		/// <summary>
		/// Just Des
		/// </summary>
		DES = 3
	}
}