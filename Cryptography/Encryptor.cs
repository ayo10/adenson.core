using System;
using System.Collections.Generic;
using Adenson.Log;
using Adenson.Configuration;

namespace Adenson.Cryptography
{
	/// <summary>
	/// String encryption and decription utilities
	/// </summary>
	public static class Encryptor
	{
		#region Variables
		private static Logger logger = new Logger(typeof(Encryptor));
		private static Dictionary<string, BaseEncryptor> encryptors = new Dictionary<string,BaseEncryptor>();
		#endregion
		#region Constructor

		static Encryptor()
		{
			EncryptorSection section = (EncryptorSection)ConfigSectionHelper.GetSection("Encryptors");
			if (section != null)
			{
				foreach (EncryptorElement elem in section.Encryptors)
				{
					if (elem.EncryptorType == EncryptorType.Custom && Util.IsNullOrWhiteSpace(elem.TypeName)) throw new InvalidOperationException(Exceptions.CustomEncryptorMissingAttributes);
					encryptors.Add(elem.Name, elem.GetEncryptor());
				}
			}
		}

		#endregion
		#region Properties

		internal static BaseEncryptor Base
		{
			get { return encryptors["Default"]; }
		}

		#endregion
		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <returns></returns>
		public static string Decrypt(string toDecrypt)
		{
			if (toDecrypt == null) throw new ArgumentNullException("toDecrypt");

			return Encryptor.Base.Decrypt(toDecrypt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key">The key to look up</param>
		/// <param name="toDecrypt"></param>
		/// <returns></returns>
		public static string Decrypt(string key, string toDecrypt)
		{
			if (Util.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
			if (toDecrypt == null) throw new ArgumentNullException("toDecrypt");
			if (!encryptors.ContainsKey(key)) throw new ArgumentException(Exceptions.NoEncryptorExists, "key");

			return encryptors[key].Decrypt(toDecrypt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toEncrypt"></param>
		/// <returns></returns>
		public static string Encrypt(string toEncrypt)
		{
			if (toEncrypt == null) throw new ArgumentNullException("toEncrypt");

			return Encryptor.Base.Encrypt(toEncrypt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="toEncrypt"></param>
		/// <returns></returns>
		public static string Encrypt(string key, string toEncrypt)
		{
			if (Util.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
			if (toEncrypt == null) throw new ArgumentNullException("toEncrypt");
			if (!encryptors.ContainsKey(key)) throw new ArgumentException(Exceptions.NoEncryptorExists, "key");
			return encryptors[key].Encrypt(toEncrypt);
		}
		/// <summary>
		/// Gets the MD5 hash of specified bit array
		/// </summary>
		/// <param name="buffer">The bit array</param>
		/// <returns>String representation of the md5 hash of array</returns>
		public static string GetMD5Hash(byte[] buffer)
		{
			if (buffer == null) return null;
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			string hash = System.Convert.ToBase64String(md5.ComputeHash(buffer)).Replace("\\", String.Empty).Replace("/", String.Empty).Replace("=", String.Empty);
			return hash;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <param name="descryptedString"></param>
		/// <returns></returns>
		public static bool TryDecrypt(string toDecrypt, out string descryptedString)
		{
			if (toDecrypt == null) throw new ArgumentNullException("toDecrypt");

			descryptedString = null;
			try
			{
				descryptedString = Encryptor.Decrypt(toDecrypt);
				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toEncrypt"></param>
		/// <param name="encryptedString"></param>
		/// <returns></returns>
		public static bool TryEncrypt(string toEncrypt, out string encryptedString)
		{
			if (toEncrypt == null) throw new ArgumentNullException("toEncrypt");

			encryptedString = null;
			try
			{
				encryptedString = Encryptor.Encrypt(toEncrypt);
				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
			return false;
		}

		#endregion
	}
}