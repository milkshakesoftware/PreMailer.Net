using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using PreMailer.Net.Html;
using System;
using System.IO;
using Xunit;

namespace PreMailer.Net.Tests.Html
{
    public class EmailHtmlMarkupFormatterTests
    {
        [Fact]
        public void CloseTag_GivenEmptyTag_PreservesClosingTag()
        {
            string html = "<html><body><u></u></body></html>";
            var document = new HtmlParser().ParseDocument(html);
            var formatter = EmailHtmlMarkupFormatter.Instance;

            string result;
            using (var sw = new StringWriter())
            {
                document.ToHtml(sw, formatter);
                result = sw.ToString();
            }

            Assert.Contains("<u></u>", result);
            Assert.DoesNotContain("<u />", result);
            Assert.DoesNotContain("<u/>", result);
        }

        [Fact]
        public void Text_GivenHtmlWithCopyEntity_PreservesEntity()
        {
            string html = "<html><body><p>&copy; 2025</p></body></html>";
            var document = new HtmlParser().ParseDocument(html);
            var formatter = EmailHtmlMarkupFormatter.Instance;

            string result;
            using (var sw = new StringWriter())
            {
                document.ToHtml(sw, formatter);
                result = sw.ToString();
            }

            Assert.Contains("&copy;", result);
            Assert.DoesNotContain("©", result);
        }
    }
}
