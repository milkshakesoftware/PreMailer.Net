﻿using System;
using PreMailer.Net.Downloaders;
using Moq;
using Xunit;

namespace PreMailer.Net.Tests
{

    public class PreMailerTests
    {
        [Fact]
        public void MoveCssInline_HasStyle_DoesNotBreakImageWidthAttribute()
        {
            string input = "<html><head><style type=\"text/css\">img { }</style></head>" +
                            "<body><img style=\"width: 206px; height: 64px;\" src=\"http://localhost/left.gif\" height=\"64\" WIDTH=\"206\" border=\"0\"></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.False(premailedOutput.Html.Contains("width=\"206px\""));
            Assert.True(premailedOutput.Html.Contains("width=\"206\""));
        }

        [Fact]
        public void MoveCssInline_NoStyle_DoesNotBreakImageWidthAttribute()
        {
            string input = "<html><head><style type=\"text/css\"></style></head>" +
                            "<body><img style=\"width: 206px; height: 64px;\" src=\"http://localhost/left.gif\" height=\"64\" WIDTH=\"206\" border=\"0\"></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.False(premailedOutput.Html.Contains("width=\"206px\""));
            Assert.True(premailedOutput.Html.Contains("width=\"206\""));
        }

        [Fact]
        public void MoveCssInline_RespectExistingStyleElement()
        {
            string input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<div class=\"test\" style=\"height: 100px;width: 100px"));
        }

        [Fact]
        public void MoveCssInline_InlineStyleElementTakesPrecedence()
        {
            string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<div class=\"test\" style=\"width: 100px"));
        }

        [Fact]
        public void MoveCssInline_CssWithHigherSpecificity_AppliesMoreSpecificCss()
        {
            string input = "<html><head><style type=\"text/css\">#high-imp.test { width: 42px; } .test { width: 150px; }</style></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("style=\"width: 42px\""));
        }

        [Fact]
        public void MoveCssInline_CssWithHigherSpecificityInSeparateStyleTag_AppliesMoreSpecificCss()
        {
            string input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\">.outer .inner .target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("style=\"width: 1337px\""));
        }

        [Fact]
        public void MoveCssInline_IgnoreStyleElement_DoesntApplyCss()
        {
            string input = "<html><head><style type=\"text/css\">.target { width: 42px; }</style><style type=\"text/css\" id=\"ignore\">.target { width: 1337px; }</style></head><body><div class=\"outer\"><div class=\"inner\"><div class=\"target\">test</div></div></div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false, ignoreElements: "#ignore");

            Assert.True(premailedOutput.Html.Contains("style=\"width: 42px\""));
        }

        [Fact]
        public void MoveCssInline_SupportedPseudoSelector_AppliesCss()
        {
            string input = "<html><head><style type=\"text/css\">li:first-child { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<li style=\"width: 42px\">"));
        }

        [Fact]
        public void MoveCssInline_CrazyCssSelector_DoesNotThrowError()
        {
            string input = "<html><head><style type=\"text/css\">li:crazy { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";
            PreMailer.MoveCssInline(input);
        }

        [Fact]
        public void MoveCssInline_SupportedjQuerySelector_AppliesCss()
        {
            string input = "<html><head><style type=\"text/css\">li:first-child { width: 42px; }</style></head><body><ul><li>target</li><li>blargh></li></ul></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<li style=\"width: 42px\">target</li>"));
        }

        [Fact]
        public void MoveCssInline_UnsupportedSelector_AppliesCss()
        {
            string input = "<html><head><style type=\"text/css\">p:first-letter { width: 42px; }</style></head><body><p>target</p></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<p>target</p>"));
        }

        [Fact]
        public void MoveCssInline_KeepStyleElementsIgnoreElementsMatchesStyleElement_DoesntRemoveScriptTag()
        {
            string input = "<html><head><style id=\"ignore\" type=\"text/css\">li:before { width: 42px; }</style></head><body><div class=\"target\">test</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, removeStyleElements: false, ignoreElements: "#ignore");

            Assert.True(premailedOutput.Html.Contains("<style id=\"ignore\" type=\"text/css\">"));
        }

        [Fact]
        public void MoveCssInline_MultipleSelectors_HonorsIndividualSpecificity()
        {
            string input = "<html><head><style type=\"text/css\">p,li,tr.pub-heading td,tr.pub-footer td,tr.footer-heading td { font-size: 12px; line-height: 16px; } td.disclaimer p {font-size: 11px;} </style></head><body><table><tr class=\"pub-heading\"><td class=\"disclaimer\"><p></p></td></tr></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<p style=\"font-size: 11px;line-height: 16px\"></p>"));
        }

        [Fact]
        public void MoveCssInline_ImportantFlag_HonorsImportantFlagInStylesheet()
        {
            string input = "<style>div { color: blue !important; }</style><div style=\"color: red\">Red</div>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<div style=\"color: blue"));
        }

        [Fact]
        public void MoveCssInline_ImportantFlag_HonorsImportantFlagInline()
        {
            string input = "<style>div { color: blue !important; }</style><div style=\"color: red !important\">Red</div>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<div style=\"color: red"));
        }

        [Fact]
        public void MoveCssInline_AbsoluteBackgroundUrl_ShouldNotBeCleanedAsComment()
        {
            string input = "<style>div { background: url('http://my.web.site.com/Content/email/content.png') repeat-y }</style><div></div>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<div style=\"background: url('http://my.web.site.com/Content/email/content.png') repeat-y\"></div>"));
        }

        public void MoveCssInline_SupportedMediaAttribute_InlinesAsNormal()
        {
            string input = "<html><head><style type=\"text/css\" media=\"screen\">div { width: 100% }</style></head><body><div>Target</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<div style=\"width: 100%;\">Target</div>"));
        }

        [Fact]
        public void MoveCssInline_UnsupportedMediaAttribute_IgnoresStyles()
        {
            string input = "<html><head><style type=\"text/css\" media=\"print\">div { width: 100% }</style></head><body><div>Target</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input);

            Assert.True(premailedOutput.Html.Contains("<div>Target</div>"));
        }

        [Fact]
        public void MoveCssInline_AddBgColorStyle()
        {
            string input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; }</style></head><body><table><tr><td class=\"test\" bgcolor=\"\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<td class=\"test\" bgcolor=\"#f1f1f1\" style=\"background-color: #f1f1f1\">"));
        }

        [Fact]
        public void MoveCssInline_AddSpecial()
        {
            string input = "<html><head><style type=\"text/css\">.test { padding: 7px; -premailer-cellspacing: 5; -premailer-width: 14%; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<td class=\"test\" cellspacing=\"5\" width=\"14%\" style=\"padding: 7px\">"), "Actual: " + premailedOutput.Html);
        }

        [Fact]
        public void MoveCssInline_AddSpecial_RemoveEmptyStyle()
        {
            string input = "<html><head><style type=\"text/css\">.test { -premailer-cellspacing: 5; -premailer-width: 14%; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<td class=\"test\" cellspacing=\"5\" width=\"14%\">"), "Actual: " + premailedOutput.Html);
        }

        [Fact]
        public void MoveCssInline_AddBgColorStyle_IgnoreElementWithBackgroundColorAndNoBgColor()
        {
            string input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; }</style></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<td class=\"test\" style=\"background-color: #f1f1f1\""));
        }

        [Fact]
        public void MoveCssInline_NoStyleElement_StylesGivenInCSSParam_InlinesThat()
        {
            string input = "<html><head></head><body><table><tr><td class=\"test\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false, css: ".test { background-color:#f1f1f1; }");

            Assert.True(premailedOutput.Html.Contains("<td class=\"test\" style=\"background-color: #f1f1f1\""));
        }

        [Fact]
        public void MoveCssInline_StripsClassAttributes()
        {
            string input = "<html><head></head><body><table id=\"testTable\"><tr><td class=\"test\"></td></tr></table></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false, css: ".test { background-color:#f1f1f1; }", stripIdAndClassAttributes: true);

            Assert.True(premailedOutput.Html.Contains("<td style=\"background-color: #f1f1f1\""));
        }

        [Fact]
        public void MoveCssInline_StripsIdAttributes()
        {
            string input = "<html><head><style type=\"text/css\">#high-imp.test { width: 42px; } .test { width: 150px; }</style></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false, stripIdAndClassAttributes: true);

            Assert.True(premailedOutput.Html.Contains("<div style=\"width: 42px\">"));
        }

        [Fact]
        public void MoveCssInline_StripsComments()
        {
            string input = "<html><head></head><body><!--This should be removed--></body></html>";
            string expected = "<html><head></head><body></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, removeComments: true);

            Assert.True(expected == premailedOutput.Html);
        }

        [Fact]
        public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_InSameBlock()
        {
            string input1 = "<html><head><style type=\"text/css\">table.acolor td { color: #0F0; } table.bcolor td { color: #00F; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";
            string input2 = "<html><head><style type=\"text/css\">table.bcolor td { color: #00F; } table.acolor td { color: #0F0; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";

            var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
            var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

            Assert.True(premailedOutput1.Html.Contains("<td style=\"color: #00F\">test</td>"));
            Assert.True(premailedOutput2.Html.Contains("<td style=\"color: #0F0\">test</td>"));
        }

        [Fact]
        public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_Nested_InSameBlock()
        {
            string input1 = "<html><head><style type=\"text/css\">table.child td { color: #00F; } table.parent td { color: #0F0; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";
            string input2 = "<html><head><style type=\"text/css\">table.parent td { color: #0F0; } table.child td { color: #00F; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";

            var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
            var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

            Assert.True(premailedOutput1.Html.Contains("<td style=\"color: #0F0\">test</td>"));
            Assert.True(premailedOutput2.Html.Contains("<td style=\"color: #00F\">test</td>"));
        }

        [Fact]
        public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_InSeparateBlocks()
        {
            string input1 = "<html><head><style type=\"text/css\">table.acolor td { color: #00F; }</style><style type=\"text/css\">table.bcolor td { color: #0F0; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";
            string input2 = "<html><head><style type=\"text/css\">table.bcolor td { color: #0F0; }</style><style type=\"text/css\">table.acolor td { color: #00F; }</style></head><body><table class=\"acolor bcolor\"><tr><td>test</td></tr></table></body></html>";

            var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
            var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

            Assert.True(premailedOutput1.Html.Contains("<td style=\"color: #0F0\">test</td>"));
            Assert.True(premailedOutput2.Html.Contains("<td style=\"color: #00F\">test</td>"));
        }

        [Fact]
        public void MoveCssInline_LaterPositionStylesWithEqualSpecificityHasPrecedence_Nested_InSeparateBlocks()
        {
            string input1 = "<html><head><style type=\"text/css\">table.child td { color: #00F; } table.parent td { color: #00F; }</style><style type=\"text/css\">table.parent td { color: #0F0; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";
            string input2 = "<html><head><style type=\"text/css\">table.parent td { color: #0F0; } table.child td { color: #0F0; }</style><style type=\"text/css\">table.child td { color: #00F; }</style></head><body><table class=\"parent\"><tr><td><table class=\"child\"><tr><td>test</td></tr></table></td></tr></table></body></html>";

            var premailedOutput1 = PreMailer.MoveCssInline(input1, false);
            var premailedOutput2 = PreMailer.MoveCssInline(input2, false);

            Assert.True(premailedOutput1.Html.Contains("<td style=\"color: #0F0\">test</td>"));
            Assert.True(premailedOutput2.Html.Contains("<td style=\"color: #00F\">test</td>"));
        }

        [Fact]
        public void AddAnalyticsTags_AddsTags()
        {
            const string input = @"<div><a href=""http://blah.com/someurl"">Some URL</a><a>No href</a></div><div><a href=""http://blah.com/someurl?extra=1"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div>";
            const string expected = @"<html><head></head><body><div><a href=""http://blah.com/someurl?utm_source=source&amp;utm_medium=medium&amp;utm_campaign=campaign&amp;utm_content=content"">Some URL</a><a>No href</a></div><div><a href=""http://blah.com/someurl?extra=1&amp;utm_source=source&amp;utm_medium=medium&amp;utm_campaign=campaign&amp;utm_content=content"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div></body></html>";
            var premailedOutput = new PreMailer(input)
                .AddAnalyticsTags("source", "medium", "campaign", "content")
                .MoveCssInline();
            Assert.True(expected == premailedOutput.Html);
        }

        [Fact]
        public void AddAnalyticsTags_AddsTagsAndExcludesDomain()
        {
            const string input = @"<div><a href=""http://www.blah.com/someurl"">Some URL</a><a>No href</a></div><div><a href=""https://www.nomatch.com/someurl?extra=1"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div>";
            const string expected = @"<html><head></head><body><div><a href=""http://www.blah.com/someurl?utm_source=source&amp;utm_medium=medium&amp;utm_campaign=campaign&amp;utm_content=content"">Some URL</a><a>No href</a></div><div><a href=""https://www.nomatch.com/someurl?extra=1"">Extra Stuff</a><a href=""{{Handlebars}}"">Don't Touch</a></div></body></html>";
            var premailedOutput = new PreMailer(input)
                .AddAnalyticsTags("source", "medium", "campaign", "content", "www.Blah.com")
                .MoveCssInline();
            Assert.True(expected == premailedOutput.Html);
        }

        [Fact]
        public void ContainsLinkCssElement_DownloadsCss()
        {
            var mockDownloader = new Mock<IWebDownloader>();
            mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".a { display: block; }");
            WebDownloader.SharedDownloader = mockDownloader.Object;

            Uri baseUri = new Uri("http://a.com");
            Uri fullUrl = new Uri(baseUri, "b.css");
            string input = String.Format("<html><head><link href=\"{0}\"></link></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>", fullUrl);

            PreMailer sut = new PreMailer(input, baseUri);
            sut.MoveCssInline();

            mockDownloader.Verify(d => d.DownloadString(fullUrl));
        }

        [Fact]
        public void ContainsLinkCssElement_NotCssFile_DoNotDownload()
        {
            var mockDownloader = new Mock<IWebDownloader>();
            mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".a { display: block; }");
            WebDownloader.SharedDownloader = mockDownloader.Object;

            Uri baseUri = new Uri("http://a.com");
            Uri fullUrl = new Uri(baseUri, "b.bs");
            string input = String.Format("<html><head><link href=\"{0}\"></link></head><body><div id=\"high-imp\" class=\"test\">test</div></body></html>", fullUrl);

            PreMailer sut = new PreMailer(input, baseUri);
            sut.MoveCssInline();

            mockDownloader.Verify(d => d.DownloadString(It.IsAny<Uri>()), Times.Never());
        }

        [Fact]
        public void ContainsLinkCssElement_DownloadsCss_InlinesContent()
        {
            var mockDownloader = new Mock<IWebDownloader>();
            mockDownloader.Setup(d => d.DownloadString(It.IsAny<Uri>())).Returns(".test { width: 150px; }");
            WebDownloader.SharedDownloader = mockDownloader.Object;

            string input = "<html><head><link href=\"http://a.com/b.css\"></link></head><body><div class=\"test\">test</div></body></html>";

            PreMailer sut = new PreMailer(input, new Uri("http://a.com"));
            var premailedOutput = sut.MoveCssInline();

            Assert.True(premailedOutput.Html.Contains("<div class=\"test\" style=\"width: 150px\">"));
        }

        [Fact]
        public void ContainsKeyframeCSS_InlinesCSSWithOutError()
        {
            string keyframeAnimation = @"
				@keyframes mymove {
						0%   {top: 0px;}
						25%  {top: 200px;}
						75%  {top: 50px}
						100% {top: 100px;}
				}
			";

            string input = "<html><head><style type=\"text/css\">.test { background-color:#f1f1f1; } " + keyframeAnimation + "</style></head><body><div class=\"test\">test</div></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.Contains("<div class=\"test\" style=\"background-color: #f1f1f1\""));
        }

        [Fact]
        public void MoveCSSInline_PreservesDocType()
        {
            string input = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head></head><body></body></html>";

            var premailedOutput = PreMailer.MoveCssInline(input, false);

            Assert.True(premailedOutput.Html.StartsWith("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">"));
        }
    }
}
