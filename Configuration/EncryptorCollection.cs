using System;
using System.Configuration;
using Adenson.Cryptography;

namespace Adenson.Configuration
{
	internal sealed class EncryptorCollection : ConfigurationElementCollection
	{
		#region Constructors

		public EncryptorCollection()
		{
			EncryptorElement element = new EncryptorElement();
			element.Name = "Default";
			element.EncryptorType = EncryptorType.AES;
			element.LockItem = true; //to prevent removal using <remove name="Default" in the application configuration

			this.BaseAdd(element);
		}

		#endregion
		#region Properties

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}
		public EncryptorElement this[int index]
		{
			get { return (EncryptorElement)this.BaseGet(index); }
			set
			{
				if (this.BaseGet(index) != null) this.BaseRemoveAt(index);
				this.BaseAdd(index, value);
			}
		}
		protected override bool ThrowOnDuplicate
		{
			get { return true; }
		}
		protected override string ElementName
		{
			get { return "EncryptorElement"; }
		}

		#endregion
		#region Methods

		public bool ContainsKey(string key)
		{
			foreach (string str in this.BaseGetAllKeys())
			{
				if (str.Equals(key, StringComparison.CurrentCultureIgnoreCase)) return true;
			}
			return false;
		}
		internal new void BaseAdd(ConfigurationElement element)
		{
			base.BaseAdd(element);
		}
		internal new void BaseAdd(int index, ConfigurationElement element)
		{
			base.BaseAdd(index, element);
		}
		protected override ConfigurationElement CreateNewElement()
		{
			return new EncryptorElement();
		}
		protected override ConfigurationElement CreateNewElement(string elementName)
		{
			EncryptorElement element = new EncryptorElement();
			element.Name = elementName;
			return element;
		}
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((EncryptorElement)element).Name;
		}

		#endregion
	}
}