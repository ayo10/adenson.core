using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace System
{
	/// <summary>
	/// Utility classes for types
	/// </summary>
	public static partial class TypeUtil
	{
		/// <summary>
		/// Finds all types in the current domain where <paramref name="type"/> is assignable from (or implements) (but not equal to) (ie. type.IsAssignableFrom(t) via <see cref="Type.IsAssignableFrom(Type)"/>).
		/// </summary>
		/// <param name="type">The type to find.</param>
		/// <returns>An enumerable list of types found.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
		public static IEnumerable<Type> FindAssignableFrom(Type type)
		{
			Arg.IsNotNull(type);
			return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => type != t && type.IsAssignableFrom(t)));
		}

		/// <summary>
		/// Finds one or more assemblies that reference one or more assemblies name starts with the specified partial name in the current app domain directory.
		/// </summary>
		/// <param name="partialAssemblyName">The name the assembly starts with.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="partialAssemblyName"/> is null.</exception>
		/// <returns>A list of found assemblies.</returns>
		public static IEnumerable<Assembly> FindReferencingAssemblies(string partialAssemblyName)
		{
			return TypeUtil.FindReferencingAssemblies(partialAssemblyName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath));
		}

		/// <summary>
		/// Finds one or more assemblies that reference one or more assemblies name starts with the specified partial name in the specified directory (uses reflection only load).
		/// </summary>
		/// <param name="partialAssemblyName">The name the assembly starts with.</param>
		/// <param name="directory">The directory look for files.</param>
		/// <returns>List of found assemblies.</returns>
		/// <exception cref="ArgumentNullException">If either <paramref name="partialAssemblyName"/> or <paramref name="directory"/> is null.</exception>
		/// <exception cref="DirectoryNotFoundException">If <paramref name="directory"/> does not exist.</exception>
		public static IEnumerable<Assembly> FindReferencingAssemblies(string partialAssemblyName, string directory)
		{
			foreach (string file in Directory.GetFiles(directory, "*.dll").Union(Directory.GetFiles(directory, "*.exe")))
			{
				Assembly assembly = null;
				try
				{
					assembly = Assembly.ReflectionOnlyLoadFrom(file);
				}
				catch (BadImageFormatException)
				{
				}

				if (assembly != null && assembly.GetReferencedAssemblies().Any(a => a.Name.StartsWith(partialAssemblyName)))
				{
					yield return assembly;
				}
			}
		}

		/// <summary>
		/// Loads one or more assemblies that reference one or more assemblies name starts with the specified partial name in the current <see cref="AppDomain.BaseDirectory"/> and <see cref="AppDomain.RelativeSearchPath"/>.
		/// </summary>
		/// <param name="partialAssemblyName">The name the assembly starts with.</param>
		/// <exception cref="ArgumentNullException">If either <paramref name="partialAssemblyName"/> is null.</exception>
		public static void LoadReferencingAssemblies(string partialAssemblyName)
		{
			TypeUtil.LoadReferencingAssemblies(partialAssemblyName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath));
		}

		/// <summary>
		/// Loads one or more assemblies that reference one or more assemblies name starts with the specified partial name.
		/// </summary>
		/// <param name="partialAssemblyName">The name the assembly starts with.</param>
		/// <param name="directory">The directory look for files.</param>
		/// <exception cref="ArgumentNullException">If either <paramref name="partialAssemblyName"/> or <paramref name="directory"/> is null.</exception>
		/// <exception cref="DirectoryNotFoundException">If <paramref name="directory"/> does not exist.</exception>
		public static void LoadReferencingAssemblies(string partialAssemblyName, string directory)
		{
			var names = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName()).ToList();
			foreach (Assembly assembly in TypeUtil.FindReferencingAssemblies(partialAssemblyName, directory))
			{
				var name = assembly.GetName();
				if (!names.Contains(name))
				{
					Assembly.Load(name);
				}
			}
		}
	}
}