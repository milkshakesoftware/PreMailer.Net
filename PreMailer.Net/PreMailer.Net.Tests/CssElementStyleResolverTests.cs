using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssElementStyleResolverTests
    {
        [TestMethod]
        public void GetAllStylesForElement()
        {
            var tableDomObject = new HtmlParser().Parse("<table id=\"tabletest\" class=\"test\" bgcolor=\"\"></table>");
            var nodewithoutselector = (IElement)tableDomObject.Body.FirstChild;

            var clazz = new StyleClass();
            clazz.Attributes["background-color"] = CssAttribute.FromRule("background-color: red");

            var result = CssElementStyleResolver.GetAllStyles(nodewithoutselector, clazz);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("style", result.ElementAt(0).AttributeName);
            Assert.AreEqual("bgcolor", result.ElementAt(1).AttributeName);
        }
    }
}
