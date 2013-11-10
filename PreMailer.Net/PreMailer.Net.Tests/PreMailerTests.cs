using Microsoft.VisualStudio.TestTools.UnitTesting;

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

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"height: 100px;width: 100px;"));
		}

		[TestMethod]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px"));
		}

		[TestMethod]
		public void MoveCssInline_CssWithHigherSpecificity_AppliesMoreSpecificCss()
		{
			string input = "<html><head><style type=\"text/css\">#high-imp.test { width: 42px; } .test { width: 150px; }</style></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("style=\"width: 42px;\""));
		}

		[TestMethod]
		public void MoveCssInline_CssWithHigherSpecificityInSeparateStyleTag_AppliesMoreSpecificCss()
		{
			string input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\">.outer .inner .target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("style=\"width: 1337px;\""));
		}
	}
}