using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace PreMailer.Net.Tests
{
    public class CssElementStyleResolverTests
    {
        [Fact]
        public void GetAllStylesForElement()
        {
            var tableDomObject = new HtmlParser().ParseDocument("<table id=\"tabletest\" class=\"test\" bgcolor=\"\"></table>");
            var nodewithoutselector = (IElement)tableDomObject.Body.FirstChild;

            var clazz = new StyleClass();
            clazz.Attributes["background-color"] = CssAttribute.FromRule("background-color: red");

            var result = CssElementStyleResolver.GetAllStyles(nodewithoutselector, clazz);

            Assert.Equal(2, result.Count());
            Assert.Equal("style", result.ElementAt(0).AttributeName);
            Assert.Equal("bgcolor", result.ElementAt(1).AttributeName);
        }
    }
}
