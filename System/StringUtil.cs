using System;
using System.Collections;
using System.Collections.Generic;
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
		/// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
		/// </summary>
		/// <param name="value">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		/// <exception cref="ArgumentNullException">Either <paramref name="value"/> or <paramref name="args"/> is null.</exception>
		public static string Format(string value, params object[] args)
		{
			if (StringUtil.IsNullOrWhiteSpace(value))
			{
				return String.Empty;
			}

			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			try
			{
				return String.Format(System.Globalization.CultureInfo.CurrentCulture, value, args);
			}
			catch (FormatException)
			{
				var str = value;
				for (int i = 0; i < args.Length; i++)
				{
					str = str.Replace("{" + i + "}", args[i] == null ? "null" : StringUtil.ToString(args[i]));
				}

				return str;
			}
		}

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
		/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		/// <remarks>Method exists for .NET 3.5, for .NET 4, simply calls String.IsNullOrWhiteSpace</remarks>
		/// <param name="value">The string to test.</param>
		/// <returns>true if the value parameter is null or System.String.Empty, or if value consists exclusively of white-space characters.</returns>
		public static bool IsNullOrWhiteSpace(string value)
		{
			#if NET35
			return String.IsNullOrEmpty(value == null ? null : value.Trim());
			#else
			return String.IsNullOrWhiteSpace(value);
			#endif
		}

		/// <summary>
		/// Converts specified string to a byte array using <see cref="System.Text.Encoding.Default"/>.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>A byte array, or null if string is null</returns>
		public static byte[] ToBytes(string value)
		{
			return StringUtil.ToBytes(value, Encoding.Default);
		}

		/// <summary>
		/// Converts specified string to a byte array using <see cref="System.Text.Encoding.Default"/>.
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

			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}

			return encoding.GetBytes(value);
		}

		/// <summary>
		/// Converts the specified list to comma delimited list.
		/// </summary>
		/// <typeparam name="T">The type of items in the list.</typeparam>
		/// <param name="list">The items to iterate and string-ify.</param>
		/// <returns>A comma delimited list of items in the list.</returns>
		public static string ToString<T>(IEnumerable<T> list)
		{
			if (list == null)
			{
				return StringUtil.Format("Null<{0}>", typeof(T).Name);
			}
			else if (!list.Any())
			{
				return StringUtil.Format("Empty<{0}>", typeof(T).Name);
			}

			return StringUtil.Format("{0}<{1}> [{2}]", list.GetType().GetGenericTypeDefinition().Name, typeof(T).Name, String.Join(",", list.Select(x => x == null ? "null" : x.ToString()).ToArray()));
		}

		/// <summary>
		/// Converts the specified byte array to its equivalent string representation in Base64, minus the slashes and equal signs.
		/// </summary>
		/// <param name="buffer">The byte array to convert.</param>
		/// <returns>The string representation of the specified byte array, or null if its null.</returns>
		public static string ToString(byte[] buffer)
		{
			if (buffer == null)
			{
				return null;
			}

			var base64 = System.Convert.ToBase64String(buffer);
			return base64.Replace("\\", String.Empty).Replace("/", String.Empty).Replace("=", String.Empty);
		}

		/// <summary>
		/// Converts an Exception object into a string by looping thru its InnerException and prepending Message and StackTrace until InnerException becomes null.
		/// </summary>
		/// <param name="exception">The Exception object to convert.</param>
		/// <returns>The string</returns>
		public static string ToString(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}

			List<string> message = new List<string>();
			StringUtil.ToString(exception, message);
			return String.Join(Environment.NewLine, message.ToArray());
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

			byte[] arr = value as byte[];
			if (arr != null)
			{
				return StringUtil.ToString(arr);
			}

			Exception ex = value as Exception;
			if (ex != null)
			{
				return StringUtil.ToString(ex);
			}

			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null)
			{
				var genericType = enumerable.GetType().GetGenericTypeDefinition();
				return StringUtil.Format("{0}{1} [{2}]", value.GetType().Name, genericType == null ? String.Empty : StringUtil.Format("<{0}>", genericType.Name), String.Join(",", enumerable.Cast<object>().Select(o => o == null ? "null" : o.ToString()).ToArray()));
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

			lines.Add(StringUtil.Format("{0}: {1}", exception.GetType().FullName, exception.Message));
			if (!String.IsNullOrEmpty(exception.Source))
			{
				lines.Add(StringUtil.Format("   HelpLink: {0}, Source: {1}", exception.HelpLink, exception.Source));
			}

			ReflectionTypeLoadException rtlex = exception as ReflectionTypeLoadException;
			if (rtlex != null)
			{
				if (rtlex.Types != null && rtlex.Types.Length > 0)
				{
					lines.Add(StringUtil.Format("\tTypes: {0}", String.Join(", ", rtlex.Types.Select(t => t.FullName).ToArray())));
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