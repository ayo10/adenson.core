using System;
using System.ComponentModel;
using System.Globalization;

namespace Adenson
{
	/// <summary>
	/// Utility classes for types
	/// </summary>
	public static class TypeUtil
	{
		#region Methods

		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static object CreateInstance(string typeName)
		{
			if (StringUtil.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");
			Type type = TypeUtil.GetType(typeName);
			if (type == null) throw new TypeLoadException(StringUtil.Format(Exceptions.TypeArgCouldNotBeLoaded, typeName));
			return Activator.CreateInstance(type);
		}
		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor, and casts it to specified generic type parameter
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeName)
		{
			if (StringUtil.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");
			return (T)TypeUtil.CreateInstance(typeName);
		}
		/// <summary>
		/// Creates an instance of the specified type using that type's default constructor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return (T)Activator.CreateInstance(type);
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
			var converted = TypeUtil.TryConvert(value, typeof(T), out output);
			if (converted) result = (T)output;
			else result = default(T);
			return converted;
		}
		/// <summary>
		/// Tries to convert the specified value
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <param name="type">The type of the object we need to convert</param>
		/// <param name="output">the output of the conversion</param>
		/// <returns>true if conversion happened correctly, false otherwise</returns>
		public static bool TryConvert(object value, Type type, out object output)
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
						var enumConverter = typeConverter as EnumConverter;
						var valueAsString = value as string;
						if (enumConverter != null && valueAsString != null)
						{
							var splits = valueAsString.Split(',', '|');
							var intValue = 0;
							var successful = true;
							foreach (var str in splits)
							{
								object newOutput;
								if (TryConvert(str.Trim(), type, out newOutput)) intValue += (int)newOutput;
								else
								{
									successful = false;
									break;
								}
							}
							if (successful)
							{
								output = Enum.Parse(type, intValue.ToString(CultureInfo.InvariantCulture));
								result = true;
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