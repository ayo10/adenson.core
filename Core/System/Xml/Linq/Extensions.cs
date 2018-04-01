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