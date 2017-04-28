using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Xml.Linq
{
	/// <summary>
	/// Bunch of extensions on the System.Xml.Linq namespace
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Gets the first (in document order) child element with the specified <see cref="XName"/>.
		/// </summary>
		/// <param name="source">The <see cref="XElement"/> to look into.</param>
		/// <param name="name">The <see cref="XName"/> to match.</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>A <see cref="XElement"/> that matches the specified<see cref="XName"/>, or null.</returns>
		/// <exception cref="ArgumentNullException">If source is null, OR name is null or name.LocalName is whitespace</exception>
		public static XElement Element(this XContainer source, XName name, StringComparison comparisonType)
		{
			Arg.IsNotNull(source, "source");
			Arg.IsNotNull(name, "name");
			Arg.IsNotEmpty(name.LocalName, "name");

			return source.Elements().FirstOrDefault(e => String.Equals(e.Name.LocalName, name.LocalName, comparisonType));
		}

		/// <summary>
		/// Finds an Element then an Attribute in the <paramref name="source"/> and returns the value of the first one matching the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="source">The <see cref="XElement"/> to look into.</param>
		/// <param name="name">The <see cref="XName"/> to match.</param>
		/// <returns>Found value of any, null otherwise.</returns>
		/// <exception cref="ArgumentNullException">If source is null, OR name is null or name.LocalName is whitespace</exception>
		public static string GetValue(this XElement source, XName name)
		{
			return source.GetValue<string>(name);
		}

		/// <summary>
		/// Finds an Element then an Attribute in the <paramref name="source"/> and returns the value of the first one matching the specified <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T">The type to convert the value to</typeparam>
		/// <param name="source">The <see cref="XElement"/> to look into.</param>
		/// <param name="name">The <see cref="XName"/> to match.</param>
		/// <returns>Found value of any, default of T otherwise</returns>
		/// <exception cref="ArgumentNullException">If source is null, OR name is null or name.LocalName is whitespace</exception>
		public static T GetValue<T>(this XElement source, XName name)
		{
			Arg.IsNotNull(source, "source");
			Arg.IsNotNull(name, "name");
			Arg.IsNotEmpty(name.LocalName, "name");

			T result = default(T);
			string value = null;

			var element = source.Element(name);
			if (element != null)
			{
				value = element.Value;
			}

			if (StringUtil.IsNullOrWhiteSpace(value))
			{
				var attribute = source.Attribute(name);
				if (attribute != null)
				{
					value = attribute.Value;
				}
			}

			if (!StringUtil.IsNullOrWhiteSpace(value))
			{
				T output;
				if (TypeUtil.TryConvert<T>(value, out output))
				{
					result = output;
				}
			}

			return result;
		}

		/// <summary>
		/// Gets if the specified element has the specified sub element with specified key.
		/// </summary>
		/// <param name="source">The element to look into.</param>
		/// <param name="name">The key to look for.</param>
		/// <returns>True if an element with specified key is found, false otherwise</returns>
		/// <exception cref="ArgumentNullException">If source is null, OR name is null or name.LocalName is whitespace</exception>
		public static bool HasElement(this XContainer source, XName name)
		{
			Arg.IsNotNull(source, "source");
			Arg.IsNotNull(name, "name");
			Arg.IsNotEmpty(name.LocalName, "name");

			return source.HasElement(name, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Gets if the specified element has the specified sub element with specified key.
		/// </summary>
		/// <param name="source">The <see cref="XContainer"/> to look into.</param>
		/// <param name="name">The key to look for.</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
		/// <returns>True if an element with specified key is found, false otherwise</returns>
		/// <exception cref="ArgumentNullException">If source is null, OR name is null or name.LocalName is whitespace</exception>
		public static bool HasElement(this XContainer source, XName name, StringComparison comparisonType)
		{
			Arg.IsNotNull(source, "source");
			Arg.IsNotNull(name, "name");
			Arg.IsNotEmpty(name.LocalName, "name");

			return source.Elements().FirstOrDefault(e => String.Equals(e.Name.LocalName, name.LocalName, comparisonType)) != null;
		}
	}
}