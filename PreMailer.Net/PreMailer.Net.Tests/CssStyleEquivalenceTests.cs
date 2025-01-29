using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace PreMailer.Net.Tests
{
    public class CssStyleEquivalenceTests
    {
        [Fact]
        public void FindEquivalentStyles()
        {
            var tableDomObject = new HtmlParser().ParseDocument("<table id=\"tabletest\" class=\"test\" bgcolor=\"\"></table>");
            var nodewithoutselector = (IElement)tableDomObject.Body.FirstChild;

            var clazz = new StyleClass();
            clazz.Attributes["background-color"] = CssAttribute.FromRule("background-color: red");

            var result = CssStyleEquivalence.FindEquivalent(nodewithoutselector, clazz);

            Assert.Single(result);
        }

        [Fact]
        public void FindEquivalentStylesNoMatchingStyles()
        {
            var tableDomObject = new HtmlParser().ParseDocument("<table id=\"tabletest\" class=\"test\" bgcolor=\"\"></table>");

            var clazz = new StyleClass();
            clazz.Attributes["border"] = CssAttribute.FromRule("border: 1px");

            var nodewithoutselector = (IElement)tableDomObject.Body.FirstChild;

            var result = CssStyleEquivalence.FindEquivalent(nodewithoutselector, clazz);

            Assert.Empty(result);
        }
    }
}