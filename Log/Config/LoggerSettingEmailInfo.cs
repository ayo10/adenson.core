using System;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log.Config
{
	internal sealed class LoggerSettingEmailInfo : XElementSettingBase
	{
		#region Constructor

		public LoggerSettingEmailInfo(XElement element) : base(element)
		{
			this.From = this.GetValue("From", "errors@Adenson.Log.Logger");
			this.Subject = this.GetValue("Subject", "Adenson.Log.Logger");
			this.To = this.GetValue<string>("To", null);
		}

		#endregion
		#region Properties

		public string From
		{
			get;
			set;
		}

		public string Subject
		{
			get;
			set;
		}

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