using System;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
	/// Borrowed from http://www.pagedesigners.co.nz/2011/02/asp-net-mvc-3-email-validation-with-unobtrusive-jquery-validation/
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class EmailAttribute : RegularExpressionAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EmailAttribute"/> class.
		/// </summary>
		public EmailAttribute() : base(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
		{
			this.ErrorMessage = "Invalid email. Must be in the form of person@host.part";
		}
	}
}