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
        
        [Fact]
        public void GetAllStyles_PreservesImportantFlagsInInlineStyles()
        {
            var document = new HtmlParser().ParseDocument("<div style=\"font-weight: bold !important;\"></div>");
            var element = document.Body.FirstElementChild;
            
            var styleClass = new StyleClass();
            styleClass.Attributes["color"] = CssAttribute.FromRule("color: red");
            
            var result = CssElementStyleResolver.GetAllStyles(element, styleClass);
            
            var styleAttribute = result.FirstOrDefault(a => a.AttributeName == "style");
            Assert.NotNull(styleAttribute);
            Assert.Contains("font-weight: bold !important", styleAttribute.CssValue);
            Assert.Contains("color: red", styleAttribute.CssValue);
        }
        
        [Fact]
        public void GetAllStyles_PreservesImportantFlagsWhenMergingStyles()
        {
            var document = new HtmlParser().ParseDocument("<div style=\"font-weight: bold !important;\"></div>");
            var element = document.Body.FirstElementChild;
            
            var styleClass = new StyleClass();
            styleClass.Attributes["font-weight"] = CssAttribute.FromRule("font-weight: normal");
            
            var result = CssElementStyleResolver.GetAllStyles(element, styleClass);
            
            var styleAttribute = result.FirstOrDefault(a => a.AttributeName == "style");
            Assert.NotNull(styleAttribute);
            Assert.Contains("font-weight: bold !important", styleAttribute.CssValue);
            Assert.DoesNotContain("font-weight: normal", styleAttribute.CssValue);
        }
    }
}
