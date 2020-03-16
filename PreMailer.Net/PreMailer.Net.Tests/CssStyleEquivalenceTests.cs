using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssStyleEquivalenceTests
    {
        [TestMethod]
        public void FindEquivalentStyles()
        {
            var tableDomObject = new HtmlParser().ParseDocument("<table id=\"tabletest\" class=\"test\" bgcolor=\"\"></table>");
            var nodewithoutselector = (IElement)tableDomObject.Body.FirstChild;

            var clazz = new StyleClass
            {
	            Attributes =
	            {
		            ["background-color"] = CssAttribute.FromRule("background-color: red")
	            }
            };

            var result = CssStyleEquivalence.FindEquivalent(nodewithoutselector, clazz);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void FindEquivalentStylesNoMatchingStyles()
        {
            var tableDomObject = new HtmlParser().ParseDocument("<table id=\"tabletest\" class=\"test\" bgcolor=\"\"></table>");

            var clazz = new StyleClass
            {
	            Attributes =
	            {
		            ["border"] = CssAttribute.FromRule("border: 1px")
	            }
            };

            var nodewithoutselector = (IElement)tableDomObject.Body.FirstChild;

            var result = CssStyleEquivalence.FindEquivalent(nodewithoutselector, clazz);

            Assert.AreEqual(0, result.Count);
        }
    }
}