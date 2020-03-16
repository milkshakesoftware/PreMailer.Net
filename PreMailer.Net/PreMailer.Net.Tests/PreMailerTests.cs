using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PreMailer.Net.Downloaders;
using System;
using System.IO;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class PreMailerTests
	{
		[TestMethod]
		public void MoveCssInline_HasStyle_DoesNotBreakImageWidthAttribute()
		{
			var input = "<html><head><style type=\"text/css\">img { }</style></head>" +
			            "<body><img style=\"width: 206px; height: 64px;\" src=\"http://localhost/left.gif\" height=\"64\" WIDTH=\"206\" border=\"0\"></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsFalse(premailedOutput.Html.Contains("width=\"206px\""));
			Assert.IsTrue(premailedOutput.Html.Contains("width=\"206\""));
		}

		[TestMethod]
		public void MoveCssInline_NoStyle_DoesNotBreakImageWidthAttribute()
		{
			var input = "<html><head><style type=\"text/css\"></style></head>" +
			            "<body><img style=\"width: 206px; height: 64px;\" src=\"http://localhost/left.gif\" height=\"64\" WIDTH=\"206\" border=\"0\"></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsFalse(premailedOutput.Html.Contains("width=\"206px\""));
			Assert.IsTrue(premailedOutput.Html.Contains("width=\"206\""));
		}

		[TestMethod]
		public void MoveCssInline_RespectExistingStyleElement()
		{
			var input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<div class=\"test\" style=\"height: 100px;width: 100px"));
		}

		[TestMethod]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			var input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<div class=\"test\" style=\"width: 100px"));
		}

		[TestMethod]
		public void MoveCssInline_CssWithHigherSpecificity_AppliesMoreSpecificCss()
		{
			var input = "<html><head><style type=\"text/css\">#high-imp.test { width: 42px; } .test { width: 150px; }</style></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"width: 42px\""));
		}

		[TestMethod]
		public void MoveCssInline_CssWithHigherSpecificityInSeparateStyleTag_AppliesMoreSpecificCss()
		{
			var input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\">.outer .inner .target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"width: 1337px\""));
		}

		[TestMethod]
		public void MoveCssInline_IgnoreStyleElement_DoesntApplyCss()
		{
			var input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\" id=\"ignore\">.target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false, ignoreElements: "#ignore");

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"width: 42px\""));
		}

		[TestMethod]
		public void MoveCssInline_SupportedPseudoSelector_AppliesCss()
		{
			var input = "<html><head><style type=\"text/css\">li:first-child { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<li style=\"width: 42px\">"));
		}

		[TestMethod]
		public void MoveCssInline_CrazyCssSelector_DoesNotThrowError()
		{
			var input = "<html><head><style type=\"text/css\">li:crazy { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

			try
			{
				PreMailer.MoveCssInline(input);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public void MoveCssInline_SupportedjQuerySelector_AppliesCss()
		{
			var input = "<html><head><style type=\"text/css\">li:first-child { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<li style=\"width: 42px\">target</li>"));
		}

		[TestMethod]
		public void MoveCssInline_UnsupportedSelector_AppliesCss()
		{
			var input = "<html><head><style type=\"text/css\">p:first-letter { width: 42px; }</style></head><body><p>target</p></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<p>target</p>"));
		}

		[TestMethod]
		public void MoveCssInline_KeepStyleElementsIgnoreElementsMatchesStyleElement_DoesntRemoveScriptTag()
		{
			var input = "<html><head><style id=\"ignore\" type=\"text/css\">li:before { width: 42px; }</style></head><body><div class=\"target\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, removeStyleElements: false, ignoreElements: "#ignore");

			Assert.IsTrue(premailedOutput.Html.Contains("<style id=\"ignore\" type=\"text/css\">"));
		}

		[TestMethod]
		public void MoveCssInline_MultipleSelectors_HonorsIndividualSpecificity()
		{
			var input = "<html><head><style type=\"text/css\">p,li,tr.pub-heading td,tr.pub-footer td,tr.footer-heading td { font-size: 12px; line-height: 16px; } td.disclaimer p {font-size: 11px;} </style></head><body><table><tr class=\"pub-heading\"><td class=\"disclaimer\"><p></p></td></tr></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<p style=\"font-size: 11px;line-height: 16px\"></p>"));
		}

		[TestMethod]
		public void MoveCssInline_ImportantFlag_HonorsImportantFlagInStylesheet()
		{
			var input = "<style>div { color: blue !important; }</style><div style=\"color: red\">Red</div>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"color: blue"));
		}

		[TestMethod]
		public void MoveCssInline_ImportantFlag_HonorsImportantFlagInline()
		{
			var input = "<style>div { color: blue !important; }</style><div style=\"color: red !important\">Red</div>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"color: red"));
		}

		[TestMethod]
		public void MoveCssInline_AbsoluteBackgroundUrl_ShouldNotBeCleanedAsComment()
		{
			var input = "<style>div { background: url('http://my.web.site.com/Content/email/content.png') repeat-y }</style><div></div>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"background: url('http://my.web.site.com/Content/email/content.png') repeat-y\"></div>"));
		}

		public void MoveCssInline_SupportedMediaAttribute_InlinesAsNormal()
		{
			var input = "<html><head><style type=\"text/css\" media=\"screen\">div { width: 100% }</style></head><body><div>Target</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"width: 100%;\">Target</div>"));
		}

		[TestMethod]
		public void MoveCssInline_UnsupportedMediaAttribute_IgnoresStyles()
		{
			var input = "<html><head><style type=\"text/css\" media=\"print\">div { width: 100% }</style></head><body><div>Target</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input);

			Assert.IsTrue(premailedOutput.Html.Contains("<div>Target</div>"));
		}

		[TestMethod]
		public void MoveCssInline_AddBgColorStyle()
		{
			var input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; }</style></head><body><table><tr><td class=\"test\" bgcolor=\"\"></td></tr></table></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" bgcolor=\"#f1f1f1\" style=\"background-color: #f1f1f1\">"));
		}

		[TestMethod]
		public void MoveCssInline_AddSpecial()
		{
			var input = "<html><head><style type=\"text/css\">.test { padding: 7px; -premailer-cellspacing: 5; -premailer-width: 14%; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" style=\"padding: 7px\" cellspacing=\"5\" width=\"14%\">"), "Actual: " + premailedOutput.Html);
		}

		[TestMethod]
		public void MoveCssInline_AddSpecial_RemoveEmptyStyle()
		{
			var input = "<html><head><style type=\"text/css\">.test { -premailer-cellspacing: 5; -premailer-width: 14%; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" cellspacing=\"5\" width=\"14%\">"), "Actual: " + premailedOutput.Html);
		}

		[TestMethod]
		public void MoveCssInline_AddBgColorStyle_IgnoreElementWithBackgroundColorAndNoBgColor()
		{
			var input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" style=\"background-color: #f1f1f1\""));
		}

		[TestMethod]
		public void MoveCssInline_NoStyleElement_StylesGivenInCSSParam_InlinesThat()
		{
			var input = "<html><head></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false, css: ".test { background-color:#f1f1f1; }");

			Assert.IsTrue(premailedOutput.Html.Contains("<td class=\"test\" style=\"background-color: #f1f1f1\""));
		}

		[TestMethod]
		public void MoveCssInline_StripsClassAttributes()
		{
			var input = "<html><head></head><body><table id=\"testTable\"><tr><td class=\"test\"></td></tr></table></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false, css: ".test { background-color:#f1f1f1; }", stripIdAndClassAttributes: true);

			Assert.IsTrue(premailedOutput.Html.Contains("<td style=\"background-color: #f1f1f1\""));
		}

		[TestMethod]
		public void MoveCssInline_StripsIdAttributes()
		{
			var input = "<html><head><style type=\"text/css\">#high-imp.test { width: 42px; } .test { width: 150px; }</style></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false, stripIdAndClassAttributes: true);

			Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"width: 42px\">"));
		}

		[TestMethod]
		public void MoveCssInline_StripsComments()
		{
			var input = "<html><head></head><body><!--This should be removed--></body></html>";
			var expected = "<html><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, removeComments: true);

			Assert.IsTrue(expected == premailedOutput.Html);
		}

		[TestMethod]
		public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_InSameBlock()
		{
			var input1 = "<html><head><style type=\"text/css\">table.acolor td { color: #0F0; } table.bcolor td { color: #00F; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";
			var input2 = "<html><head><style type=\"text/css\">table.bcolor td { color: #00F; } table.acolor td { color: #0F0; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";

			var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
			var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

			Assert.IsTrue(premailedOutput1.Html.Contains("<td style=\"color: #00F\">test</td>"));
			Assert.IsTrue(premailedOutput2.Html.Contains("<td style=\"color: #0F0\">test</td>"));
		}

		[TestMethod]
		public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_Nested_InSameBlock()
		{
			var input1 = "<html><head><style type=\"text/css\">table.child td { color: #00F; } table.parent td { color: #0F0; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";
			var input2 = "<html><head><style type=\"text/css\">table.parent td { color: #0F0; } table.child td { color: #00F; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";

			var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
			var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

			Assert.IsTrue(premailedOutput1.Html.Contains("<td style=\"color: #0F0\">test</td>"));
			Assert.IsTrue(premailedOutput2.Html.Contains("<td style=\"color: #00F\">test</td>"));
		}

		[TestMethod]
		public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_InSeparateBlocks()
		{
			var input1 = "<html><head><style type=\"text/css\">table.acolor td { color: #00F; }</style><style type=\"text/css\">table.bcolor td { color: #0F0; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";
			var input2 = "<html><head><style type=\"text/css\">table.bcolor td { color: #0F0; }</style><style type=\"text/css\">table.acolor td { color: #00F; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";

			var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
			var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

			Assert.IsTrue(premailedOutput1.Html.Contains("<td style=\"color: #0F0\">test</td>"));
			Assert.IsTrue(premailedOutput2.Html.Contains("<td style=\"color: #00F\">test</td>"));
		}

		[TestMethod]
		public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_Nested_InSeparateBlocks()
		{
			var input1 = "<html><head><style type=\"text/css\">table.child td { color: #00F; } table.parent td { color: #00F; }</style><style type=\"text/css\">table.parent td { color: #0F0; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";
			var input2 = "<html><head><style type=\"text/css\">table.parent td { color: #0F0; } table.child td { color: #0F0; }</style><style type=\"text/css\">table.child td { color: #00F; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";

			var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
			var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

			Assert.IsTrue(premailedOutput1.Html.Contains("<td style=\"color: #0F0\">test</td>"));
			Assert.IsTrue(premailedOutput2.Html.Contains("<td style=\"color: #00F\">test</td>"));
		}

		[TestMethod]
		public void AddAnalyticsTags_AddsTags()
		{
			const string input = @"<div><a href=""http://blah.com/someurl"">Some URL</a><a>No href</a></div><div><a href=""http://blah.com/someurl?extra=1"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div>";
			const string expected = @"<html><head></head><body><div><a href=""http://blah.com/someurl?utm_source=source&amp;utm_medium=medium&amp;utm_campaign=campaign&amp;utm_content=content"">Some URL</a><a>No href</a></div><div><a href=""http://blah.com/someurl?extra=1&amp;utm_source=source&amp;utm_medium=medium&amp;utm_campaign=campaign&amp;utm_content=content"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div></body></html>";
			var premailedOutput = new PreMailer(input)
				.AddAnalyticsTags("source", "medium", "campaign", "content")
				.MoveCssInline();
			Assert.IsTrue(expected == premailedOutput.Html);
		}

		[TestMethod]
		public void AddAnalyticsTags_AddsTagsAndExcludesDomain()
		{
			const string input = @"<div><a href=""http://www.blah.com/someurl"">Some URL</a><a>No href</a></div><div><a href=""https://www.nomatch.com/someurl?extra=1"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div>";
			const string expected = @"<html><head></head><body><div><a href=""http://www.blah.com/someurl?utm_source=source&amp;utm_medium=medium&amp;utm_campaign=campaign&amp;utm_content=content"">Some URL</a><a>No href</a></div><div><a href=""https://www.nomatch.com/someurl?extra=1"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div></body></html>";
			var premailedOutput = new PreMailer(input)
				.AddAnalyticsTags("source", "medium", "campaign", "content", "www.Blah.com")
				.MoveCssInline();
			Assert.IsTrue(expected == premailedOutput.Html);
		}

		[TestMethod]
		public void ContainsLinkCssElement_DownloadsCss()
		{
			var mockDownloader = new Mock<IWebDownloader>();
			mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".a { display: block; }");
			WebDownloader.SharedDownloader = mockDownloader.Object;

			var baseUri = new Uri("http://a.com");
			var fullUrl = new Uri(baseUri, "b.css");
			var input = $"<html><head><link href=\"{fullUrl}\"></link></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			var sut = new PreMailer(input, baseUri);
			sut.MoveCssInline();

			mockDownloader.Verify(d => d.DownloadString(fullUrl));
		}

		[TestMethod]
		public void ContainsLinkCssElement_Bundle_DownloadsCss()
		{
			var mockDownloader = new Mock<IWebDownloader>();
			mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".a { display: block; }");
			WebDownloader.SharedDownloader = mockDownloader.Object;

			var baseUri = new Uri("http://a.com");
			var fullUrl = new Uri(baseUri, "/Content/css?v=7V7TZzP9Wo7LiH9_q-r5mRBdC_N0lA_YJpRL_1V424E1");
			var input = $"<html><head><link href=\"{fullUrl}\" rel=\"stylesheet\"></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			var sut = new PreMailer(input, baseUri);
			sut.MoveCssInline();

			mockDownloader.Verify(d => d.DownloadString(fullUrl));
		}

		[TestMethod]
		public void ContainsLinkCssElement_NotCssFile_DoNotDownload()
		{
			var mockDownloader = new Mock<IWebDownloader>();
			mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".a { display: block; }");
			WebDownloader.SharedDownloader = mockDownloader.Object;

			var baseUri = new Uri("http://a.com");
			var fullUrl = new Uri(baseUri, "b.bs");
			var input = $"<html><head><link href=\"{fullUrl}\"></link></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

			var sut = new PreMailer(input, baseUri);
			sut.MoveCssInline();

			mockDownloader.Verify(d => d.DownloadString(It.IsAny<Uri>()), Times.Never());
		}

		[TestMethod]
		public void ContainsLinkCssElement_DownloadsCss_InlinesContent()
		{
			var mockDownloader = new Mock<IWebDownloader>();
			mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".test { width: 150px; }");
			WebDownloader.SharedDownloader = mockDownloader.Object;

			var input = "<html><head><link href=\"http://a.com/b.css\"></link></head><body><div class=\"test\">test</div></body></html>";

			var sut = new PreMailer(input, new Uri("http://a.com"));
			var premailedOutput = sut.MoveCssInline();

			Assert.IsTrue(premailedOutput.Html.Contains("<div class=\"test\" style=\"width: 150px\">"));
		}

		[TestMethod]
		public void ContainsKeyframeCSS_InlinesCSSWithOutError()
		{
			var keyframeAnimation = @"
				@keyframes mymove {
						0%   {top: 0px;}
						25%  {top: 200px;}
						75%  {top: 50px}
						100% {top: 100px;}
				}
			";

			var input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; } " + keyframeAnimation + "</style></head><body><div class=\"test\">test</div></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.Contains("<div class=\"test\" style=\"background-color: #f1f1f1\""));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType()
		{
			var input = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_HTML5()
		{
			var docType = "<!DOCTYPE html>";
			var input = $"{docType}<html><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html>"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_HTML401_Strict()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">";
			var input = $"{docType}<html><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html>"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_HTML401_Transitional()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">";
			var input = $"{docType}<html><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html>"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_HTML401_Frameset()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\">";
			var input = $"{docType}<html><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html>"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_XHTML10_Strict()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";
			var input = $"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\">"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_XHTML10_Transitional()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
			var input = $"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\">"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_XHTML10_Frameset()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\">";
			var input = $"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\">"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesDocType_XHTML11()
		{
			var docType = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";
			var input = $"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith($"{docType}<html xmlns=\"http://www.w3.org/1999/xhtml\">"));
		}

		[TestMethod]
		public void MoveCSSInline_PreservesXMLNamespace()
		{
			var input = "<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\"><head></head><body></body></html>";

			var premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Html.StartsWith("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">"));
		}

		[TestMethod]
		public void MoveCSSInline_MergingTwoValidCssRules()
		{
			var input = @"<html>
<head>
<style><!--
/* Font Definitions */
p.MsoNormal
  {margin:0cm;}
p
  {mso-style-priority:99;}
--></style>
</head>
<body>
<div>
<p class=""MsoNormal""><span style=""font-family:Source Sans Pro,serif"">Line1</span></p>
</div>
</body>
</html>";

			var premailedOutput = PreMailer.MoveCssInline(input, true, null, null);

			Assert.IsTrue(premailedOutput.Html.Contains("style=\"mso-style-priority: 99;margin: 0cm\""));
		}

		[TestMethod]
		public void MoveCSSInline_AcceptsStream()
		{
			var input = "<html><head><style type=\"text/css\" media=\"screen\">div { width: 100% }</style></head><body><div>Target</div></body></html>";
			using (var stream = new MemoryStream())
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(input);
					writer.Flush();
					stream.Position = 0;
					var premailedOutput = PreMailer.MoveCssInline(stream);

					Assert.IsTrue(premailedOutput.Html.Contains("<div style=\"width: 100%\">Target</div>"));
				}
			}
		}
	}
}
