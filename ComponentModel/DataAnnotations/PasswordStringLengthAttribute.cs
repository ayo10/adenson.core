using System;
using System.ComponentModel.DataAnnotations;

namespace Wierra.Annotations
{
	/// <summary>
	/// Specifies the minimum and maximum length of characters that are allowed in a password data field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class PasswordStringLengthAttribute : StringLengthAttribute
	{
		/// <summary>
		/// Initializes a new instance of the PasswordStringLengthAttribute class, setting max length to 100, min to 4
		/// </summary>
		public PasswordStringLengthAttribute() : base(100)
		{
			this.MinimumLength = 4;
			this.ErrorMessage = "Invalid password length";
		}
	}
}