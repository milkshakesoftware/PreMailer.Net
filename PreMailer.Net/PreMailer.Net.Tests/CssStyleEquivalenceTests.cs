using CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssStyleEquivalenceTests
    {
        [TestMethod]
        public void FindEquivalentStyles()
        {
            var tableDomObject = CQ.CreateFragment("<td id=\"tabletest\" class=\"test\" bgcolor=\"\"></td>");
            var nodewithoutselector = tableDomObject.FirstElement();

            var clazz = new StyleClass();
            clazz.Attributes["background-color"] = CssAttribute.FromRule("background-color: red");

            var result = CssStyleEquivalence.FindEquivalent(nodewithoutselector, clazz);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void FindEquivalentStylesNoMatchingStyles()
        {
            var tableDomObject = CQ.CreateFragment("<td id=\"tabletest\" class=\"test\" bgcolor=\"\"></td>");

            var clazz = new StyleClass();
            clazz.Attributes["border"] = CssAttribute.FromRule("border: 1px");

            var nodewithoutselector = tableDomObject.FirstElement();

            var result = CssStyleEquivalence.FindEquivalent(nodewithoutselector, clazz);

            Assert.AreEqual(0, result.Count);
        }
    }
}