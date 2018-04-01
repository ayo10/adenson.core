using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
	/// <summary>
	/// Collection of string utility methods
	/// </summary>
	public static class StringUtil
	{
		#region Methods

		/// <summary>
		/// Generates a random string of specified <paramref name="length"/>.
		/// </summary>
		/// <param name="length">Max length of string to generate.</param>
		/// <returns>String generated</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/> is less or equal to 0.</exception>
		public static string GenerateRandomString(int length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}

			Random ra = new Random();
			string result = string.Empty;
			for (int i = 0; i < length; i++)
			{
				result += (char)ra.Next(65, 122); ////65 = 'A', 122 = 'z'
			}

			return result;
		}

		/// <summary>
		/// Converts specified string to a byte array using <see cref="System.Text.Encoding.UTF8"/>.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>A byte array, or null if string is null</returns>
		public static byte[] ToBytes(string value)
		{
			return StringUtil.ToBytes(value, Encoding.UTF8);
		}

		/// <summary>
		/// Converts specified string to a byte array using <see cref="System.Text.Encoding.UTF8"/>.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <param name="encoding">The encoding to use.</param>
		/// <returns>A byte array, or null if string is null</returns>
		public static byte[] ToBytes(string value, Encoding encoding)
		{
			if (value == null)
			{
				return null;
			}

			return Arg.IsNotNull(encoding).GetBytes(value);
		}

		/// <summary>
		/// Converts the specified list to comma delimited list.
		/// </summary>
		/// <typeparam name="T">The type of items in the list.</typeparam>
		/// <param name="list">The items to iterate and string-ify.</param>
		/// <returns>A comma delimited list of items in the list.</returns>
		public static string ToString<T>(IEnumerable<T> list)
		{
			string str = list as string;
			if (str != null)
			{
				return str;
			}
			else if (list == null)
			{
				return $"Null<{typeof(T).Name}>";
			}
			else if (!list.Any())
			{
				return $"Empty<{typeof(T).Name}>";
			}

			return $"{list.GetType().GetGenericTypeDefinition().Name}<{typeof(T).Name}> [{String.Join(",", list.Select(x => x == null ? "null" : x.ToString()).ToArray())}]";
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null.
		/// </summary>
		/// <param name="exception">The Exception object to convert.</param>
		/// <returns>The string</returns>
		public static string ToString(Exception exception)
		{
			Arg.IsNotNull(exception, "exception");

			List<string> message = new List<string>();
			StringUtil.ToString(exception, message);
			return String.Join(Environment.NewLine, message);
		}

		/// <summary>
		/// Converts the value of the specified object to its equivalent string representation.
		/// </summary>
		/// <remarks>Calls Convert.ToString(value)</remarks>
		/// <param name="value">An object that supplies the value to convert, or null.</param>
		/// <returns>The string representation of value, or System.String.Empty if value is null.</returns>
		public static string ToString(object value)
		{
			if (value == null)
			{
				return null;
			}

			string str = value as string;
			if (str != null)
			{
				return str;
			}

			Exception ex = value as Exception;
			if (ex != null)
			{
				return StringUtil.ToString(ex);
			}

			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null)
			{
				Type type = value.GetType();
				string typeName = TypeUtil.GetName(type);
				string items = String.Join(",", enumerable.Cast<object>().Select(o => ToString(o) ?? "null").ToArray());
				return $"{typeName}{{{items}}}";
			}

			return Convert.ToString(value, System.Globalization.CultureInfo.CurrentCulture);
		}

		private static void ToString(Exception exception, List<string> lines, string prepend = null)
		{
			if (exception == null)
			{
				return;
			}

			int lastEnd = lines.Count;

			lines.Add($"{exception.GetType().FullName}: {exception.Message}");
			string linkAndSource = String.Empty;
			if (!String.IsNullOrWhiteSpace(exception.HelpLink))
			{
				linkAndSource = $"HelpLink: {exception.HelpLink}";
			}

			if (!String.IsNullOrWhiteSpace(exception.Source))
			{
				linkAndSource = (String.IsNullOrEmpty(linkAndSource) ? String.Empty : linkAndSource + ", ") + $"Source: {exception.Source}";
			}

			if (!String.IsNullOrEmpty(linkAndSource))
			{
				lines.Add("\t" + linkAndSource);
			}

			ReflectionTypeLoadException rtlex = exception as ReflectionTypeLoadException;
			if (rtlex != null)
			{
				if (rtlex.Types != null && rtlex.Types.Length > 0)
				{
					lines.Add($"\tTypes: {String.Join(", ", rtlex.Types.Select(t => t.FullName).ToArray())}");
				}

				if (rtlex.LoaderExceptions != null && rtlex.LoaderExceptions.Length > 0)
				{
					lines.Add("\tLoaderExceptions:");
					foreach (Exception lex in rtlex.LoaderExceptions)
					{
						StringUtil.ToString(lex, lines, "\t\t");
					}
				}
			}

			if (exception.StackTrace != null)
			{
				lines.Add(exception.StackTrace);
			}

			if (!String.IsNullOrEmpty(prepend))
			{
				for (int i = lastEnd; i < lines.Count; i++)
				{
					lines[i] = prepend + lines[i];
				}
			}

			StringUtil.ToString(exception.InnerException, lines, prepend);
		}

		#endregion
	}
}