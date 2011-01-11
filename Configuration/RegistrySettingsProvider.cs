using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Adenson.Log;
using Adenson.ListenQuest.Converters;
using Microsoft.Win32;

namespace Adenson.ListenQuest.Configuration
{
	public sealed class RegistrySettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		#region Variables
		private string previousVersionKeyName;
		#endregion
		#region Constructor

		public RegistrySettingsProvider() : this(Util.Version)
		{
		}
		public RegistrySettingsProvider(Version version)
		{
			this.SettingKey = RegistrySettingsProvider.GetSettingsKey(version);
		}

		#endregion
		#region Properties

		public string SettingKey
		{
			get;
			private set;
		}
		public bool IsReadOnly
		{
			get;
			internal set;
		}
		public override string ApplicationName
		{
			get { return Strings.ApplicationName; }
			set { }
		}
		
		public static string ApplicationKey
		{
			get
			{
				string key = String.Format("Software\\{0}\\{1}", Strings.CompanyName, Strings.ApplicationName);
				#if DEBUG
				key += "Debug";
				#endif
				return key;
			}
		}

		#endregion
		#region Methods

		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			throw new NotImplementedException();
		}
		public void Reset(SettingsContext context)
		{
			throw new NotImplementedException();
		}
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			if (this.IsReadOnly) throw new InvalidOperationException(Exceptions.ReadOnlyInstance);
			SettingsPropertyValueCollection spvc = this.GetPreviousSettings(context, properties);
			if (spvc.Count > 0) this.SetPropertyValues(context, spvc);
		}
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
		{
			SettingsPropertyValueCollection settingValues = new SettingsPropertyValueCollection();
			SettingsProperty isNewProperty = collection.Cast<SettingsProperty>().FirstOrDefault(sp => sp.Attributes.Values.Cast<Attribute>().Any(f => f.GetType() == typeof(AppIsNewSettingAttribute)));

			if (isNewProperty != null)
			{
				bool isApplicationScoped = isNewProperty.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				RegistryKey registryKey = isApplicationScoped ? Registry.LocalMachine.OpenSubKey(RegistrySettingsProvider.ApplicationKey) : Registry.CurrentUser.OpenSubKey(RegistrySettingsProvider.ApplicationKey);
				settingValues.Add(new SettingsPropertyValue(isNewProperty) { Deserialized = true, IsDirty = false, PropertyValue = (registryKey == null) });
			}

			foreach (SettingsProperty setting in collection)
			{
				if (setting == isNewProperty) continue;
				settingValues.Add(this.GetSettingsValue(setting));
			}

			return settingValues;
		}
		public override void Initialize(string name, NameValueCollection config)
		{
			base.Initialize(this.ApplicationName, config);
		}
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
		{
			foreach (SettingsPropertyValue setting in collection)
			{
				SettingReadOnlyAttribute settingReadOnlyAttrib = setting.Property.Attributes.Values.OfType<SettingReadOnlyAttribute>().FirstOrDefault();
				if (settingReadOnlyAttrib != null) continue;

				bool isApplicationScoped = setting.Property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				if (isApplicationScoped) throw new InvalidOperationException();

				RegistryKey settingKey = this.GetSettingRegistryKey(false, true);

				RegistrySubKeyAttribute registrySubKeyAttrib = setting.Property.Attributes.Values.OfType<RegistrySubKeyAttribute>().FirstOrDefault();
				SystemSettingBaseAttribute systemSettingAttrib = setting.Property.Attributes.Values.OfType<SystemSettingBaseAttribute>().SingleOrDefault();
				OldSettingAttribute oldSetting = setting.Property.Attributes.Values.OfType<OldSettingAttribute>().FirstOrDefault();//SystemSettingBaseAttribute does not allow multiples

				if (registrySubKeyAttrib != null && systemSettingAttrib != null) throw new InvalidOperationException(Exceptions.RegistrySubKeyAndSystemSettingAttributeUsedTogether);

				if (oldSetting != null) settingKey.DeleteValue(oldSetting.OldKey, false);

				if (registrySubKeyAttrib != null)
				{
					Hashtable<string, NameObjectDictionary> hash = (Hashtable<string, NameObjectDictionary>)setting.PropertyValue;
				}
				else if (systemSettingAttrib != null)
				{
					#region System Setting Attributed Value
					TypeConverter typeConverter = TypeDescriptor.GetConverter(setting.Property.PropertyType);
					MergedNameObjectDictionary hash = setting.PropertyValue as MergedNameObjectDictionary;
					if (hash == null)
					{
						Type type = typeof(MergedNameObjectDictionary);
						if (typeConverter != null && typeConverter.CanConvertTo(type)) hash = (MergedNameObjectDictionary)typeConverter.ConvertTo(setting.PropertyValue, type);
					}

					if (hash == null) continue;

					RegistryKey subSettingKey = settingKey.CreateSubKey(systemSettingAttrib.SettingName);
					System.Collections.Generic.List<string> existing = new System.Collections.Generic.List<string>(subSettingKey.GetSubKeyNames());
					foreach (string hashKey in hash.Slave.Keys)
					{
						existing.Remove(hashKey);
						RegistryKey subSubSettingKey = subSettingKey.CreateSubKey(hashKey);
						foreach (string subHashKey in hash.Slave[hashKey].Keys)
						{
							subSubSettingKey.SetValue(subHashKey.Equals("Default") ? null : subHashKey, hash.Slave[hashKey][subHashKey]);
						}
					}
					//kill the remaining, we deleted them
					foreach (string hashKey in existing) subSettingKey.DeleteSubKey(hashKey, false);
					#endregion
				}
				else
				{
					#region Default Value Setting
					RegistryValueKind registryKind = RegistryValueKind.String;
					object registryValue = setting.PropertyValue;

					if (setting.Property.SerializeAs == SettingsSerializeAs.Binary)
					{
						registryValue = setting.SerializedValue;
						registryKind = RegistryValueKind.Binary;
					}
					else
					{
						TypeConverter typeConverter = TypeDescriptor.GetConverter(setting.Property.PropertyType);
						IListToArrayTypeConverter converter = typeConverter as IListToArrayTypeConverter;
						if (setting.Property.PropertyType == typeof(bool) || setting.Property.PropertyType == typeof(int)) registryKind = RegistryValueKind.DWord;
						else if (setting.PropertyValue != null)
						{
							if (converter != null)
							{
								registryKind = RegistryValueKind.MultiString;
								registryValue = converter.ConvertTo(setting.PropertyValue);
							}
							else if (typeConverter.CanConvertTo(typeof(String))) registryValue = typeConverter.ConvertTo(setting.PropertyValue, typeof(String));
							else
							{
							}
						}
					}

					if (registryValue == null) settingKey.DeleteValue(setting.Name, false);
					else settingKey.SetValue(setting.Name, registryValue, registryKind);

					#endregion
				}
			}
		}

		private Hashtable<string, NameObjectDictionary> GetHashtableValues(RegistryKey settingKey, string subKeyName)
		{
			Hashtable<string, NameObjectDictionary> result = null;
			RegistryKey key = settingKey == null ? null : settingKey.OpenSubKey(subKeyName);
			if (key != null)
			{
				result = new Hashtable<string, NameObjectDictionary>();
				foreach (string keyName in key.GetSubKeyNames())
				{
					NameObjectDictionary subhash = new NameObjectDictionary();
					RegistryKey subKey = key.OpenSubKey(keyName);
					foreach (string valueName in subKey.GetValueNames())
					{
						subhash.Add(Util.IsNullOrEmpty(valueName) ? "Default" : valueName, subKey.GetValue(valueName));
					}
					result.Add(keyName, subhash);
				}
			}

			return result;
		}
		private SettingsPropertyValue GetSettingsValue(SettingsProperty setting)
		{
			RegistrySubKeyAttribute registrySubKeyAttrib = setting.Attributes.Values.OfType<RegistrySubKeyAttribute>().FirstOrDefault();//RegistrySubKeyAttribute does not allow multiples
			SystemSettingBaseAttribute systemSettingAttrib = setting.Attributes.Values.OfType<SystemSettingBaseAttribute>().FirstOrDefault();//SystemSettingBaseAttribute does not allow multiples
			OldSettingAttribute oldSetting = setting.Attributes.Values.OfType<OldSettingAttribute>().FirstOrDefault();//SystemSettingBaseAttribute does not allow multiples

			bool isApplicationScoped = setting.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;

			bool tryConvert = true;
			RegistryKey settingRegistryKey = this.GetSettingRegistryKey(isApplicationScoped, false);
			object registryValue = settingRegistryKey == null ? null : settingRegistryKey.GetValue(setting.Name);
			if (registryValue == null && oldSetting != null && settingRegistryKey != null) registryValue = settingRegistryKey.GetValue(oldSetting.OldKey);

			if (isApplicationScoped && systemSettingAttrib != null) throw new InvalidOperationException(Exceptions.SystemSettingBaseAttributeCantBeAppScoped);
			if (registrySubKeyAttrib != null && systemSettingAttrib != null) throw new InvalidOperationException(Exceptions.RegistrySubKeyAndSystemSettingAttributeUsedTogether);

			if (registrySubKeyAttrib != null)
			{
				if (registrySubKeyAttrib.LeaveSerialized) tryConvert = false;//So that it is not serialized
				registryValue = this.GetHashtableValues(settingRegistryKey, registrySubKeyAttrib.Key);
			}
			else if (systemSettingAttrib != null)
			{
				MergedNameObjectDictionary merged = new MergedNameObjectDictionary();
				merged.SetMaster((Hashtable<string, NameObjectDictionary>)systemSettingAttrib.SystemValue);
				Hashtable<string, NameObjectDictionary> currValue = this.GetHashtableValues(settingRegistryKey, systemSettingAttrib.SettingName);
				if (currValue != null) merged.SetSlave(currValue);
				registryValue = merged;
				tryConvert = false;
			}

			if (registryValue == null) registryValue = setting.DefaultValue;

			SettingsPropertyValue settingValue = new SettingsPropertyValue(setting) { IsDirty = false };

			object result;
			if (tryConvert) Util.TryConvert(registryValue, setting.PropertyType, out result);
			else result = registryValue;
			if (result != null)
			{
				settingValue.PropertyValue = result;
				settingValue.Deserialized = true;
			}
			else
			{
				settingValue.SerializedValue = registryValue;
				settingValue.Deserialized = false;
			}

			return settingValue;
		}
		private SettingsPropertyValueCollection GetPreviousSettings(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection previousSettings = null;
			RegistryKey registryKey = ((Type)context["SettingsClassType"] == typeof(GlobalSettings) ? Registry.LocalMachine : Registry.CurrentUser).OpenSubKey(RegistrySettingsProvider.ApplicationKey);
			bool upgradeNeeded = registryKey == null ? false : (registryKey.GetSubKeyNames().SingleOrDefault(s => s == this.SettingKey) == null);
			if (upgradeNeeded)
			{
				string[] names = registryKey.GetSubKeyNames();
				int index = Array.IndexOf<string>(names, this.SettingKey);
				if (index > 0) previousVersionKeyName = names[index - 1];
				else if (names.Length > 0) previousVersionKeyName = names[names.Length - 1];
				if (previousVersionKeyName == this.SettingKey) previousVersionKeyName = null;

				if (!Util.IsNullOrEmpty(previousVersionKeyName))
				{
					RegistrySettingsProvider rsp = new RegistrySettingsProvider(new Version(previousVersionKeyName));
					rsp.Initialize(this.ApplicationName, null);
					previousSettings = new SettingsPropertyValueCollection();
					foreach (SettingsProperty setting in properties)
					{
						try
						{
							previousSettings.Add(rsp.GetSettingsValue(setting));
						}
						catch (Exception ex)
						{
							Logger.Error(this.GetType(), ex);
						}
					}
				}
			}

			if (previousSettings == null) previousSettings = new SettingsPropertyValueCollection();

			return previousSettings;
		}
		private RegistryKey GetSettingRegistryKey(bool isApplicationScoped, bool createIfNotExist)
		{
			RegistryKey registryKey = (isApplicationScoped ? Registry.LocalMachine : Registry.CurrentUser).OpenSubKey(RegistrySettingsProvider.ApplicationKey, createIfNotExist);
			RegistryKey result = null;
			if (isApplicationScoped)
			{
				if (registryKey == null) throw new NullReferenceException(String.Format(Exceptions.RegistryKeyArgNotFound, String.Concat(Registry.LocalMachine, "\\", RegistrySettingsProvider.ApplicationKey)));
				result = registryKey.OpenSubKey(this.SettingKey);
				if (result == null) throw new NullReferenceException(String.Format(Exceptions.RegistryKeyArgNotFound, String.Concat(registryKey, "\\", this.SettingKey)));
			}
			else
			{
				if (registryKey == null && createIfNotExist) registryKey = Registry.CurrentUser.CreateSubKey(RegistrySettingsProvider.ApplicationKey);
				if (registryKey != null) result = registryKey.OpenSubKey(this.SettingKey, true);
				if (result == null && createIfNotExist) result = registryKey.CreateSubKey(this.SettingKey);
			}
			return result;
		}

		#endregion
		#region Static Methods

		public static string GetSettingsKey()
		{
			return RegistrySettingsProvider.GetSettingsKey(Util.Version);
		}
		public static string GetSettingsKey(Version version)
		{
			return String.Format("{0}.{1}", version.Major, version.Minor);
		}

		#endregion
	}
}