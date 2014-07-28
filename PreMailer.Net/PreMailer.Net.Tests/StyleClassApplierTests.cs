using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class StyleClassApplierTests
    {
        [TestMethod]
        public void ApplyStylesToAllElements()
        {
            var elementDictionary = new Dictionary<IDomObject, StyleClass>();

            var tableDomObject1 = CQ.CreateFragment("<td id=\"tabletest1\" class=\"test1\" bgcolor=\"\"></td>");
            var tableDomObject2 = CQ.CreateFragment("<td id=\"tabletest2\" class=\"test2\" bgcolor=\"\" width=\"\"></td>");
            var tableDomObject3 = CQ.CreateFragment("<td id=\"tabletest3\" class=\"test3\" bgcolor=\"\" height=\"\"></td>");
            var tableDomObject4 = CQ.CreateFragment("<td id=\"tabletest4\" class=\"test4\" bgcolor=\"\" width=\"\"></td>");

            var styleClassBgColor = new StyleClass();
            styleClassBgColor.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008001");

            var styleClassWidth = new StyleClass();
            styleClassWidth.Attributes["width"] = CssAttribute.FromRule("width: 10px");

            var styleClassHeight = new StyleClass();
            styleClassHeight.Attributes["height"] = CssAttribute.FromRule("height: 10px");

            var styleClassBgAndWidth = new StyleClass();
            styleClassBgAndWidth.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008003");
            styleClassBgAndWidth.Attributes["width"] = CssAttribute.FromRule("width: 10px");
            
            elementDictionary.Add(tableDomObject1.FirstElement(), styleClassBgColor);
            elementDictionary.Add(tableDomObject2.FirstElement(), styleClassWidth);
            elementDictionary.Add(tableDomObject3.FirstElement(), styleClassHeight);
            elementDictionary.Add(tableDomObject4.FirstElement(), styleClassBgAndWidth);

            var result = StyleClassApplier.ApplyAllStyles(elementDictionary);

            Assert.AreEqual("<td class=\"test1\" style=\"background-color: #008001\" id=\"tabletest1\" bgcolor=\"#008001\"></td>", result.ElementAt(0).Key.ToString());
            Assert.AreEqual("<td class=\"test2\" style=\"width: 10px\" id=\"tabletest2\" bgcolor width=\"10px\"></td>", result.ElementAt(1).Key.ToString());
            Assert.AreEqual("<td class=\"test3\" style=\"height: 10px\" id=\"tabletest3\" bgcolor height=\"10px\"></td>", result.ElementAt(2).Key.ToString());
            Assert.AreEqual("<td class=\"test4\" style=\"background-color: #008003;width: 10px\" id=\"tabletest4\" bgcolor=\"#008003\" width=\"10px\"></td>", result.ElementAt(3).Key.ToString());
        }
    }
}
