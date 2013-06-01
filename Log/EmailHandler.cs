using System;
using System.Configuration;
using System.Net.Mail;
using System.Xml.Linq;
using Adenson.Configuration;

namespace Adenson.Log
{
	/// <summary>
	/// Logger email settings.
	/// </summary>
	public sealed class EmailHandler : BaseHandler
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailHandler"/> class.
		/// </summary>
		/// <param name="element">The element to initialize the class with.</param>
		internal EmailHandler(SettingsConfiguration.HandlerElement element) : base()
		{
			this.From = element.GetValue("From", "errors@Logger");
			this.Subject = element.GetValue("Subject", "Adenson.Log.Logger");
			this.To = element.GetValue("To", null);

			if (String.IsNullOrEmpty(this.To))
			{
				throw new ConfigurationErrorsException(Exceptions.EmailToAddressInvalid);
			}
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

		/// <summary>
		/// Writes the log to the diagnostics trace (using <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True, regardless.</returns>
		public override bool Write(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			return SmtpUtil.TrySend(this.From, this.To, this.Subject, entry.ToString(), false);
		}

		#endregion
	}
}