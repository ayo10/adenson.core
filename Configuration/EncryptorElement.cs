using System;
using System.Configuration;
using Adenson.Cryptography;

namespace Adenson.Configuration
{
	internal sealed class EncryptorElement : ConfigurationElement
	{
		#region Variables
		private static readonly byte[] DefaultKey = new byte[] { 143, 48, 7, 241, 35, 6, 35, 236, 123, 93, 240, 244, 62, 229, 41, 246, 49, 154, 85, 106, 14, 65, 208, 202, 228, 38, 253, 171, 52, 219, 22, 175 };
		private static readonly byte[] DefaultIV = new byte[] { 181, 230, 54, 105, 12, 203, 61, 109, 211, 133, 34, 177, 76, 29, 245, 43 };
		#endregion
		#region Properties

		[ConfigurationProperty("Name", IsKey=true, IsRequired=true)]
		public string Name
		{
			get { return (string)this["Name"]; }
			set { this["Name"] = value; }
		}

		[ConfigurationProperty("Type")]
		public EncryptorType EncryptorType
		{
			get { return this["Type"] == null ? EncryptorType.AES : (EncryptorType)this["Type"]; }
			set { this["Type"] = value; }
		}

		[ConfigurationProperty("KeyFormat")]
		public KeyFormat KeyFormat
		{
			get { return (KeyFormat)this["KeyFormat"]; }
			set { this["KeyFormat"] = value; }
		}

		[ConfigurationProperty("Key")]
		public string Key
		{
			get { return (string)this["Key"]; }
			set { this["Key"] = value; }
		}

		[ConfigurationProperty("Vector")]
		public string Vector
		{
			get { return (string)this["Vector"]; }
			set { this["Vector"] = value; }
		}

		[ConfigurationProperty("TypeName")]
		public string TypeName
		{
			get { return (string)this["TypeName"]; }
			set { this["TypeName"] = value; }
		}

		[ConfigurationProperty("AssemblyName")]
		public string AssemblyName
		{
			get { return (string)this["AssemblyName"]; }
			set { this["AssemblyName"] = value; }
		}

		public byte[] KeyInBytes
		{
			get
			{
				if (String.IsNullOrEmpty(this.Key)) return EncryptorElement.DefaultKey;
				switch (this.KeyFormat)
				{
					case KeyFormat.CommaDelimitedBytes: return this.Split(this.Key);
					default: return Convert.FromBase64String(this.Key);
				}
			}
		}
		public byte[] VectorInBytes
		{
			get
			{
				if (String.IsNullOrEmpty(this.Vector)) return EncryptorElement.DefaultIV;
				switch (this.KeyFormat)
				{
					case KeyFormat.CommaDelimitedBytes: return this.Split(this.Vector);
					default: return Convert.FromBase64String(this.Vector);
				}
			}
		}

		#endregion
		#region Methods

		public BaseEncryptor GetEncryptor()
		{
			BaseEncryptor encryptor = null;
			switch (this.EncryptorType)
			{
				case EncryptorType.AES: encryptor = new Rijndael(this.KeyInBytes, this.VectorInBytes); break;
				case EncryptorType.DES: encryptor = new DES(this.KeyInBytes, this.VectorInBytes); break;
				case EncryptorType.TripleDES: encryptor = new TripleDES(this.KeyInBytes, this.VectorInBytes); break;
				case EncryptorType.Custom:
					Type encType = null;
					if (!String.IsNullOrEmpty(this.AssemblyName))
					{
						System.Reflection.Assembly assembly = AppDomain.CurrentDomain.Load(this.AssemblyName);
						encType = assembly.GetType(this.TypeName);
					}
					else encType = Type.GetType(this.TypeName);
					encryptor = (BaseEncryptor)Activator.CreateInstance(encType);
					encryptor.SetKeys(this.KeyInBytes, this.VectorInBytes);
					break;
			}
			return encryptor;
		}
		private byte[] Split(string value)
		{
			string[] arr1 = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			byte[] buffer = new byte[arr1.Length];
			for (int i = 0; i < arr1.Length; i++) buffer[i] = Convert.ToByte(arr1[i].Trim());
			return buffer;
		}

		#endregion
	}
}