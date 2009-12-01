using System;
using System.Configuration;

namespace Adenson.Configuration
{
	internal sealed class EncryptorSection : ConfigurationSection
	{
		private static ConfigurationProperty prop = new ConfigurationProperty(null, typeof(EncryptorCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

		/// <remarks>
		/// Setting name to 'Encryptors' and using base["Encryptors"] fails miserable, dont know 
		/// why, had to 'reflect' how microsoft did ConnectionStringsSection, so, nope, I am not 
		/// that brilliant, just know how to cheat!
		/// </remarks>
		[ConfigurationProperty("", IsDefaultCollection=true)]
		public EncryptorCollection Encryptors
		{
			get { return (EncryptorCollection)base[prop]; }
		}
	}
}