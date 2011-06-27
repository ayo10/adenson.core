using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Adenson
{
	/// <summary>
	/// Utility classes for types
	/// </summary>
	public static class TypeUtil
	{
		#region Methods

		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor, and casts it to specified generic type parameter
		/// </summary>
		/// <typeparam name="T">The type of instance to return</typeparam>
		/// <param name="typeName">The full name of the type</param>
		/// <returns>Created instance</returns>
		public static T CreateInstance<T>()
		{
			return Activator.CreateInstance<T>();
		}
		
		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor, and casts it to specified generic type parameter
		/// </summary>
		/// <typeparam name="T">The type of instance to return</typeparam>
		/// <param name="typeName">The full name of the type</param>
		/// <returns>Created instance</returns>
		/// <exception cref="ArgumentNullException">If typeName is null or whitespace</exception>
		public static T CreateInstance<T>(string typeName)
		{
			if (StringUtil.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");
			Type type = TypeUtil.GetType(typeName);
			if (type == null) throw new TypeLoadException(StringUtil.Format(Exceptions.TypeArgCouldNotBeLoaded, typeName));
			return (T)Activator.CreateInstance(type);
		}
		
		/// <summary>
		/// Creates an instance of the specified type using that type's default constructor.
		/// </summary>
		/// <remarks>calls (T)Activator.CreateInstance(type).</remarks>
		/// <typeparam name="T">The type of instance to return</typeparam>
		/// <param name="type">The type</param>
		/// <returns>Created instance</returns>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public static T CreateInstance<T>(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return (T)Activator.CreateInstance(type);
		}
		
		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. (Case insensitive).
		/// </summary>
		/// <remarks>calls Enum.Parse(typeof(T), value, true)</remarks>
		/// <typeparam name="T">The type of enum to return</typeparam>
		/// <param name="value"> A string containing the name or value to convert.</param>
		/// <returns>An object of type enumType whose value is represented by value.</returns>
		/// <exception cref="ArgumentNullException">If value is null or whitespace</exception>
		/// <exception cref="ArgumentException">enumType is not an System.Enum.-or- value is either an empty string ("") or only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration.</exception>
		/// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
		public static T EnumParse<T>(string value)
		{
			if (StringUtil.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

			return (T)Enum.Parse(typeof(T), value, true);
		}
		
		/// <summary>
		/// Gets the System.Type with the specified name.
		/// </summary>
		/// <param name="typeName">The assembly-qualified name of the type to get.</param>
		/// <returns>The type with the specified name.</returns>
		public static Type GetType(string typeName)
		{
			if (StringUtil.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");

			Type type = Type.GetType(typeName, false, true);
			if (type == null)
			{
				string[] splits = typeName.Split(';');
				if (splits.Length >= 2)
				{
					string assembly = splits[0].Trim();
					string typestr = splits[1].Trim();
					type = Type.GetType(typestr + ", " + assembly, false, true);
				}
			}
			return type;
		}
		
		/// <summary>
		/// Tries to convert the specified value
		/// </summary>
		/// <typeparam name="T">The type of the object we need to convert</typeparam>
		/// <param name="value">the value to convert</param>
		/// <param name="result">the output of the result</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert<T>(object value, out T result)
		{
			object output;
			var converted = TypeUtil.TryConvert(typeof(T), value, out output);
			if (converted) result = (T)output;
			else result = default(T);
			return converted;
		}
		
		/// <summary>
		/// Tries to convert the specified value
		/// </summary>
		/// <param name="type">The type of the object we need to convert</param>
		/// <param name="value">the value to convert</param>
		/// <param name="output">the output of the conversion</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
		public static bool TryConvert(Type type, object value, out object output)
		{
			var result = false;
			if (value == null) output = null;
			else if (value != null && type == value.GetType())
			{
				output = value;
				result = true;
			}
			else
			{
				output = null;
				TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
				if (typeConverter != null && typeConverter.CanConvertFrom(value.GetType()))
				{
					if (typeConverter.IsValid(value))
					{
						output = typeConverter.ConvertFrom(value);
						result = true;
					}
					else
					{
						var valueAsString = value as string;
						if ((typeConverter is EnumConverter) && valueAsString != null)
						{
							try
							{
								var splits = valueAsString.Split('|');
								var intValue = 0;
								foreach (var str in splits) intValue += (int)Enum.Parse(type, str);
								output = Enum.Parse(type, intValue.ToString());
								result = true;
							}
							catch (ArgumentException)
							{
								result = false;
							}
							catch (OverflowException)
							{
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		#endregion
	}
}