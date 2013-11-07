using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class PreMailerTests
	{
		private PreMailer sut;

		[TestInitialize]
		public void TestInitialize()
		{
			this.sut = new PreMailer();
		}

		[TestMethod]
		public void MoveCssInline_RespectExistingStyleElement()
		{
			string input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px; height: 100px;"));
		}

		[TestMethod]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px"));
		}
	}
}