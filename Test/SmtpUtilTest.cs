﻿using Adenson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Mail;

namespace Adenson.CoreTest
{
	[TestClass]
	public class SmtpUtilTest
	{
		[TestMethod]
		public void SendTest()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string to = string.Empty; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.Send(smtpHost, from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Send
		///</summary>
		[TestMethod]
		public void SendTest1()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string[] to = null; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.Send(smtpHost, from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Send
		///</summary>
		[TestMethod]
		public void SendTest3()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			MailMessage message = null; // TODO: Initialize to an appropriate value
			SmtpUtil.Send(smtpHost, message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Send
		///</summary>
		[TestMethod]
		public void SendTest4()
		{
			MailMessage message = null; // TODO: Initialize to an appropriate value
			SmtpUtil.Send(message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Send
		///</summary>
		[TestMethod]
		public void SendTest5()
		{
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string[] to = null; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.Send(from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Send
		///</summary>
		[TestMethod]
		public void SendTest6()
		{
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string to = string.Empty; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.Send(from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SendAsync
		///</summary>
		[TestMethod]
		public void SendAsyncTest()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string to = string.Empty; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.SendAsync(smtpHost, from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SendAsync
		///</summary>
		[TestMethod]
		public void SendAsyncTest1()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string[] to = null; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.SendAsync(smtpHost, from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SendAsync
		///</summary>
		[TestMethod]
		public void SendAsyncTest2()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			MailMessage message = null; // TODO: Initialize to an appropriate value
			SmtpUtil.SendAsync(smtpHost, message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SendAsync
		///</summary>
		[TestMethod]
		public void SendAsyncTest3()
		{
			MailMessage message = null; // TODO: Initialize to an appropriate value
			SmtpUtil.SendAsync(message);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SendAsync
		///</summary>
		[TestMethod]
		public void SendAsyncTest4()
		{
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string[] to = null; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.SendAsync(from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SendAsync
		///</summary>
		[TestMethod]
		public void SendAsyncTest5()
		{
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string to = string.Empty; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			SmtpUtil.SendAsync(from, to, subject, message, isHtml);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for TrySend
		///</summary>
		[TestMethod]
		public void TrySendTest()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string[] to = null; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = SmtpUtil.TrySend(smtpHost, from, to, subject, message, isHtml);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TrySend
		///</summary>
		[TestMethod]
		public void TrySendTest2()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string to = string.Empty; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = SmtpUtil.TrySend(smtpHost, from, to, subject, message, isHtml);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TrySend
		///</summary>
		[TestMethod]
		public void TrySendTest3()
		{
			string smtpHost = string.Empty; // TODO: Initialize to an appropriate value
			MailMessage message = null; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = SmtpUtil.TrySend(smtpHost, message);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TrySend
		///</summary>
		[TestMethod]
		public void TrySendTest4()
		{
			MailMessage message = null; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = SmtpUtil.TrySend(message);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TrySend
		///</summary>
		[TestMethod]
		public void TrySendTest5()
		{
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string[] to = null; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = SmtpUtil.TrySend(from, to, subject, message, isHtml);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for TrySend
		///</summary>
		[TestMethod]
		public void TrySendTest6()
		{
			string from = string.Empty; // TODO: Initialize to an appropriate value
			string to = string.Empty; // TODO: Initialize to an appropriate value
			string subject = string.Empty; // TODO: Initialize to an appropriate value
			string message = string.Empty; // TODO: Initialize to an appropriate value
			bool isHtml = false; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = SmtpUtil.TrySend(from, to, subject, message, isHtml);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}