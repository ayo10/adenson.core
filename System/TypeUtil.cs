using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Adenson;
using Adenson.Log;

namespace System
{
	/// <summary>
	/// Utility classes for types
	/// </summary>
	public static class TypeUtil
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(TypeUtil));
		private static Dictionary<Type, PropertyDescriptorCollection> typeDescriptorCache = new Dictionary<Type, PropertyDescriptorCollection>();
		#endregion
		#region Methods

		/// <summary>
		/// Converts the specified value to the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the object we need to convert.</typeparam>
		/// <param name="value">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static T Convert<T>(object value)
		{
			return (T)TypeUtil.Convert(typeof(T), value);
		}

		/// <summary>
		/// Converts the specified value to the specified type.
		/// </summary>
		/// <param name="type">The type of the object we need to convert.</param>
		/// <param name="value">The value to convert.</param>
		/// <returns>The converted value.</returns>
		/// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
		public static object Convert(Type type, object value)
		{
			if (value == null)
			{
				return null;
			}
			else if (value != null && type == value.GetType())
			{
				return value;
			}
			else
			{
				TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
				if (typeConverter != null && typeConverter.CanConvertFrom(value.GetType()))
				{
					if (typeConverter.IsValid(value))
					{
						return typeConverter.ConvertFrom(value);
					}
					else
					{
						string valueAsString = value as string;
						if ((typeConverter is EnumConverter) && valueAsString != null)
						{
							string[] splits = valueAsString.Split('|');
							int pipedValue = 0;
							foreach (var str in splits)
							{
								int parse = (int)Enum.Parse(type, str);
								pipedValue = pipedValue == 0 ? parse : pipedValue | parse;
							}

							return Enum.Parse(type, pipedValue.ToString(System.Globalization.CultureInfo.CurrentCulture));
						}
					}
				}
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates an instance of the type whose name is specified, using the named assembly and default constructor, and casts it to specified generic type parameter
		/// </summary>
		/// <typeparam name="T">The type of instance to return</typeparam>
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
			if (StringUtil.IsNullOrWhiteSpace(typeName))
			{
				throw new ArgumentNullException("typeName");
			}

			Type type = TypeUtil.GetType(typeName);
			if (type == null)
			{
				throw new TypeLoadException(StringUtil.Format(Adenson.Exceptions.TypeArgCouldNotBeLoaded, typeName));
			}

			return TypeUtil.CreateInstance<T>(type);
		}

		/// <summary>
		/// Creates an instance of the specified type using that type's default constructor.
		/// </summary>
		/// <remarks>Calls (T)Activator.CreateInstance(type).</remarks>
		/// <typeparam name="T">The type of instance to return</typeparam>
		/// <param name="type">The type</param>
		/// <returns>Created instance</returns>
		/// <exception cref="ArgumentNullException">If type is null</exception>
		public static T CreateInstance<T>(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			return (T)Activator.CreateInstance(type);
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. (Case insensitive).
		/// </summary>
		/// <remarks>Calls Enum.Parse(typeof(T), value, true)</remarks>
		/// <typeparam name="T">The type of enum to return</typeparam>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <returns>An object of type enumType whose value is represented by value.</returns>
		/// <exception cref="ArgumentNullException">If value is null or whitespace</exception>
		/// <exception cref="ArgumentException">The type is not an System.Enum.-or- value is either an empty string ("") or only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration.</exception>
		/// <exception cref="OverflowException">If <paramref name="value"/> is outside the range of the underlying type of enumType.</exception>
		public static T EnumParse<T>(string value)
		{
			if (StringUtil.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentNullException("value");
			}

			return (T)Enum.Parse(typeof(T), value, true);
		}

		/// <summary>
		/// Finds all types in the current domain where <paramref name="type"/> is assignable from (or implements) (but not equal to) (ie. type.IsAssignableFrom(t) via <see cref="Type.IsAssignableFrom(Type)"/>).
		/// </summary>
		/// <param name="type">The type to find.</param>
		/// <returns>An enumerable list of types found.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
		public static IEnumerable<Type> FindAllIsAssignableFrom(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => type != t && type.IsAssignableFrom(t)));
		}
		
		/// <summary>
		/// Gets a <see cref="System.ComponentModel.PropertyDescriptor"/> from object using the passed property name
		/// </summary>
		/// <param name="item">Object into whose toga we shall be looking in</param>
		/// <param name="propertyName">The particular property name we are looking for</param>
		/// <returns>A property descriptor object if found, null otherwise</returns>
		public static PropertyDescriptor GetPropertyDescriptor(object item, string propertyName)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			if (StringUtil.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}

			object source;
			return TypeUtil.GetPropertyDescriptor(item, propertyName, out source);
		}

		/// <summary>
		/// Gets the value of the property of the object passed in (attempts to anyway)
		/// </summary>
		/// <param name="item">Object into whose toga we shall be looking in</param>
		/// <param name="propertyName">The particular property name we are looking for</param>
		/// <returns>Value of the property if found</returns>
		/// <exception cref="System.ArgumentException">Thrown if the field name does not exist as a property of the object</exception>
		public static object GetPropertyValue(object item, string propertyName)
		{
			if (item == null)
			{
				return null;
			}

			if (String.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}

			IDictionary di = item as IDictionary;
			if (di != null)
			{
				return di[propertyName];
			}

			if (propertyName.IndexOf("[", StringComparison.Ordinal) > -1 && propertyName.EndsWith("]", StringComparison.Ordinal))
			{
				string[] splits = propertyName.Split('[');
				if (splits.Length >= 2)
				{
					string propName = splits[0];
					string indexName = splits[1].TrimEnd(']').Replace("'", String.Empty).Replace("\"", String.Empty);
					PropertyDescriptor pd = TypeUtil.GetPropertyDescriptor(item, propName);
					if (pd != null)
					{
						object itemValue = pd.GetValue(item);
						IDictionary id = itemValue as IDictionary;
						if (id != null)
						{
							return id[indexName];
						}
					}
					else
					{
						for (int i = 1; i < splits.Length; i++)
						{
							indexName = splits[i].TrimEnd(']').Replace("'", String.Empty).Replace("\"", String.Empty);
							PropertyInfo pi = item.GetType().GetProperty("Item");
							if (pi != null)
							{
								ParameterInfo[] parms = pi.GetIndexParameters();
								object x = System.Convert.ChangeType(indexName, parms[0].ParameterType, CultureInfo.InvariantCulture);
								item = pi.GetValue(item, new object[] { x });
							}
						}

						return item;
					}
				}

				throw new NotImplementedException(Adenson.Exceptions.NoSupportNonSingleParameterArrayedObjects);
			}
			else
			{
				object source;
				PropertyDescriptor pd = TypeUtil.GetPropertyDescriptor(item, propertyName, out source);
				if (pd != null)
				{
					return pd.GetValue(source);
				}
			}

			throw new ArgumentException(StringUtil.Format(Adenson.Exceptions.PropertyOrFieldNotFound, propertyName));
		}

		/// <summary>
		/// Gets the value of the property of the object passed in (attempts to anyway)
		/// </summary>
		/// <param name="item">Object into whose toga we shall be looking in</param>
		/// <param name="propertyNames">The particular property name we are looking for</param>
		/// <returns>Value of the property if found</returns>
		/// <exception cref="System.ArgumentException">Thrown if the field name does not exist as a property of the object</exception>
		public static object[] GetPropertyValues(object item, string[] propertyNames)
		{
			if (item == null)
			{
				return null;
			}

			if (propertyNames == null)
			{
				throw new ArgumentNullException("propertyNames");
			}

			List<object> list = new List<object>();
			for (int i = 0; i < propertyNames.Length; i++)
			{
				string val = propertyNames[i].Trim();
				if (!String.IsNullOrEmpty(val))
				{
					list.Add(TypeUtil.GetPropertyValue(item, val));
				}
			}

			return list.ToArray();
		}

		/// <summary>
		/// Gets the <see cref="Type"/> with the specified name.
		/// </summary>
		/// <param name="typeDescription">The assembly-qualified name of the type to get.</param>
		/// <returns>The type with the specified name, and if not found, returns null.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="typeDescription"/> is null.</exception>
		public static Type GetType(string typeDescription)
		{
			return TypeUtil.GetType(typeDescription, false);
		}

		/// <summary>
		/// Gets the System.Type with the specified name.
		/// </summary>
		/// <param name="typeDescription">The assembly-qualified name of the type to get.</param>
		/// <param name="throwOnError">True to throw an exception if the type cannot be found; false to return null.</param>
		/// <returns>The type with the specified description.</returns>
		/// <exception cref="ArgumentNullException">Argument <paramref name="typeDescription"/> is null.</exception>
		/// <exception cref="System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="TypeLoadException">
		///	Argument <paramref name="throwOnError"/> is true and the type is not found. 
		///	-or-<paramref name="throwOnError"/> is true and <paramref name="typeDescription"/> contains invalid characters, such as an embedded tab.
		///	-or- <paramref name="throwOnError"/> is true and <paramref name="typeDescription"/> is an empty string.
		///	-or-<paramref name="throwOnError"/> is true and <paramref name="typeDescription"/> represents an array type with an invalid size. -or-<paramref name="typeDescription"/> represents an array of <see cref="TypedReference"/>.
		///	</exception>
		/// <exception cref="ArgumentException">throwOnError is true and typeName contains invalid syntax.</exception>
		/// <exception cref="System.IO.FileNotFoundException"><paramref name="throwOnError"/> is true and the assembly or one of its dependencies was not found.</exception>
		/// <exception cref="System.IO.FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.</exception>
		/// <exception cref="BadImageFormatException">The assembly or one of its dependencies is not valid.</exception>
		public static Type GetType(string typeDescription, bool throwOnError)
		{
			if (String.IsNullOrWhiteSpace(typeDescription))
			{
				throw new ArgumentNullException("typeDescription");
			}

			Type type = Type.GetType(typeDescription, throwOnError, true);
			if (type == null)
			{
				string[] splits = typeDescription.Split(';');
				if (splits.Length >= 2)
				{
					string assemblyName = splits[0].Trim();
					string typeName = splits[1].Trim();

					type = Type.GetType(typeName + ", " + assemblyName, throwOnError, true);
				}
			}

			return type;
		}

		/// <summary>
		/// Loads one or more assemblies that reference one or more assemblies name starts with the specified partial name.
		/// </summary>
		/// <param name="partialAssemblyName">The name the assembly starts with.</param>
		public static void LoadReferencingAssemblies(string partialAssemblyName)
		{
			string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).AbsolutePath);
			foreach (string file in Directory.GetFiles(path, "*.dll"))
			{
				var rassembly = Assembly.ReflectionOnlyLoadFrom(file);
				if (rassembly.GetReferencedAssemblies().Any(a => a.Name.StartsWith(partialAssemblyName)))
				{
					Assembly.Load(rassembly.GetName());
				}
			}
		}

		/// <summary>
		/// Tries to convert the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the object we need to convert.</typeparam>
		/// <param name="value">The value to convert.</param>
		/// <param name="result">The output of the result.</param>
		/// <returns>Returns true if conversion happened correctly, false otherwise.</returns>
		public static bool TryConvert<T>(object value, out T result)
		{
			object output;
			var converted = TypeUtil.TryConvert(typeof(T), value, out output);
			if (converted)
			{
				result = (T)output;
			}
			else
			{
				result = default(T);
			}

			return converted;
		}

		/// <summary>
		/// Tries to convert the specified value
		/// </summary>
		/// <param name="type">The type of the object we need to convert</param>
		/// <param name="value">The value to convert</param>
		/// <param name="output">The output of the conversion</param>
		/// <returns>Returns true if conversion happened correctly, false otherwise</returns>
		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "It is not all instances that you can use generics.")]
		public static bool TryConvert(Type type, object value, out object output)
		{
			output = null;
			try
			{
				output = TypeUtil.Convert(type, value);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}
			catch (NotSupportedException)
			{
				return false;
			}
		}

		private static PropertyDescriptor GetPropertyDescriptor(object item, string propertyName, out object source)
		{
			source = item;

			PropertyDescriptor pd = TypeUtil.GetDescriptors(item.GetType())[propertyName];
			if (pd == null)
			{
				string[] splits = propertyName.Split('.');
				for (int i = 0; i < splits.Length; i++)
				{
					pd = TypeUtil.GetDescriptors(source.GetType())[splits[i]];
					if (pd == null)
					{
						return null;
					}

					if (i != splits.Length - 1)
					{
						source = pd.GetValue(source);
						if (source == null)
						{
							return null;
						}
					}
				}
			}

			return pd;
		}

		private static PropertyDescriptorCollection GetDescriptors(Type type)
		{
			if (!typeDescriptorCache.ContainsKey(type))
			{
				typeDescriptorCache.Add(type, TypeDescriptor.GetProperties(type));
			}

			return typeDescriptorCache[type];
		}

		#endregion
	}
}