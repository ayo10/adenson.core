using System;
using System.Configuration;
using System.Diagnostics;
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
		/// <param name="from">The from.</param>
		/// <param name="to">To whom</param>
		/// <param name="subject">The email subject.</param>
		public EmailHandler(string from, string to, string subject) : base()
		{
			this.From = Arg.IsNotNull(from);
			this.To = Arg.IsNotNull(to);
			this.Subject = Arg.IsNotNull(subject);
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
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "MailMessage was disposed!!!.")]
		public override bool Write(LogEntry entry)
		{
			Arg.IsNotNull(entry, "entry");

			#if !NET35
			using (SmtpClient s = new SmtpClient())
			#else
			SmtpClient s = new SmtpClient();
			#endif
			{
				using (MailMessage message = new MailMessage { From = new MailAddress(this.From), Subject = this.Subject, Body = entry.ToString(), IsBodyHtml = true })
				{
					message.To.Add(this.To);
					try
					{
						s.SendAsync(message, null);
						return true;
					}
					catch
					{
						return false;
					}
				}
			}
		}

		#endregion
	}
}