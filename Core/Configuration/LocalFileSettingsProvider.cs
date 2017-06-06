using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace Adenson.Configuration
{
	/// <summary>
	/// Provides local file based application settings
	/// </summary>
	public class LocalFileSettingsProvider : ApplicationSettingsProvider
	{
		#region Constants
		private const string ConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
	<configSections>
		<sectionGroup name=""userSettings"" type=""{2}"">
			<section name=""{0}"" type=""{3}"" allowExeDefinition=""MachineToLocalUser"" requirePermission=""false"" />
		</sectionGroup>
	</configSections>
	<userSettings>
		<{1}>
		</{1}>
	</userSettings>
</configuration>";
		#endregion
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalFileSettingsProvider"/> class.
		/// </summary>
		public LocalFileSettingsProvider()
		{
			this.IgnoreVersion = true;
		}

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

		/// <summary>
		/// Gets or sets a value indicating whether to ignore version or not.
		/// </summary>
		protected bool IgnoreVersion
		{
			get;
			set;
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
			Arg.IsNotNull(collection, "collection");

			var config = this.GetConfiguration(context);
			var sectionName = LocalFileSettingsProvider.GetSectionName(context);
			ClientSettingsSection userSettings = config.GetSection("userSettings/" + sectionName) as ClientSettingsSection;
			SettingsPropertyValueCollection settingValues = new SettingsPropertyValueCollection();
			foreach (SettingsProperty property in collection)
			{
				bool isApplicationScoped = property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				SettingsPropertyValue settingsValue = new SettingsPropertyValue(property);
				if (isApplicationScoped)
				{
					if (property.DefaultValue != null)
					{
						settingsValue.SerializedValue = property.DefaultValue;
					}
				}
				else
				{
					var element = userSettings == null ? null : userSettings.Settings.Get(property.Name);
					if (element != null)
					{
						if (element.SerializeAs == SettingsSerializeAs.String)
						{
							settingsValue.SerializedValue = element.Value.ValueXml.InnerText;
						}
						else if (element.SerializeAs == SettingsSerializeAs.Xml)
						{
							settingsValue.SerializedValue = element.Value.ValueXml.InnerXml;
						}
						else
						{
							settingsValue.SerializedValue = element.Value.ValueXml;
						}
					}
					else if (property.DefaultValue != null)
					{
						settingsValue.SerializedValue = property.DefaultValue;
					}
					else
					{
						settingsValue.PropertyValue = null;
					}
				}

				settingsValue.IsDirty = false;
				settingValues.Add(settingsValue);
			}

			return settingValues;
		}

		/// <summary>
		/// Initializes the provider, setting .
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			if (String.IsNullOrEmpty(name))
			{
				name = "LocalFileSettingsProvider";
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
			Arg.IsNotNull(collection);
			var config = this.GetConfiguration(context, true);
			var sectionName = LocalFileSettingsProvider.GetSectionName(context);
			ClientSettingsSection userSettings = config.GetSection("userSettings/" + sectionName) as ClientSettingsSection;
			foreach (SettingsPropertyValue value in collection)
			{
				bool isApplicationScoped = value.Property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				if (isApplicationScoped)
				{
					continue;
				}

				if (userSettings != null)
				{
					SettingElementCollection settings = userSettings.Settings;
					SettingElement element = userSettings.Settings.Get(value.Property.Name);
					if (element == null)
					{
						element = new SettingElement { Name = value.Property.Name };
						settings.Add(element);
					}

					element.SerializeAs = value.Property.SerializeAs;
					element.Value.ValueXml = ConvertToXmlElement(value.Property, value);
				}
			}

			try
			{
				config.Save();
				return;
			}
			catch (ConfigurationErrorsException exception)
			{
				throw new ConfigurationErrorsException("SettingsSaveFailed", exception);
			}
		}

		/// <summary>
		/// Indicates to the provider that the application has been upgraded. This offers the provider an opportunity to upgrade its stored settings as appropriate.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		/// <param name="properties">A SettingsPropertyCollection containing the settings property group whose values are to be retrieved.</param>
		public override void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			throw new NotImplementedException();
		}

		private static XmlNode ConvertToXmlElement(SettingsProperty setting, SettingsPropertyValue value)
		{
			XmlElement element = new XmlDocument().CreateElement("value");
			string serializedValue = value.SerializedValue as string;
			if ((serializedValue == null) && (setting.SerializeAs == SettingsSerializeAs.Binary))
			{
				byte[] inArray = value.SerializedValue as byte[];
				if (inArray != null)
				{
					serializedValue = Convert.ToBase64String(inArray);
				}
			}

			if (serializedValue == null)
			{
				serializedValue = String.Empty;
			}

			element.InnerXml = serializedValue;
			XmlNode declaration = element.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.NodeType == XmlNodeType.XmlDeclaration);
			if (declaration != null)
			{
				element.RemoveChild(declaration);
			}

			return element;
		}

		private System.Configuration.Configuration GetConfiguration(SettingsContext context)
		{
			return this.GetConfiguration(context, false);
		}

		private System.Configuration.Configuration GetConfiguration(SettingsContext context, bool create)
		{
			var applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string configFilePath = Path.Combine(Path.Combine(applicationDataPath, this.CompanyName), this.ProductName);
			if (!this.IgnoreVersion)
			{
				configFilePath = Path.Combine(configFilePath, this.Version.Major + "." + this.Version.Minor);
			}

			ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
			filemap.ExeConfigFilename = Path.Combine(configFilePath, "user.config");

			if (create && !File.Exists(filemap.ExeConfigFilename))
			{
				if (!Directory.Exists(configFilePath))
				{
					Directory.CreateDirectory(configFilePath);
				}

				var sectionName = LocalFileSettingsProvider.GetSectionName(context);
				File.WriteAllText(filemap.ExeConfigFilename, StringUtil.Format(LocalFileSettingsProvider.ConfigContent, sectionName, sectionName, typeof(UserSettingsGroup).AssemblyQualifiedName, typeof(ClientSettingsSection).AssemblyQualifiedName));
			}

			var config = ConfigurationManager.OpenMappedExeConfiguration(filemap, System.Configuration.ConfigurationUserLevel.None);
			if (create)
			{
				config.Save();
			}

			return config;
		}

		#endregion
	}
}