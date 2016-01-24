using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
	/// <summary>
	/// Exists for the sole reason to suppress CA1062 warnings for Check
	/// </summary>
	internal sealed class ValidatedNotNullAttribute : Attribute
	{
	}
}