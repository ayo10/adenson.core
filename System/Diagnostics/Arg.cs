using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
	/// <summary>
	/// Method arguments/parameters checker.
	/// Borrowed/Stolen/Hijacked/Hoodwinkled/Hinted (mostly) from MS.
	/// </summary>
	[DebuggerStepThrough]
	#if !NET35
	[ExcludeFromCodeCoverage]
	#endif
	public static class Arg
	{
		#region Methods

		/// <summary>
		/// Checks to see if the value is not null and is an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The type to check for.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="parameterName">The parameter name.</param>
		[SuppressMessage("Microsoft.Design", "CA1004", Justification = "In use.")]
		[SuppressMessage("Microsoft.Globalization", "CA1305", Justification = "Irrelevant.")]
		public static void IsAssignableFrom<T>(Type value, string parameterName)
		{
			Arg.IsNotNull(value, parameterName);
			Arg.ThrowIf<ArgumentException>(typeof(T).IsAssignableFrom(value), parameterName, String.Format("The specified type '{0}' is not an instance of '{1}'.", value.FullName, typeof(T).FullName));
		}

		/// <summary>
		/// Checks to see if the value is not null and is an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The type to check for.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value if its not null and is of the specified type.</returns>
		[SuppressMessage("Microsoft.Design", "CA1004", Justification = "In use.")]
		[SuppressMessage("Microsoft.Globalization", "CA1305", Justification = "Irrelevant.")]
		public static T IsInstanceOf<T>([ValidatedNotNull]object value, string parameterName)
		{
			Arg.IsNotNull(value, parameterName);
			Arg.ThrowIf<ArgumentException>(typeof(T).IsAssignableFrom(value.GetType()), parameterName, String.Format("The specified type '{0}' is not an instance of '{1}'.", value.GetType().FullName, typeof(T).FullName));
			return (T)value;
		}

		/// <summary>
		/// Checks to see if the list has any null values. This WILL also check if <paramref name="value"/> is not null.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The <paramref name="value"/> argument.</returns>
		[SuppressMessage("Microsoft.Design", "CA1004", Justification = "In use.")]
		[SuppressMessage("Microsoft.Globalization", "CA1305", Justification = "Irrelevant.")]
		public static IEnumerable<T> IsNotAllNull<T>([ValidatedNotNull]IEnumerable<T> value, string parameterName)
		{
			Arg.IsNotNull(value, parameterName);
			Arg.ThrowIf<ArgumentNullException>(!value.Any(x => x == null), parameterName, String.Format("Argument '{0}' cannot contain null values", parameterName));
			return value;
		}

		/// <summary>
		/// Checks to see if the value is null or empty or whitespace.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value if its not empty.</returns>
		public static string IsNotEmpty([ValidatedNotNull]string value, string parameterName)
		{
			Arg.ThrowIf<ArgumentNullException>(!String.IsNullOrEmpty(value), parameterName);
			return value;
		}

		/// <summary>
		/// Checks to see if the list is not null or empty.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The list if its not null.</returns>
		[SuppressMessage("Microsoft.Design", "CA1004", Justification = "In use.")]
		[SuppressMessage("Microsoft.Globalization", "CA1305", Justification = "Irrelevant.")]
		public static IEnumerable<T> IsNotEmpty<T>([ValidatedNotNull]IEnumerable<T> value, string parameterName)
		{
			Arg.IsNotNull(value, parameterName);
			Arg.ThrowIf<ArgumentNullException>(value.Any(), parameterName, String.Format("Argument '{0}' cannot be empty.", parameterName));
			return value;
		}
		
		/// <summary>
		/// Checks to see if the value is null.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The value of it is not null.</returns>
		public static T IsNotNull<T>([ValidatedNotNull]T value, string parameterName)
		{
			Arg.ThrowIf<ArgumentNullException>(value != null, parameterName);
			return value;
		}

		/// <summary>
		/// Checks to see if the value, invoked on the specified condition returns true. If not, throws an ArgumentException.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="condition">The condition that needs to be met.</param>
		/// <param name="parameterName">The message to set on the exception.</param>
		/// <returns>The value of it meets the specified condition.</returns>
		public static T Is<T>(T value, Func<T, bool> condition, string parameterName)
		{
			Arg.IsNotNull(condition, "condition");
			Arg.ThrowIf<ArgumentException>(condition(value), parameterName);
			return value;
		}

		/// <summary>
		/// Checks the value against the specified list of conditions.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="conditions">The conditions that need to be met. The signature in conditions you will notice matches Arg method like <see cref="IsNotNull"/>, <see cref="IsNotAllNull"/> etc.</param>
		/// <param name="parameterName">The parameter to pass to the argument exception.</param>
		/// <returns>The value if its not null and is of the specified type.</returns>
		public static T Is<T>(T value, Func<T, string, T>[] conditions, string parameterName)
		{
			foreach (var method in Arg.IsNotAllNull(conditions, "conditions"))
			{
				method(value, parameterName);
			}

			return value;
		}

		/// <summary>
		/// Checks that the condition is true, and if it is not, throws an <see cref="ArgumentException"/>.
		/// </summary>
		/// <param name="condition">The condition that needs to be met.</param>
		/// <param name="parameterName">The parameter to pass to the argument exception.</param>
		public static void Is(bool condition, string parameterName)
		{
			Arg.ThrowIf<ArgumentException>(condition, parameterName);
		}

		private static void ThrowIf<T>(bool condition, string parameterName, string message = null) where T : Exception
		{
			if (condition)
			{
				return;
			}

			if (typeof(T) == typeof(ArgumentNullException))
			{
				if (message == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				else
				{
					throw new ArgumentNullException(parameterName, message);
				}
			}
			else if (typeof(T) == typeof(ArgumentException))
			{
				throw new ArgumentException(message, parameterName);
			}
			else
			{
				throw (Exception)Activator.CreateInstance(typeof(T), message);
			}
		}

		#endregion
	}
}