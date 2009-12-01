using System;
using System.Collections.Generic;
using System.Linq;

namespace Adenson
{
	public static class Extensions
	{
		public static bool ContainsKey<T>(this Dictionary<string, T> dick, string key, StringComparison comparism)
		{
			return dick.Keys.Any(k => k.Equals(key, comparism));
		}
		public static T GetValue<T>(this Dictionary<string, T> dick, string key)
		{
			string actualKey = dick.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.CurrentCultureIgnoreCase));
			return dick[actualKey];
		}
	}
}