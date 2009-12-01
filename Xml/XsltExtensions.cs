using System;
using System.Text.RegularExpressions;

namespace Adenson.Xml
{
	/// <summary>
	/// Bunch o xsl extensions
	/// </summary>
	public class XsltExtensions
	{
		/// <summary>
		/// Converts value to boolean
		/// </summary>
		/// <param name="value">the value</param>
		/// <returns>the result, or false if it cannot be parsed</returns>
		public bool boolean(string value)
		{
			try
			{
				return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// Uppercases the value
		/// </summary>
		/// <param name="value">the value</param>
		/// <returns>the result</returns>
		public string lowercase(string value)
		{
			return value == null ? string.Empty : value.ToLower();
		}
		/// <summary>
		/// Lower cases the value
		/// </summary>
		/// <param name="value">the value</param>
		/// <returns>the result</returns>
		public string uppercase(string value)
		{
			return value == null ? string.Empty : value.ToUpper();
		}
		/// <summary>
		/// Removes xml-like tags from the string
		/// </summary>
		/// <param name="value">the value</param>
		/// <returns>the result</returns>
		public string stripHtml(string value)
		{
			// Setup the regular expression and add the Or operator.
			Regex regExp = new Regex("<(.+?)>", RegexOptions.IgnoreCase);
			// Highlight keywords by calling the delegate each time a keyword is found.
			return regExp.Replace(value, new MatchEvaluator(delegate { return " "; }));
		}
	}
}