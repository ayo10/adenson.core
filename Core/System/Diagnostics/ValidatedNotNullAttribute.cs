using System;

namespace System.Diagnostics
{
	/// <summary>
	/// Exists for the sole reason to suppress CA1062 warnings for Check
	/// </summary>
	internal sealed class ValidatedNotNullAttribute : Attribute
	{
	}
}