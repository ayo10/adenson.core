using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Internal;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml;

namespace Adenson.Configuration
{
	/// <summary>
	/// Provides local file based application settings
	/// </summary>
	public class LocalFileSettingsProvider : ApplicationSettingsProvider
	{
		#region Properties

		/// <summary>
		/// Gets or sets the name of the currently running application.
		/// </summary>
		public override string ApplicationName
		{
			get;
			set;
		}

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
			var config = this.GetConfiguration(context);
			var sectionName = this.GetSectionName(context);
			ClientSettingsSection appSettings = config.GetSection("applicationSettings/" + sectionName) as ClientSettingsSection;
			ClientSettingsSection userSettings = config.GetSection("userSettings/" + sectionName) as ClientSettingsSection;
			SettingsPropertyValueCollection settingValues = new SettingsPropertyValueCollection();
			foreach (SettingsProperty property in collection)
			{
				bool isApplicationScoped = property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				SettingsPropertyValue settingsValue = new SettingsPropertyValue(property);
				if (isApplicationScoped)
				{
					if (property.DefaultValue != null) settingsValue.SerializedValue = property.DefaultValue;
				}
				else
				{
					var element = userSettings == null ? null : userSettings.Settings.Get(property.Name);
					if (element != null)
					{
						settingsValue.SerializedValue = element.Value.ValueXml;
						if (element.SerializeAs == SettingsSerializeAs.String) settingsValue.SerializedValue = element.Value.ValueXml.InnerText;
					}
					else if (property.DefaultValue != null) settingsValue.SerializedValue = property.DefaultValue;
					else settingsValue.PropertyValue = null;
				}
				settingsValue.IsDirty = false;
				settingValues.Add(settingsValue);
			}
			return settingValues;
		}
		/// <summary>
		/// Initializes the provider, setting 
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="values">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection values)
		{
			if (String.IsNullOrEmpty(name)) name = "LocalFileSettingsProvider";
			base.Initialize(name, values);
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
			var config = this.GetConfiguration(context, true);
			var sectionName = this.GetSectionName(context);
			ClientSettingsSection userSettings = config.GetSection("userSettings/" + sectionName) as ClientSettingsSection;
			foreach (SettingsPropertyValue value in collection)
			{
				bool isApplicationScoped = value.Property.Attributes.Values.OfType<ApplicationScopedSettingAttribute>().Count() > 0;
				if (isApplicationScoped) continue;

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

		private XmlNode ConvertToXmlElement(SettingsProperty setting, SettingsPropertyValue value)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement element = doc.CreateElement("value");
			string serializedValue = value.SerializedValue as string;
			if ((serializedValue == null) && (setting.SerializeAs == SettingsSerializeAs.Binary))
			{
				byte[] inArray = value.SerializedValue as byte[];
				if (inArray != null) serializedValue = Convert.ToBase64String(inArray);
			}

			if (serializedValue == null) serializedValue = String.Empty;

			element.InnerXml = serializedValue;
			XmlNode oldChild = null;
			foreach (XmlNode node in element.ChildNodes)
			{
				if (node.NodeType == XmlNodeType.XmlDeclaration)
				{
					oldChild = node;
					break;
				}
			}
			if (oldChild != null) element.RemoveChild(oldChild);
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
			if (!this.IgnoreVersion) configFilePath = Path.Combine(configFilePath, (this.Version.Major + "." + this.Version.Minor));
			ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
			filemap.ExeConfigFilename = Path.Combine(configFilePath, "user.config");

			if (create && !File.Exists(filemap.ExeConfigFilename))
			{
				if (!Directory.Exists(configFilePath)) Directory.CreateDirectory(configFilePath);
				File.WriteAllText(filemap.ExeConfigFilename, String.Format(LocalFileSettingsProvider.ConfigContent, this.GetSectionName(context), this.GetSectionName(context), typeof(UserSettingsGroup).AssemblyQualifiedName, typeof(ClientSettingsSection).AssemblyQualifiedName));
			}

			var config = ConfigurationManager.OpenMappedExeConfiguration(filemap, System.Configuration.ConfigurationUserLevel.None);
			if (create) config.Save();
			return config;
		}
		private string GetSectionName(SettingsContext context)
		{
			string groupName = (string)context["GroupName"];
			string settingsKey = (string)context["SettingsKey"];
			string name = groupName;
			if (!String.IsNullOrEmpty(settingsKey)) name = String.Format("{0}.{1}", new object[] { name, settingsKey });
			return XmlConvert.EncodeLocalName(name);
		}

		#endregion
		#region Inner Classes

		/// <summary>
		/// LocalFileSettingsProvider that does not use versioning in the config file name. Does not support upgrade either.
		/// </summary>
		public sealed class NoVersionProvider : LocalFileSettingsProvider
		{
			/// <summary>
			/// Initializes a new provider
			/// </summary>
			public NoVersionProvider()
			{
				this.IgnoreVersion = true;
			}
			/// <summary>
			/// With IgnoreVersion set to false, there is no unique lookup path, thus, nothing can be done
			/// </summary>
			/// <param name="context">A SettingsContext describing the current application usage.</param>
			/// <param name="properties">A SettingsPropertyCollection containing the settings property group whose values are to be retrieved.</param>
			/// <exception cref="NotSupportedException">NoVersionProvider does not support upgrade</exception>
			public override void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
			{
				throw new NotSupportedException();
			}
		}

		#endregion
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
	}
}