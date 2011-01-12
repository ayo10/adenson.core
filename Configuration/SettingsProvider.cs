using System;
using System.Linq;
using System.Reflection;

namespace Adenson.Configuration
{
	/// <summary>
	///  Acts as a base class for deriving custom settings providers in the application settings architecture.
	/// </summary>
	public abstract class SettingsProvider : System.Configuration.SettingsProvider
	{
		#region Constructor

		/// <summary>
		/// Initializes an instance of the SettingsProvider class.
		/// </summary>
		protected SettingsProvider()
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
	}
}