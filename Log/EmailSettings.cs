using System;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log
{
	/// <summary>
	/// Logger email settings.
	/// </summary>
	public sealed class EmailSettings : XElementSettingBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailSettings"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		internal EmailSettings(XElement element) : base(element)
		{
			this.From = this.GetValue("From", "errors@Adenson.Log.Logger");
			this.Subject = this.GetValue("Subject", "Adenson.Log.Logger");
			this.To = this.GetValue<string>("To", null);
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets or sets the from.
		/// </summary>
		public string From
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		public string Subject
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the to.
		/// </summary>
		public string To
		{
			get;
			set;
		}

		#endregion
		#region Methods

		internal bool IsEmpty()
		{
			return StringUtil.IsNullOrWhiteSpace(this.From) || StringUtil.IsNullOrWhiteSpace(this.To);
		}

		#endregion
	}
}