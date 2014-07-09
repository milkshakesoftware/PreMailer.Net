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
            var tableDomObject2 = CQ.CreateFragment("<td id=\"tabletest2\" class=\"test2\" bgcolor=\"\"></td>");
            var tableDomObject3 = CQ.CreateFragment("<td id=\"tabletest3\" class=\"test3\" bgcolor=\"\"></td>");

            var clazz1 = new StyleClass();
            clazz1.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008001");

            var clazz2 = new StyleClass();
            clazz2.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008002");

            var clazz3 = new StyleClass();
            clazz3.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008003");
            

            elementDictionary.Add(tableDomObject1.FirstElement(), clazz1);
            elementDictionary.Add(tableDomObject2.FirstElement(), clazz2);
            elementDictionary.Add(tableDomObject3.FirstElement(), clazz3);

            var result = StyleClassApplier.ApplyAllStyles(elementDictionary);

            Assert.AreEqual("<td class=\"test1\" style=\"background-color: #008001\" id=\"tabletest1\" bgcolor=\"#008001\"></td>", result.ElementAt(0).Key.ToString());
            Assert.AreEqual("<td class=\"test2\" style=\"background-color: #008002\" id=\"tabletest2\" bgcolor=\"#008002\"></td>", result.ElementAt(1).Key.ToString());
            Assert.AreEqual("<td class=\"test3\" style=\"background-color: #008003\" id=\"tabletest3\" bgcolor=\"#008003\"></td>", result.ElementAt(2).Key.ToString());
        }
    }
}
