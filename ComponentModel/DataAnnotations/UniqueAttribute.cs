using System;
using System.ComponentModel.DataAnnotations;

namespace Wierra.Annotations
{
	/// <summary>
	/// Marks a property as is unique among objects of the same type
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class UniqueAttribute : ValidationAttribute
	{
	}
}