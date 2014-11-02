using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
	/// <summary>
	/// Argument checker.
	/// Borrowed/Stolen/Hijacked/Hoodwinkled/Hinted from MS.
	/// </summary>
	public static class Check
	{
		#region Methods

		/// <summary>
		/// Checks to see if the value is not null and is an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The type to check for.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value if its not null and is of the specified type.</returns>
		public static object InstanceOf<T>(object value, string parameterName)
		{
			Check.NotNull(value, parameterName);
			if (typeof(T).IsAssignableFrom(value.GetType()))
			{
				throw new ArgumentException(String.Format("The specified type '{0}' is not an instance of '{1}'.", value.GetType().FullName, typeof(T).FullName));
			}

			return value;
		}

		/// <summary>
		/// Checks to see if the list is not null or has null values.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The list if its not null.</returns>
		public static IEnumerable<T> NotAllNull<T>([ValidatedNotNull]IEnumerable<T> value, string parameterName)
		{
			Check.NotNull(value, parameterName);
			if (value.Any(x => x == null))
			{
				throw new ArgumentException(String.Format("Argument '{0}' cannot contain null values", parameterName), parameterName);
			}

			return value;
		}

		/// <summary>
		/// Checks to see if the list is not null or empty.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The list if its not null.</returns>
		public static IEnumerable<T> NotEmpty<T>([ValidatedNotNull]IEnumerable<T> value, string parameterName)
		{
			Check.NotNull(value, parameterName);
			if (!value.Any())
			{
				throw new ArgumentException(String.Format("Argument '{0}' cannot be empty.", parameterName), parameterName);
			}

			return value;
		}

		/// <summary>
		/// Checks to see if the value is null or empty or whitespace.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value if its not empty.</returns>
		public static string NotEmpty([ValidatedNotNull]string value, string parameterName)
		{
			if (String.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(parameterName);
			}

			return value;
		}

		/// <summary>
		/// Checks to see if the value is null.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value of it is not null.</returns>
		public static T NotNull<T>([ValidatedNotNull]T value, string parameterName) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}

			return value;
		}

		/// <summary>
		/// Checks to see if the value has a value.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value if it has a value.</returns>
		public static T? NotNull<T>([ValidatedNotNull]T? value, string parameterName) where T : struct
		{
			if (!value.HasValue)
			{
				throw new ArgumentNullException(parameterName);
			}

			return value;
		}

		#endregion
		#region Inner Classes

		/// <summary>
		/// Exists for the sole reason to tell fxcop that the specified value will be checked for null.
		/// </summary>
		internal sealed class ValidatedNotNullAttribute : Attribute
		{
		}

		#endregion
	}
}