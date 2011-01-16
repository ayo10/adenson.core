using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using Microsoft.Win32;

namespace Adenson.Configuration
{
	public sealed class RegistrySettingsProvider : ApplicationSettingsProvider, IApplicationSettingsProvider
	{
		#region Variables
		private string previousVersionKeyName;
		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the name of the currently running application.
		/// </summary>
		public override string ApplicationName
		{
			get;
			set;
		}

		private string SettingKey
		{
			get;
			set;
		}
		private string ApplicationKey
		{
			get { return String.Format("Software\\{0}\\{1}", this.CompanyName, this.ProductName); }
		}

		#endregion
		#region Methods

		public override SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			throw new NotImplementedException();
		}
		public override void Reset(SettingsContext context)
		{
			throw new NotImplementedException();
		}
		public override void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection spvc = this.GetPreviousSettings(context, properties);
			if (spvc.Count > 0) this.SetPropertyValues(context, spvc);
		}
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
		{
			SettingsPropertyValueCollection settingValues = new SettingsPropertyValueCollection();
			foreach (SettingsProperty setting in collection) settingValues.Add(this.GetSettingsValue(setting));
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
				bool isApplicationScoped = setting.Property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				if (isApplicationScoped) throw new InvalidOperationException();

				RegistryKey settingKey = this.GetSettingRegistryKey(false, true);

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
					if (setting.Property.PropertyType == typeof(bool) || setting.Property.PropertyType == typeof(int)) registryKind = RegistryValueKind.DWord;
					else if (setting.PropertyValue != null)
					{
						if (typeConverter.CanConvertTo(typeof(String))) registryValue = typeConverter.ConvertTo(setting.PropertyValue, typeof(String));
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

		private SettingsPropertyValue GetSettingsValue(SettingsProperty setting)
		{
			bool isApplicationScoped = setting.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;

			bool tryConvert = true;
			RegistryKey settingRegistryKey = this.GetSettingRegistryKey(isApplicationScoped, false);
			object registryValue = settingRegistryKey == null ? null : settingRegistryKey.GetValue(setting.Name);

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
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this.ApplicationKey);
			bool upgradeNeeded = registryKey == null ? false : (registryKey.GetSubKeyNames().SingleOrDefault(s => s == this.SettingKey) == null);
			if (upgradeNeeded)
			{
				string[] names = registryKey.GetSubKeyNames();
				int index = Array.IndexOf<string>(names, this.SettingKey);
				if (index > 0) previousVersionKeyName = names[index - 1];
				else if (names.Length > 0) previousVersionKeyName = names[names.Length - 1];
				if (previousVersionKeyName == this.SettingKey) previousVersionKeyName = null;

				if (!String.IsNullOrWhiteSpace(previousVersionKeyName))
				{
					RegistrySettingsProvider rsp = new RegistrySettingsProvider();
					rsp.Version = new Version(previousVersionKeyName);
					rsp.Initialize(this.ApplicationName, null);
					previousSettings = new SettingsPropertyValueCollection();
					foreach (SettingsProperty setting in properties) previousSettings.Add(rsp.GetSettingsValue(setting));
				}
			}

			if (previousSettings == null) previousSettings = new SettingsPropertyValueCollection();

			return previousSettings;
		}
		private RegistryKey GetSettingRegistryKey(bool isApplicationScoped, bool createIfNotExist)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this.ApplicationKey, createIfNotExist);
			RegistryKey result = null;
			if (registryKey == null && createIfNotExist) registryKey = Registry.CurrentUser.CreateSubKey(this.ApplicationKey);
			if (registryKey != null) result = registryKey.OpenSubKey(this.SettingKey, true);
			if (result == null && createIfNotExist) result = registryKey.CreateSubKey(this.SettingKey);
			return result;
		}

		#endregion
	}
}