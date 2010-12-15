using System;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Xml;
using System.Linq;
using Adenson.Log;
using Adenson.Data;

namespace Adenson.Net
{
	/// <summary>
	/// Simplified mailer class that will attempt to send mail
	/// </summary>
	public static class Mailer
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(Mailer));
		#endregion
		#region Methods

		/// <summary>
		/// Sends an email using MailMessage object
		/// </summary>
		/// <param name="message">MailMessage Object to use</param>
		/// <returns>True if there were no exceptions calling SmtpClient.Send, false otherwise</returns>
		public static bool Send(MailMessage message)
		{
			return Mailer.Send(null, message, false);
		}
		/// <summary>
		/// Sends an email using MailMessage object
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="message">MailMessage Object to use</param>
		/// <returns>True if there were no exceptions calling SmtpClient.Send, false otherwise</returns>
		public static bool Send(string smtpHost, MailMessage message)
		{
			return Send(smtpHost, message, false);
		}
		/// <summary>
		/// Sends an email using MailMessage object
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="message">MailMessage Object to use</param>
		/// <param name="sendAsync">If to call SmtpClient.Send or SmtpClient.SendAsync</param>
		/// <returns>True if there were no exceptions calling SmtpClient.Send, false otherwise</returns>
		public static bool Send(string smtpHost, MailMessage message, bool sendAsync)
		{
			SmtpClient smtp = new SmtpClient(smtpHost);
			if (String.IsNullOrWhiteSpace(smtpHost)) smtp = new SmtpClient();
			else smtp = new SmtpClient(smtpHost);
			try
			{
				logger.Debug("Trying to send mail to '{1}' using server '{0}' (from '{2}')", smtp.Host, message.To, message.From);
				if (sendAsync) smtp.SendAsync(message, null);
				else smtp.Send(message);
				logger.Debug("Send Success");
				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				logger.Debug("Failed sending mail to '{1}' using server '{0}' (from '{2}')", smtp.Host, message.To, message.From);
				return false;
			}
		}
		/// <summary>
		/// Sends an email using passed variables
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <returns>True if there were no exceptions calling SmtpClient.Send, false otherwise</returns>
		public static bool Send(string from, string to, string subject, string message, bool isHtml)
		{
			return Send(from, new string[] { to }, subject, message, isHtml);
		}
		/// <summary>
		/// Sends an email using passed variables
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <returns>True if there were no exceptions calling SmtpClient.Send, false otherwise</returns>
		public static bool Send(string from, string[] to, string subject, string message, bool isHtml)
		{
			return Mailer.Send(ComposeMailMessage(from, to, subject, message, isHtml));
		}
		/// <summary>
		/// Sends email asynchronously, using MailMessage object
		/// </summary>
		/// <param name="message">MailMessage Object to use</param>
		public static bool SendAsync(MailMessage message)
		{
			return Mailer.Send(null, message, true);
		}
		/// <summary>
		/// Sends email asynchronously, using passed variables
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		public static void SendAsync(string from, string to, string subject, string message, bool isHtml)
		{
			SendAsync(from, new string[] { to }, subject, message, isHtml);
		}
		/// <summary>
		/// Sends email asynchronously, using passed variables
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		public static void SendAsync(string from, string[] to, string subject, string message, bool isHtml)
		{
			Mailer.SendAsync(ComposeMailMessage(from, to, subject, message, isHtml));
		}

		private static MailMessage ComposeMailMessage(string from, string[] to, string subject, string message, bool isHtml)
		{
			if (to == null || to.Length == 0) throw new ArgumentNullException("to", Exceptions.EmailAddressInvalid);
			if (to.Any(s => String.IsNullOrWhiteSpace(s))) throw new ArgumentException(Exceptions.EmailAddressInvalid, "to");

			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(from);
			foreach (string str in to) mailMessage.To.Add(str);
			mailMessage.Subject = subject;
			mailMessage.Body = message;
			mailMessage.IsBodyHtml = isHtml;

			return mailMessage;
		}

		#endregion
	}
}