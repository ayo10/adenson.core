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
			//<Encryptor Type="AES" Key="jzAH8SMGI+x7XfD0PuUp9jGaVWoOQdDK5Cb9qzTbFq8=" Vector="teY2aQzLPW3ThSKxTB31Kw==" KeyFormat="Base64"/>
			foreach (Configuration.EncryptorElement elem in section.Encryptors)
			{
				if (elem.EncryptorType == EncryptorType.Custom && String.IsNullOrEmpty(elem.TypeName)) throw new InvalidOperationException(ExceptionMessages.CustomEncryptorMissingAttributes);
				encryptors.Add(elem.Name, elem.GetEncryptor());
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
		/// <param name="toEncrypt"></param>
		/// <returns></returns>
		public static string Encrypt(string toEncrypt)
		{
			if (toEncrypt == null) throw new ArgumentNullException("toEncrypt", ExceptionMessages.ArgumentNull);

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
			if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key", ExceptionMessages.ArgumentNull);
			if (toEncrypt == null) throw new ArgumentNullException("toEncrypt", ExceptionMessages.ArgumentNull);
			if (!encryptors.ContainsKey(key)) throw new ArgumentException(ExceptionMessages.NoEncryptorExists, "key");
			return encryptors[key].Encrypt(toEncrypt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <returns></returns>
		public static string Decrypt(string toDecrypt)
		{
			if (toDecrypt == null) throw new ArgumentNullException("toDecrypt", ExceptionMessages.ArgumentNull);

			return Encryptor.Base.Decrypt(toDecrypt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <returns></returns>
		public static string Decrypt(string key, string toDecrypt)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key", ExceptionMessages.ArgumentNull);
			if (toDecrypt == null) throw new ArgumentNullException("toDecrypt", ExceptionMessages.ArgumentNull);
			if (!encryptors.ContainsKey(key)) throw new ArgumentException(ExceptionMessages.NoEncryptorExists, "key");

			return encryptors[key].Decrypt(toDecrypt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toEncrypt"></param>
		/// <param name="encryptedString"></param>
		/// <returns></returns>
		public static bool TryEncrypt(string toEncrypt, out string encryptedString)
		{
			if (toEncrypt == null) throw new ArgumentNullException("toEncrypt", ExceptionMessages.ArgumentNull);

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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toDecrypt"></param>
		/// <param name="descryptedString"></param>
		/// <returns></returns>
		public static bool TryDecrypt(string toDecrypt, out string descryptedString)
		{
			if (toDecrypt == null) throw new ArgumentNullException("toDecrypt", ExceptionMessages.ArgumentNull);

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

		#endregion
	}
}