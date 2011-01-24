using System;
using System.Linq;
using System.Reflection;
using System.Configuration;

namespace Adenson.Configuration
{
	/// <summary>
	///  Acts as a base class for deriving custom settings providers in the application settings architecture.
	/// </summary>
	public abstract class ApplicationSettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		#region Constructor

		/// <summary>
		/// Initializes an instance of the SettingsProvider class.
		/// </summary>
		protected ApplicationSettingsProvider()
		{
			var assembly = Assembly.GetEntryAssembly();
			this.CompanyName = ((AssemblyCompanyAttribute)assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true).First()).Company;
			this.ProductName = assembly.GetName().Name;
			this.Version = assembly.GetName().Version;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the current entry assembly's company name
		/// </summary>
		public string CompanyName
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets the current entry assembly's product name
		/// </summary>
		public string ProductName
		{
			get;
			protected set;
		}
		/// <summary>
		/// Gets the current entry assembly's version
		/// </summary>
		public Version Version
		{
			get;
			protected set;
		}

		#endregion
		#region Methods

		/// <summary>
		/// Returns the value of the specified settings property for the previous version of the same application.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		/// <param name="property">The SettingsProperty whose value is to be returned.</param>
		/// <returns> A SettingsPropertyValue containing the value of thes pecified property setting as it was last set in the previous version of the application; or null if the setting cannot be found.</returns>
		public abstract SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property);
		/// <summary>
		///  Resets the application settings associated with the specified application to their default values.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		public abstract void Reset(SettingsContext context);
		/// <summary>
		/// Indicates to the provider that the application has been upgraded. This offers the provider an opportunity to upgrade its stored settings as appropriate.
		/// </summary>
		/// <param name="context">A SettingsContext describing the current application usage.</param>
		/// <param name="properties">A SettingsPropertyCollection containing the settings property group whose values are to be retrieved.</param>
		public abstract void Upgrade(SettingsContext context, SettingsPropertyCollection properties);

		/// <summary>
		/// Gets the section name from the context
		/// </summary>
		/// <param name="context">The context</param>
		/// <returns>found section name</returns>
		protected static string GetSectionName(SettingsContext context)
		{
			string groupName = (string)context["GroupName"];
			string settingsKey = (string)context["SettingsKey"];
			string name = groupName;
			if (!String.IsNullOrEmpty(settingsKey)) name = StringUtil.Format("{0}.{1}", new object[] { name, settingsKey });
			return System.Xml.XmlConvert.EncodeLocalName(name);
		}

		#endregion
	}
}