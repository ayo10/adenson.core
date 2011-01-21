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
	/// LocalFileSettingsProvider that does not use versioning in the config file name. Does not support upgrade either.
	/// </summary>
	public sealed class LocalFileSettingsNoVersionProvider : LocalFileSettingsProvider
	{
		/// <summary>
		/// Initializes a new provider
		/// </summary>
		public LocalFileSettingsNoVersionProvider()
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
}