using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adenson.Log;

namespace System.Net.Mail
{
	/// <summary>
	/// SMTP Helper utility class
	/// </summary>
	public static class SmtpUtil
	{
		#region Variables
		private static Logger logger = Logger.GetLogger(typeof(SmtpUtil));
		#endregion
		#region Methods

		/// <summary>
		/// Sends an email using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="message">MailMessage to send</param>
		/// <exception cref="ArgumentNullException">if message is null</exception>
		public static void Send(MailMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			SmtpUtil.Send(null, message, true);
		}

		/// <summary>
		/// Sends an email using MailMessage object
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="message">MailMessage to send</param>
		/// <exception cref="ArgumentNullException">if smtpHost is null or whitespace, OR message is null</exception>
		public static void Send(string smtpHost, MailMessage message)
		{
			if (StringUtil.IsNullOrWhiteSpace(smtpHost))
			{
				throw new ArgumentNullException("smtpHost");
			}

			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			SmtpUtil.Send(smtpHost, message, false);
		}

		/// <summary>
		/// Sends an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">from is null, OR to is null.</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty.</exception>
		/// <exception cref="FormatException">from, OR to is malformed.</exception>
		public static void Send(string from, string to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(null, SmtpUtil.ComposelMessage(from, new string[] { to }, subject, message, isHtml), false);
		}

		/// <summary>
		/// Sends an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">from is null, OR to is null, OR any item in to is null</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty, OR any item in to is null.</exception>
		/// <exception cref="FormatException">from, OR to is malformed, OR any item in to is malformed.</exception>
		public static void Send(string from, string[] to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(null, SmtpUtil.ComposelMessage(from, to, subject, message, isHtml), false);
		}

		/// <summary>
		/// Sends an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="smtpHost">The SMTP Host name/address</param>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">if smtpHost is null or whitespace, OR from is null, OR to is null.</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty.</exception>
		/// <exception cref="FormatException">from, OR to is malformed.</exception>
		public static void Send(string smtpHost, string from, string to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(smtpHost, SmtpUtil.ComposelMessage(from, new string[] { to }, subject, message, isHtml), false);
		}

		/// <summary>
		/// Sends an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="smtpHost">The SMTP Host name/address</param>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">if smtpHost is null or whitespace, OR from is null, OR to is null, OR any item in to is null</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty, OR any item in to is null.</exception>
		/// <exception cref="FormatException">from, OR to is malformed, OR any item in to is malformed.</exception>
		public static void Send(string smtpHost, string from, string[] to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(smtpHost, SmtpUtil.ComposelMessage(from, to, subject, message, isHtml), false);
		}

		/// <summary>
		/// Sends email asynchronously, using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="message">MailMessage to send</param>
		/// <exception cref="ArgumentNullException">if message is null</exception>
		public static void SendAsync(MailMessage message)
		{
			SmtpUtil.Send(null, message, true);
		}

		/// <summary>
		/// Sends an email asynchronously using MailMessage object
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="message">MailMessage Object to use</param>
		/// <exception cref="ArgumentNullException">if smtpHost is null or whitespace, OR message is null</exception>
		public static void SendAsync(string smtpHost, MailMessage message)
		{
			if (StringUtil.IsNullOrWhiteSpace(smtpHost))
			{
				throw new ArgumentNullException("smtpHost");
			}

			SmtpUtil.Send(smtpHost, message, true);
		}

		/// <summary>
		/// Sends email asynchronously, using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">from is null, OR to is null.</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty.</exception>
		/// <exception cref="FormatException">from, OR to is malformed.</exception>
		public static void SendAsync(string from, string to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(null, SmtpUtil.ComposelMessage(from, new string[] { to }, subject, message, isHtml), true);
		}

		/// <summary>
		/// Sends email asynchronously, using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">from is null, OR to is null, OR any item in to is null</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty, OR any item in to is null.</exception>
		/// <exception cref="FormatException">from, OR to is malformed, OR any item in to is malformed.</exception>
		public static void SendAsync(string from, string[] to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(null, SmtpUtil.ComposelMessage(from, to, subject, message, isHtml), true);
		}

		/// <summary>
		/// Sends email asynchronously, using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">from is null, OR to is null.</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty.</exception>
		/// <exception cref="FormatException">from, OR to is malformed.</exception>
		public static void SendAsync(string smtpHost, string from, string to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(smtpHost, SmtpUtil.ComposelMessage(from, new string[] { to }, subject, message, isHtml), true);
		}

		/// <summary>
		/// Sends email asynchronously, using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <exception cref="ArgumentNullException">from is null, OR to is null, OR any item in to is null</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty, OR any item in to is null.</exception>
		/// <exception cref="FormatException">from, OR to is malformed, OR any item in to is malformed.</exception>
		public static void SendAsync(string smtpHost, string from, string[] to, string subject, string message, bool isHtml)
		{
			SmtpUtil.Send(smtpHost, SmtpUtil.ComposelMessage(from, to, subject, message, isHtml), true);
		}

		/// <summary>
		/// Tries to send an email using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="message">MailMessage Object to use</param>
		/// <returns>True if there were no exceptions, false otherwise</returns>
		/// <exception cref="ArgumentNullException">if message is null</exception>
		public static bool TrySend(MailMessage message)
		{
			return SmtpUtil.TrySend(null, message, true);
		}

