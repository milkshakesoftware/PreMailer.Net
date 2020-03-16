﻿using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class StyleClassApplierTests
    {
        [TestMethod]
        public void ApplyStylesToAllElements()
        {
            var elementDictionary = new Dictionary<IElement, StyleClass>();

            var tableDomObject1 = new HtmlParser().ParseDocument("<table id=\"tabletest1\" class=\"test1\" bgcolor=\"\"></table>");
            var tableDomObject2 = new HtmlParser().ParseDocument("<table id=\"tabletest2\" class=\"test2\" bgcolor=\"\" width=\"\"></table>");
            var tableDomObject3 = new HtmlParser().ParseDocument("<table id=\"tabletest3\" class=\"test3\" bgcolor=\"\" height=\"\"></table>");
            var tableDomObject4 = new HtmlParser().ParseDocument("<table id=\"tabletest4\" class=\"test4\" bgcolor=\"\" width=\"\"></table>");

            var styleClassBgColor = new StyleClass
            {
	            Attributes =
	            {
		            ["background-color"] = CssAttribute.FromRule("background-color: #008001")
	            }
            };

            var styleClassWidth = new StyleClass
            {
	            Attributes =
	            {
		            ["width"] = CssAttribute.FromRule("width: 10px")
	            }
            };

            var styleClassHeight = new StyleClass
            {
	            Attributes =
	            {
		            ["height"] = CssAttribute.FromRule("height: 10px")
	            }
            };

            var styleClassBgAndWidth = new StyleClass
            {
	            Attributes =
	            {
		            ["background-color"] = CssAttribute.FromRule("background-color: #008003"),
		            ["width"] = CssAttribute.FromRule("width: 10px")
	            }
            };

            elementDictionary.Add(tableDomObject1.Body.FirstElementChild, styleClassBgColor);
            elementDictionary.Add(tableDomObject2.Body.FirstElementChild, styleClassWidth);
            elementDictionary.Add(tableDomObject3.Body.FirstElementChild, styleClassHeight);
            elementDictionary.Add(tableDomObject4.Body.FirstElementChild, styleClassBgAndWidth);

            var result = StyleClassApplier.ApplyAllStyles(elementDictionary);

            Assert.AreEqual("<table id=\"tabletest1\" class=\"test1\" bgcolor=\"#008001\" style=\"background-color: #008001\"></table>", result.ElementAt(0).Key.OuterHtml);
            Assert.AreEqual("<table id=\"tabletest2\" class=\"test2\" bgcolor=\"\" width=\"10px\" style=\"width: 10px\"></table>", result.ElementAt(1).Key.OuterHtml);
            Assert.AreEqual("<table id=\"tabletest3\" class=\"test3\" bgcolor=\"\" height=\"10px\" style=\"height: 10px\"></table>", result.ElementAt(2).Key.OuterHtml);
            Assert.AreEqual("<table id=\"tabletest4\" class=\"test4\" bgcolor=\"#008003\" width=\"10px\" style=\"background-color: #008003;width: 10px\"></table>", result.ElementAt(3).Key.OuterHtml);
        }
    }
}
