using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Internal;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Xml;
using System.Security.Permissions;
using System.Globalization;

namespace Adenson.Configuration
{
	/// <summary>
	/// Extended <see cref="System.Configuration.LocalFileSettingsProvider"/> that does nothing, yet
	/// </summary>
	public class LocalFileSettingsProvider2 : System.Configuration.LocalFileSettingsProvider
	{
	}
	public class LocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		#region Variables
		private string _appName = string.Empty;
		private XmlEscaper _escaper;
		private string _prevLocalConfigFileName;
		private string _prevRoamingConfigFileName;
		private ClientSettingsStore _store;
		#endregion
		#region Methods

		private Version CreateVersion(string name)
		{
			try
			{
				return new Version(name);
			}
			catch (ArgumentException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			catch (FormatException)
			{
				return null;
			}
		}
		private string GetPreviousConfigFileName(bool isRoaming)
		{
			if (!ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
			{
				throw new ConfigurationErrorsException("UserSettingsNotSupported");
			}
			string str = isRoaming ? this._prevRoamingConfigFileName : this._prevLocalConfigFileName;
			if (string.IsNullOrEmpty(str))
			{
				string path = isRoaming ? ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigDirectory : ConfigurationManagerInternalFactory.Instance.ExeLocalConfigDirectory;
				Version version = this.CreateVersion(ConfigurationManagerInternalFactory.Instance.ExeProductVersion);
				Version version2 = null;
				DirectoryInfo info = null;
				string str3 = null;
				if (version == null)
				{
					return null;
				}
				DirectoryInfo parent = Directory.GetParent(path);
				if (parent.Exists)
				{
					foreach (DirectoryInfo info3 in parent.GetDirectories())
					{
						Version version3 = this.CreateVersion(info3.Name);
						if ((version3 != null) && (version3 < version))
						{
							if (version2 == null)
							{
								version2 = version3;
								info = info3;
							}
							else if (version3 > version2)
							{
								version2 = version3;
								info = info3;
							}
						}
					}
					if (info != null)
					{
						str3 = Path.Combine(info.FullName, ConfigurationManagerInternalFactory.Instance.UserConfigFilename);
					}
					if (File.Exists(str3))
					{
						str = str3;
					}
				}
				if (isRoaming)
				{
					this._prevRoamingConfigFileName = str;
					return str;
				}
				this._prevLocalConfigFileName = str;
			}
			return str;
		}
		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			bool isRoaming = IsRoamingSetting(property);
			string previousConfigFileName = this.GetPreviousConfigFileName(isRoaming);
			if (!string.IsNullOrEmpty(previousConfigFileName))
			{
				SettingsPropertyCollection properties = new SettingsPropertyCollection();
				properties.Add(property);
				return this.GetSettingValuesFromFile(previousConfigFileName, this.GetSectionName(context), true, properties)[property.Name];
			}
			return new SettingsPropertyValue(property) { PropertyValue = null };
		}
		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
			string sectionName = this.GetSectionName(context);
			IDictionary dictionary = this.Store.ReadSettings(sectionName, false);
			IDictionary dictionary2 = this.Store.ReadSettings(sectionName, true);
			ConnectionStringSettingsCollection settingss = this.Store.ReadConnectionStrings();
			foreach (SettingsProperty property in properties)
			{
				string name = property.Name;
				SettingsPropertyValue value2 = new SettingsPropertyValue(property);
				SpecialSettingAttribute attribute = property.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute;
				if ((attribute != null) ? (attribute.SpecialSetting == SpecialSetting.ConnectionString) : false)
				{
					string str3 = sectionName + "." + name;
					if ((settingss != null) && (settingss[str3] != null))
					{
						value2.PropertyValue = settingss[str3].ConnectionString;
					}
					else if ((property.DefaultValue != null) && (property.DefaultValue is string))
					{
						value2.PropertyValue = property.DefaultValue;
					}
					else
					{
						value2.PropertyValue = string.Empty;
					}
					value2.IsDirty = false;
					values.Add(value2);
					continue;
				}
				bool flag2 = this.IsUserSetting(property);
				if (flag2 && !ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
				{
					throw new ConfigurationErrorsException("UserSettingsNotSupported");
				}
				IDictionary dictionary3 = flag2 ? dictionary2 : dictionary;
				if (dictionary3.Contains(name))
				{
					StoredSetting setting = (StoredSetting)dictionary3[name];
					string innerXml = setting.Value.InnerXml;
					if (setting.SerializeAs == SettingsSerializeAs.String)
					{
						innerXml = this.Escaper.Unescape(innerXml);
					}
					value2.SerializedValue = innerXml;
				}
				else if (property.DefaultValue != null)
				{
					value2.SerializedValue = property.DefaultValue;
				}
				else
				{
					value2.PropertyValue = null;
				}
				value2.IsDirty = false;
				values.Add(value2);
			}
			return values;
		}
		private string GetSectionName(SettingsContext context)
		{
			string str = (string)context["GroupName"];
			string str2 = (string)context["SettingsKey"];
			string name = str;
			if (!string.IsNullOrEmpty(str2))
			{
				name = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { name, str2 });
			}
			return XmlConvert.EncodeLocalName(name);
		}
		private SettingsPropertyValueCollection GetSettingValuesFromFile(string configFileName, string sectionName, bool userScoped, SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
			IDictionary dictionary = ClientSettingsStore.ReadSettingsFromFile(configFileName, sectionName, userScoped);
			foreach (SettingsProperty property in properties)
			{
				string name = property.Name;
				SettingsPropertyValue value2 = new SettingsPropertyValue(property);
				if (dictionary.Contains(name))
				{
					StoredSetting setting = (StoredSetting)dictionary[name];
					string innerXml = setting.Value.InnerXml;
					if (setting.SerializeAs == SettingsSerializeAs.String)
					{
						innerXml = this.Escaper.Unescape(innerXml);
					}
					value2.SerializedValue = innerXml;
					value2.IsDirty = true;
					values.Add(value2);
				}
			}
			return values;
		}
		public override void Initialize(string name, NameValueCollection values)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = "LocalFileSettingsProvider";
			}
			base.Initialize(name, values);
		}
		private static bool IsRoamingSetting(SettingsProperty setting)
		{
			bool flag = !LocalFileSettingsProvider.IsClickOnceDeployed(AppDomain.CurrentDomain);
			bool flag2 = false;
			if (flag)
			{
				SettingsManageabilityAttribute attribute = setting.Attributes[typeof(SettingsManageabilityAttribute)] as SettingsManageabilityAttribute;
				flag2 = (attribute != null) && (0 == 0);
			}
			return flag2;
		}
		private bool IsUserSetting(SettingsProperty setting)
		{
			bool flag = setting.Attributes[typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute;
			bool flag2 = setting.Attributes[typeof(ApplicationScopedSettingAttribute)] is ApplicationScopedSettingAttribute;
			if (flag && flag2)
			{
				throw new ConfigurationErrorsException("BothScopeAttributes");
			}
			if (!flag && !flag2)
			{
				throw new ConfigurationErrorsException("NoScopeAttributes");
			}
			return flag;
		}
		public void Reset(SettingsContext context)
		{
			string sectionName = this.GetSectionName(context);
			this.Store.RevertToParent(sectionName, true);
			this.Store.RevertToParent(sectionName, false);
		}
		private XmlNode SerializeToXmlElement(SettingsProperty setting, SettingsPropertyValue value)
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
				serializedValue = string.Empty;
			}
			if (setting.SerializeAs == SettingsSerializeAs.String)
			{
				serializedValue = this.Escaper.Escape(serializedValue);
			}
			element.InnerXml = serializedValue;
			XmlNode oldChild = null;
			foreach (XmlNode node2 in element.ChildNodes)
			{
				if (node2.NodeType == XmlNodeType.XmlDeclaration)
				{
					oldChild = node2;
					break;
				}
			}
			if (oldChild != null)
			{
				element.RemoveChild(oldChild);
			}
			return element;
		}
		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			string sectionName = this.GetSectionName(context);
			IDictionary newSettings = new Hashtable();
			IDictionary dictionary2 = new Hashtable();
			foreach (SettingsPropertyValue value2 in values)
			{
				SettingsProperty property = value2.Property;
				bool flag = this.IsUserSetting(property);
				if (value2.IsDirty && flag)
				{
					bool flag2 = IsRoamingSetting(property);
					StoredSetting setting = new StoredSetting(property.SerializeAs, this.SerializeToXmlElement(property, value2));
					if (flag2)
					{
						newSettings[property.Name] = setting;
					}
					else
					{
						dictionary2[property.Name] = setting;
					}
					value2.IsDirty = false;
				}
			}
			if (newSettings.Count > 0)
			{
				this.Store.WriteSettings(sectionName, true, newSettings);
			}
			if (dictionary2.Count > 0)
			{
				this.Store.WriteSettings(sectionName, false, dictionary2);
			}
		}
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyCollection propertys = new SettingsPropertyCollection();
			SettingsPropertyCollection propertys2 = new SettingsPropertyCollection();
			foreach (SettingsProperty property in properties)
			{
				if (IsRoamingSetting(property))
				{
					propertys2.Add(property);
				}
				else
				{
					propertys.Add(property);
				}
			}
			if (propertys2.Count > 0)
			{
				this.Upgrade(context, propertys2, true);
			}
			if (propertys.Count > 0)
			{
				this.Upgrade(context, propertys, false);
			}
		}
		private void Upgrade(SettingsContext context, SettingsPropertyCollection properties, bool isRoaming)
		{
			string previousConfigFileName = this.GetPreviousConfigFileName(isRoaming);
			if (!string.IsNullOrEmpty(previousConfigFileName))
			{
				SettingsPropertyCollection propertys = new SettingsPropertyCollection();
				foreach (SettingsProperty property in properties)
				{
					if (!(property.Attributes[typeof(NoSettingsVersionUpgradeAttribute)] is NoSettingsVersionUpgradeAttribute))
					{
						propertys.Add(property);
					}
				}
				SettingsPropertyValueCollection collection = this.GetSettingValuesFromFile(previousConfigFileName, this.GetSectionName(context), true, propertys);
				this.SetPropertyValues(context, collection);
			}
		}
		internal static bool IsClickOnceDeployed(AppDomain appDomain)
		{
			ActivationContext activationContext = appDomain.ActivationContext;
			return (((activationContext != null) && (activationContext.Form == ActivationContext.ContextForm.StoreBounded)) && !string.IsNullOrEmpty(activationContext.Identity.FullName));
		}

		#endregion
		#region Properties

		public override string ApplicationName
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this._appName;
			}
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this._appName = value;
			}
		}
		private XmlEscaper Escaper
		{
			get
			{
				if (this._escaper == null)
				{
					this._escaper = new XmlEscaper();
				}
				return this._escaper;
			}
		}
		private ClientSettingsStore Store
		{
			get
			{
				if (this._store == null)
				{
					this._store = new ClientSettingsStore();
				}
				return this._store;
			}
		}

		#endregion
		#region Nested Types

		private class XmlEscaper
		{
			// Fields
			private XmlDocument doc = new XmlDocument();
			private XmlElement temp;

			// Methods
			internal XmlEscaper()
			{
				this.temp = this.doc.CreateElement("temp");
			}

			internal string Escape(string xmlString)
			{
				if (string.IsNullOrEmpty(xmlString))
				{
					return xmlString;
				}
				this.temp.InnerText = xmlString;
				return this.temp.InnerXml;
			}

			internal string Unescape(string escapedString)
			{
				if (string.IsNullOrEmpty(escapedString))
				{
					return escapedString;
				}
				this.temp.InnerXml = escapedString;
				return this.temp.InnerText;
			}
		}
		internal sealed class ClientSettingsStore
		{
			// Fields
			private const string ApplicationSettingsGroupName = "applicationSettings";
			private const string ApplicationSettingsGroupPrefix = "applicationSettings/";
			private const string UserSettingsGroupName = "userSettings";
			private const string UserSettingsGroupPrefix = "userSettings/";

			// Methods
			private void DeclareSection(System.Configuration.Configuration config, string sectionName)
			{
				if (config.GetSectionGroup("userSettings") == null)
				{
					ConfigurationSectionGroup group2 = new UserSettingsGroup();
					config.SectionGroups.Add("userSettings", group2);
				}
				ConfigurationSectionGroup sectionGroup = config.GetSectionGroup("userSettings");
				if ((sectionGroup != null) && (sectionGroup.Sections[sectionName] == null))
				{
					ConfigurationSection section = new ClientSettingsSection
					{
						SectionInformation = { AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser, RequirePermission = false }
					};
					sectionGroup.Sections.Add(sectionName, section);
				}
			}

			private ClientSettingsSection GetConfigSection(System.Configuration.Configuration config, string sectionName, bool declare)
			{
				string str = "userSettings/" + sectionName;
				ClientSettingsSection section = null;
				if (config != null)
				{
					section = config.GetSection(str) as ClientSettingsSection;
					if ((section == null) && declare)
					{
						this.DeclareSection(config, sectionName);
						section = config.GetSection(str) as ClientSettingsSection;
					}
				}
				return section;
			}

			private System.Configuration.Configuration GetUserConfig(bool isRoaming)
			{
				ConfigurationUserLevel userLevel = isRoaming ? ConfigurationUserLevel.PerUserRoaming : ConfigurationUserLevel.PerUserRoamingAndLocal;
				return ClientSettingsConfigurationHost.OpenExeConfiguration(userLevel);
			}

			internal ConnectionStringSettingsCollection ReadConnectionStrings()
			{
				return PrivilegedConfigurationManager.ConnectionStrings;
			}

			internal IDictionary ReadSettings(string sectionName, bool isUserScoped)
			{
				IDictionary dictionary = new Hashtable();
				if (!isUserScoped || ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
				{
					string str = isUserScoped ? "userSettings/" : "applicationSettings/";
					ConfigurationManager.RefreshSection(str + sectionName);
					ClientSettingsSection section = ConfigurationManager.GetSection(str + sectionName) as ClientSettingsSection;
					if (section == null)
					{
						return dictionary;
					}
					foreach (SettingElement element in section.Settings)
					{
						dictionary[element.Name] = new StoredSetting(element.SerializeAs, element.Value.ValueXml);
					}
				}
				return dictionary;
			}

			internal static IDictionary ReadSettingsFromFile(string configFileName, string sectionName, bool isUserScoped)
			{
				IDictionary dictionary = new Hashtable();
				if (!isUserScoped || ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
				{
					string str = isUserScoped ? "userSettings/" : "applicationSettings/";
					ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
					ConfigurationUserLevel userLevel = isUserScoped ? ConfigurationUserLevel.PerUserRoaming : ConfigurationUserLevel.None;
					if (isUserScoped)
					{
						fileMap.ExeConfigFilename = ConfigurationManagerInternalFactory.Instance.ApplicationConfigUri;
						fileMap.RoamingUserConfigFilename = configFileName;
					}
					else
					{
						fileMap.ExeConfigFilename = configFileName;
					}
					ClientSettingsSection section = ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel).GetSection(str + sectionName) as ClientSettingsSection;
					if (section == null)
					{
						return dictionary;
					}
					foreach (SettingElement element in section.Settings)
					{
						dictionary[element.Name] = new StoredSetting(element.SerializeAs, element.Value.ValueXml);
					}
				}
				return dictionary;
			}

			internal void RevertToParent(string sectionName, bool isRoaming)
			{
				if (!ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
				{
					throw new ConfigurationErrorsException("UserSettingsNotSupported");
				}
				System.Configuration.Configuration userConfig = this.GetUserConfig(isRoaming);
				ClientSettingsSection section = this.GetConfigSection(userConfig, sectionName, false);
				if (section != null)
				{
					section.SectionInformation.RevertToParent();
					userConfig.Save();
				}
			}

			internal void WriteSettings(string sectionName, bool isRoaming, IDictionary newSettings)
			{
				if (!ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
				{
					throw new ConfigurationErrorsException("UserSettingsNotSupported");
				}
				System.Configuration.Configuration userConfig = this.GetUserConfig(isRoaming);
				ClientSettingsSection section = this.GetConfigSection(userConfig, sectionName, true);
				if (section != null)
				{
					SettingElementCollection settings = section.Settings;
					foreach (DictionaryEntry entry in newSettings)
					{
						SettingElement element = settings.Get((string)entry.Key);
						if (element == null)
						{
							element = new SettingElement
							{
								Name = (string)entry.Key
							};
							settings.Add(element);
						}
						StoredSetting setting = (StoredSetting)entry.Value;
						element.SerializeAs = setting.SerializeAs;
						element.Value.ValueXml = setting.Value;
					}
					try
					{
						userConfig.Save();
						return;
					}
					catch (ConfigurationErrorsException exception)
					{
						throw new ConfigurationErrorsException("SettingsSaveFailed", exception);
					}
				}
				throw new ConfigurationErrorsException("SettingsSaveFailedNoSection");
			}

			// Nested Types
			private sealed class ClientSettingsConfigurationHost : DelegatingConfigHost
			{
				// Fields
				private const string ClientConfigurationHostTypeName = "System.Configuration.ClientConfigurationHost,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
				private const string InternalConfigConfigurationFactoryTypeName = "System.Configuration.Internal.InternalConfigConfigurationFactory,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
				private static IInternalConfigConfigurationFactory s_configFactory;

				// Methods
				private ClientSettingsConfigurationHost()
				{
				}

				public override void Init(IInternalConfigRoot configRoot, params object[] hostInitParams)
				{
				}

				public override void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot configRoot, params object[] hostInitConfigurationParams)
				{
					object[] objArray;
					ConfigurationUserLevel level = (ConfigurationUserLevel)hostInitConfigurationParams[0];
					string roamingUserConfigPath = null;
					base.Host = (IInternalConfigHost)TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.ClientConfigurationHost,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					ConfigurationUserLevel level2 = level;
					if (level2 != ConfigurationUserLevel.None)
					{
						if (level2 != ConfigurationUserLevel.PerUserRoaming)
						{
							if (level2 != ConfigurationUserLevel.PerUserRoamingAndLocal)
							{
								throw new ArgumentException("UnknownUserLevel");
							}
							roamingUserConfigPath = this.ClientHost.GetLocalUserConfigPath();
							goto Label_006D;
						}
					}
					else
					{
						roamingUserConfigPath = this.ClientHost.GetExeConfigPath();
						goto Label_006D;
					}
					roamingUserConfigPath = this.ClientHost.GetRoamingUserConfigPath();
				Label_006D:
					objArray = new object[3];
					objArray[2] = roamingUserConfigPath;
					base.Host.InitForConfiguration(ref locationSubPath, out configPath, out locationConfigPath, configRoot, objArray);
				}

				private bool IsKnownConfigFile(string filename)
				{
					if ((!string.Equals(filename, ConfigurationManagerInternalFactory.Instance.MachineConfigPath, StringComparison.OrdinalIgnoreCase) && !string.Equals(filename, ConfigurationManagerInternalFactory.Instance.ApplicationConfigUri, StringComparison.OrdinalIgnoreCase)) && !string.Equals(filename, ConfigurationManagerInternalFactory.Instance.ExeLocalConfigPath, StringComparison.OrdinalIgnoreCase))
					{
						return string.Equals(filename, ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigPath, StringComparison.OrdinalIgnoreCase);
					}
					return true;
				}

				internal static System.Configuration.Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
				{
					return ConfigFactory.Create(typeof(ClientSettingsStore.ClientSettingsConfigurationHost), new object[] { userLevel });
				}

				public override Stream OpenStreamForRead(string streamName)
				{
					if (this.IsKnownConfigFile(streamName))
					{
						return base.Host.OpenStreamForRead(streamName, true);
					}
					return base.Host.OpenStreamForRead(streamName);
				}

				public override Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext)
				{
					if (string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeLocalConfigPath, StringComparison.OrdinalIgnoreCase))
					{
						return new ClientSettingsStore.QuotaEnforcedStream(base.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext, true), false);
					}
					if (string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigPath, StringComparison.OrdinalIgnoreCase))
					{
						return new ClientSettingsStore.QuotaEnforcedStream(base.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext, true), true);
					}
					return base.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext);
				}

				public override void WriteCompleted(string streamName, bool success, object writeContext)
				{
					if (string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeLocalConfigPath, StringComparison.OrdinalIgnoreCase) || string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigPath, StringComparison.OrdinalIgnoreCase))
					{
						base.Host.WriteCompleted(streamName, success, writeContext, true);
					}
					else
					{
						base.Host.WriteCompleted(streamName, success, writeContext);
					}
				}

				// Properties
				private IInternalConfigClientHost ClientHost
				{
					get
					{
						return (IInternalConfigClientHost)base.Host;
					}
				}

				internal static IInternalConfigConfigurationFactory ConfigFactory
				{
					get
					{
						if (s_configFactory == null)
						{
							s_configFactory = (IInternalConfigConfigurationFactory)TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.Internal.InternalConfigConfigurationFactory,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
						}
						return s_configFactory;
					}
				}
			}

			private sealed class QuotaEnforcedStream : Stream
			{
				// Fields
				private bool _isRoaming;
				private Stream _originalStream;

				// Methods
				internal QuotaEnforcedStream(Stream originalStream, bool isRoaming)
				{
					this._originalStream = originalStream;
					this._isRoaming = isRoaming;
				}

				public override IAsyncResult BeginRead(byte[] buffer, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
				{
					return this._originalStream.BeginRead(buffer, offset, numBytes, userCallback, stateObject);
				}

				public override IAsyncResult BeginWrite(byte[] buffer, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
				{
					if (!this.CanWrite)
					{
						throw new NotSupportedException();
					}
					long length = this._originalStream.Length;
					long num2 = this._originalStream.CanSeek ? (this._originalStream.Position + numBytes) : (this._originalStream.Length + numBytes);
					this.EnsureQuota(Math.Max(length, num2));
					return this._originalStream.BeginWrite(buffer, offset, numBytes, userCallback, stateObject);
				}

				public override void Close()
				{
					this._originalStream.Close();
				}

				protected override void Dispose(bool disposing)
				{
					if (disposing && (this._originalStream != null))
					{
						this._originalStream.Dispose();
						this._originalStream = null;
					}
					base.Dispose(disposing);
				}

				public override int EndRead(IAsyncResult asyncResult)
				{
					return this._originalStream.EndRead(asyncResult);
				}

				public override void EndWrite(IAsyncResult asyncResult)
				{
					this._originalStream.EndWrite(asyncResult);
				}

				private void EnsureQuota(long size)
				{
					new IsolatedStorageFilePermission(PermissionState.None) 
					{ 
						UserQuota = size, 
						UsageAllowed = (IsolatedStorageContainment)(this._isRoaming ? 80 : 0x10) 
					}.Demand();
				}

				public override void Flush()
				{
					this._originalStream.Flush();
				}

				public override int Read(byte[] buffer, int offset, int count)
				{
					return this._originalStream.Read(buffer, offset, count);
				}

				public override int ReadByte()
				{
					return this._originalStream.ReadByte();
				}

				public override long Seek(long offset, SeekOrigin origin)
				{
					long num2;
					if (!this.CanSeek)
					{
						throw new NotSupportedException();
					}
					long length = this._originalStream.Length;
					switch (origin)
					{
						case SeekOrigin.Begin:
							num2 = offset;
							break;

						case SeekOrigin.Current:
							num2 = this._originalStream.Position + offset;
							break;

						case SeekOrigin.End:
							num2 = length + offset;
							break;

						default:
							throw new ArgumentException("UnknownSeekOrigin");
					}
					this.EnsureQuota(Math.Max(length, num2));
					return this._originalStream.Seek(offset, origin);
				}

				public override void SetLength(long value)
				{
					long length = this._originalStream.Length;
					long num2 = value;
					this.EnsureQuota(Math.Max(length, num2));
					this._originalStream.SetLength(value);
				}

				public override void Write(byte[] buffer, int offset, int count)
				{
					if (!this.CanWrite)
					{
						throw new NotSupportedException();
					}
					long length = this._originalStream.Length;
					long num2 = this._originalStream.CanSeek ? (this._originalStream.Position + count) : (this._originalStream.Length + count);
					this.EnsureQuota(Math.Max(length, num2));
					this._originalStream.Write(buffer, offset, count);
				}

				public override void WriteByte(byte value)
				{
					if (!this.CanWrite)
					{
						throw new NotSupportedException();
					}
					long length = this._originalStream.Length;
					long num2 = this._originalStream.CanSeek ? (this._originalStream.Position + 1L) : (this._originalStream.Length + 1L);
					this.EnsureQuota(Math.Max(length, num2));
					this._originalStream.WriteByte(value);
				}

				// Properties
				public override bool CanRead
				{
					get
					{
						return this._originalStream.CanRead;
					}
				}

				public override bool CanSeek
				{
					get
					{
						return this._originalStream.CanSeek;
					}
				}

				public override bool CanWrite
				{
					get
					{
						return this._originalStream.CanWrite;
					}
				}

				public override long Length
				{
					get
					{
						return this._originalStream.Length;
					}
				}

				public override long Position
				{
					get
					{
						return this._originalStream.Position;
					}
					set
					{
						if (value < 0L)
						{
							throw new ArgumentOutOfRangeException("value", "PositionOutOfRange");
						}
						this.Seek(value, SeekOrigin.Begin);
					}
				}
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct StoredSetting
		{
			internal SettingsSerializeAs SerializeAs;
			internal XmlNode Value;
			internal StoredSetting(SettingsSerializeAs serializeAs, XmlNode value)
			{
				this.SerializeAs = serializeAs;
				this.Value = value;
			}
		}
		internal static class TypeUtil
		{
			[ReflectionPermission(SecurityAction.Assert, Flags = ReflectionPermissionFlag.MemberAccess)]
			internal static object CreateInstanceWithReflectionPermission(string typeString)
			{
				return Activator.CreateInstance(Type.GetType(typeString, true), true);
			}
		}
		[ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
		internal static class PrivilegedConfigurationManager
		{
			internal static object GetSection(string sectionName)
			{
				return ConfigurationManager.GetSection(sectionName);
			}
			internal static ConnectionStringSettingsCollection ConnectionStrings
			{
				get
				{
					return ConfigurationManager.ConnectionStrings;
				}
			}
		}
		internal static class ConfigurationManagerInternalFactory
		{
			private const string ConfigurationManagerInternalTypeString = "System.Configuration.Internal.ConfigurationManagerInternal, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
			private static IConfigurationManagerInternal s_instance;
			internal static IConfigurationManagerInternal Instance
			{
				get
				{
					if (s_instance == null)
					{
						s_instance = (IConfigurationManagerInternal)TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.Internal.ConfigurationManagerInternal, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					}
					return s_instance;
				}
			}
		}

		#endregion
	}
}