		/// <summary>
		/// Tries to send an email using MailMessage object
		/// </summary>
		/// <param name="smtpHost">The smtp host address to use</param>
		/// <param name="message">MailMessage Object to use</param>
		/// <returns>True if there were no exceptions, false otherwise</returns>
		public static bool TrySend(string smtpHost, MailMessage message)
		{
			if (StringUtil.IsNullOrWhiteSpace(smtpHost))
			{
				throw new ArgumentNullException("smtpHost");
			}

			return SmtpUtil.TrySend(smtpHost, message, false);
		}

		/// <summary>
		/// Tries to send an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <returns>True if there were no exceptions, false otherwise</returns>
		/// <exception cref="ArgumentNullException">from is null, OR to is null.</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty.</exception>
		/// <exception cref="FormatException">from, OR to is malformed.</exception>
		public static bool TrySend(string from, string to, string subject, string message, bool isHtml)
		{
			return SmtpUtil.TrySend(null, SmtpUtil.ComposelMessage(from, new string[] { to }, subject, message, isHtml), false);
		}

		/// <summary>
		/// Tries to send an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <returns>True if there were no exceptions, false otherwise</returns>
		/// <exception cref="ArgumentNullException">from is null, OR to is null, OR any item in to is null</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty, OR any item in to is null.</exception>
		/// <exception cref="FormatException">from, OR to is malformed, OR any item in to is malformed.</exception>
		public static bool TrySend(string from, string[] to, string subject, string message, bool isHtml)
		{
			return SmtpUtil.TrySend(null, SmtpUtil.ComposelMessage(from, to, subject, message, isHtml), false);
		}

		/// <summary>
		/// Tries to send an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="smtpHost">The SMTP Host name/address</param>
		/// <param name="from">The from field</param>
		/// <param name="to">The to field</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <returns>True if there were no exceptions, false otherwise</returns>
		/// <exception cref="ArgumentNullException">from is null, OR to is null.</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty.</exception>
		/// <exception cref="FormatException">from, OR to is malformed.</exception>
		public static bool TrySend(string smtpHost, string from, string to, string subject, string message, bool isHtml)
		{
			return SmtpUtil.TrySend(smtpHost, SmtpUtil.ComposelMessage(from, new string[] { to }, subject, message, isHtml), false);
		}

		/// <summary>
		/// Tries to send an email using passed variables using configuration file settings <see cref="System.Net.Mail.SmtpClient()"/>.
		/// </summary>
		/// <param name="smtpHost">The SMTP Host name/address</param>
		/// <param name="from">The from field</param>
		/// <param name="to">The to fields</param>
		/// <param name="subject">The email subject</param>
		/// <param name="message">The email body</param>
		/// <param name="isHtml">If email body is in HTML format, or not.</param>
		/// <returns>True if there were no exceptions, false otherwise</returns>
		/// <exception cref="ArgumentNullException">from is null, OR to is null, OR any item in to is null</exception>
		/// <exception cref="ArgumentException">from is String.Empty, OR to is String.Empty, OR any item in to is null.</exception>
		/// <exception cref="FormatException">from, OR to is malformed, OR any item in to is malformed.</exception>
		public static bool TrySend(string smtpHost, string from, string[] to, string subject, string message, bool isHtml)
		{
			return SmtpUtil.TrySend(smtpHost, SmtpUtil.ComposelMessage(from, to, subject, message, isHtml), false);
		}

		private static MailMessage ComposelMessage(string from, string[] to, string subject, string message, bool isHtml)
		{
			if (to == null || to.Length == 0)
			{
				throw new ArgumentNullException("to", Adenson.Exceptions.EmailAddressInvalid);
			}

			if (to.Any(s => StringUtil.IsNullOrWhiteSpace(s)))
			{
				throw new ArgumentException(Adenson.Exceptions.EmailAddressInvalid, "to");
			}

			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(from);
			mailMessage.Subject = subject;
			mailMessage.Body = message;
			mailMessage.IsBodyHtml = isHtml;
			foreach (string str in to)
			{
				mailMessage.To.Add(str);
			}

			return mailMessage;
		}

		private static void Send(string smtpHost, MailMessage message, bool sendAsync)
		{
			SmtpClient smtp = null;
			if (StringUtil.IsNullOrWhiteSpace(smtpHost))
			{
				smtp = new SmtpClient();
			}
			else
			{
				smtp = new SmtpClient(smtpHost);
			}

			logger.Debug("Trying to send mail to '{1}' using server '{0}' (from '{2}')", smtp.Host, message.To, message.From);
			if (sendAsync)
			{
				smtp.SendAsync(message, null);
			}
			else
			{
				smtp.Send(message);
			}

			logger.Debug("Send Success");
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Method is a try method")]
		private static bool TrySend(string smtpHost, MailMessage message, bool sendAsync)
		{
			try
			{
				SmtpUtil.Send(smtpHost, message, sendAsync);
				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				logger.Debug("Failed sending mail to '{1}' using server '{0}' (from '{2}')", smtpHost, message.To, message.From);
				return false;
			}
		}

		#endregion
	}
}