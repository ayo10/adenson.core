using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
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
		/// <returns>True if the email was sent successfully, false otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031", Justification = "Returns true if the write was successful, false otherwise.")]
		public override bool Write(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			using (SmtpClient s = new SmtpClient())
			{
				MailMessage m = new MailMessage
				{
					From = new MailAddress(this.From),
					Subject = this.Subject,
					Body = entry.ToString(),
					IsBodyHtml = true
				};

				m.To.Add(this.To);
				
				try
				{
					s.SendAsync(m, null);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		#endregion
	}
}