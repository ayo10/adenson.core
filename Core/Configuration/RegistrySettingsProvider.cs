using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Adenson.Configuration
{
	/// <summary>
	/// Provides registry based application settings
	/// </summary>
	public sealed class RegistrySettingsProvider : ApplicationSettingsProvider
	{
		#region Variables
		private string previousVersionKeyName;
		private string _settingKey;
		private string _applicationKey;
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

		private string ApplicationKey
		{
			get
			{
				if (_applicationKey == null)
				{
					_applicationKey = StringUtil.Format("Software\\{0}\\{1}", this.CompanyName, this.ProductName);
				}

				return _applicationKey;
			}
		}

		private string SettingKey
		{
			get
			{
				if (_settingKey == null)
				{
					_settingKey = StringUtil.Format("{0}.{1}", this.Version.Major, this.Version.Minor);
				}

				return _settingKey;
			}
		}

		#endregion
		#region Methods

		/// <summary>
		/// Returns the value of the specified settings property for the previous version of the same application.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		/// <param name="property">The SettingsProperty whose value is to be returned.</param>
		/// <returns> A SettingsPropertyValue containing the value of thes pecified property setting as it was last set in the previous version of the application; or null if the setting cannot be found.</returns>
		public override SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			return new SettingsPropertyValue(property);
		}

		/// <summary>
		/// Returns the collection of settings property values for the specified application instance and settings property group.
		/// </summary>
		/// <param name="context">A System.Configuration.SettingsContext describing the current application use.</param>
		/// <param name="collection">A System.Configuration.SettingsPropertyCollection containing the settings property group whose values are to be retrieved.</param>
		/// <returns>A System.Configuration.SettingsPropertyValueCollection containing the values for the specified settings property group.</returns>
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
		{
			Arg.IsNotNull(collection);
			SettingsPropertyValueCollection settingValues = new SettingsPropertyValueCollection();
			foreach (SettingsProperty setting in collection)
			{
				settingValues.Add(this.GetSettingsValue(setting));
			}

			return settingValues;
		}

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			if (String.IsNullOrEmpty(name))
			{
				name = "RegistrySettingsProvider";
			}

			base.Initialize(name, config);
		}

		/// <summary>
		///  Resets the application settings associated with the specified application to their default values.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		public override void Reset(SettingsContext context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the values of the specified group of property settings.
		/// </summary>
		/// <param name="context">A System.Configuration.SettingsContext describing the current application usage.</param>
		/// <param name="collection">A System.Configuration.SettingsPropertyValueCollection representing the group of property settings to set.</param>
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
		{
			Arg.IsNotNull(collection, "collection");

			foreach (SettingsPropertyValue setting in collection)
			{
				bool isApplicationScoped = setting.Property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				if (isApplicationScoped)
				{
					throw new InvalidOperationException();
				}

				RegistryKey settingKey = this.GetSettingRegistryKey(true);
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
					if (setting.Property.PropertyType == typeof(bool) || setting.Property.PropertyType == typeof(int))
					{
						registryKind = RegistryValueKind.DWord;
					}
					else if (setting.PropertyValue != null)
					{
						if (typeConverter.CanConvertTo(typeof(string)))
						{
							registryValue = typeConverter.ConvertTo(setting.PropertyValue, typeof(string));
						}
					}
				}

				if (registryValue == null)
				{
					settingKey.DeleteValue(setting.Name, false);
				}
				else
				{
					settingKey.SetValue(setting.Name, registryValue, registryKind);
				}
			}
		}

		/// <summary>
		/// Indicates to the provider that the application has been upgraded. This offers the provider an opportunity to upgrade its stored settings as appropriate.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		/// <param name="properties">A SettingsPropertyCollection containing the settings property group whose values are to be retrieved.</param>
		public override void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection spvc = this.GetPreviousSettings(properties);
			if (spvc.Count > 0)
			{
				this.SetPropertyValues(context, spvc);
			}
		}

		private SettingsPropertyValue GetSettingsValue(SettingsProperty setting)
		{
			bool isApplicationScoped = setting.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
			if (isApplicationScoped)
			{
				throw new InvalidOperationException();
			}

			bool tryConvert = true;
			RegistryKey settingRegistryKey = this.GetSettingRegistryKey(false);
			object registryValue = settingRegistryKey == null ? null : settingRegistryKey.GetValue(setting.Name);

			if (registryValue == null)
			{
				registryValue = setting.DefaultValue;
			}

			SettingsPropertyValue settingValue = new SettingsPropertyValue(setting) { IsDirty = false };

			object result;
			if (tryConvert)
			{
				TypeUtil.TryConvert(setting.PropertyType, registryValue, out result);
			}
			else
			{
				result = registryValue;
			}

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

		private SettingsPropertyValueCollection GetPreviousSettings(SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection previousSettings = null;
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this.ApplicationKey);
			bool upgradeNeeded = registryKey == null ? false : (registryKey.GetSubKeyNames().SingleOrDefault(s => s == this.SettingKey) == null);
			if (upgradeNeeded)
			{
				string[] names = registryKey.GetSubKeyNames();
				int index = Array.IndexOf<string>(names, this.SettingKey);
				if (index > 0)
				{
					previousVersionKeyName = names[index - 1];
				}
				else if (names.Length > 0)
				{
					previousVersionKeyName = names[names.Length - 1];
				}

				if (previousVersionKeyName == this.SettingKey)
				{
					previousVersionKeyName = null;
				}

				if (!StringUtil.IsNullOrWhiteSpace(previousVersionKeyName))
				{
					RegistrySettingsProvider rsp = new RegistrySettingsProvider();
					rsp.Version = new Version(previousVersionKeyName);
					rsp.Initialize(this.ApplicationName, null);
					previousSettings = new SettingsPropertyValueCollection();
					foreach (SettingsProperty setting in properties)
					{
						previousSettings.Add(rsp.GetSettingsValue(setting));
					}
				}
			}

			if (previousSettings == null)
			{
				previousSettings = new SettingsPropertyValueCollection();
			}

			return previousSettings;
		}

		private RegistryKey GetSettingRegistryKey(bool createIfNotExist)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this.ApplicationKey, createIfNotExist);
			RegistryKey result = null;
			if (registryKey == null && createIfNotExist)
			{
				registryKey = Registry.CurrentUser.CreateSubKey(this.ApplicationKey);
			}

			if (registryKey != null)
			{
				result = registryKey.OpenSubKey(this.SettingKey, true);
			}

			if (result == null && createIfNotExist)
			{
				result = registryKey.CreateSubKey(this.SettingKey);
			}

			return result;
		}

		#endregion
	}
}