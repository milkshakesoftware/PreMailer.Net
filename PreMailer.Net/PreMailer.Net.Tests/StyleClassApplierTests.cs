using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace PreMailer.Net.Tests
{
    public class StyleClassApplierTests
    {
        [Fact]
        public void ApplyStylesToAllElements()
        {
            var elementDictionary = new Dictionary<IElement, StyleClass>();

            var tableDomObject1 = new HtmlParser().ParseDocument("<table id=\"tabletest1\" class=\"test1\" bgcolor=\"\"></table>");
            var tableDomObject2 = new HtmlParser().ParseDocument("<table id=\"tabletest2\" class=\"test2\" bgcolor=\"\" width=\"\"></table>");
            var tableDomObject3 = new HtmlParser().ParseDocument("<table id=\"tabletest3\" class=\"test3\" bgcolor=\"\" height=\"\"></table>");
            var tableDomObject4 = new HtmlParser().ParseDocument("<table id=\"tabletest4\" class=\"test4\" bgcolor=\"\" width=\"\"></table>");

            var styleClassBgColor = new StyleClass();
            styleClassBgColor.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008001");

            var styleClassWidth = new StyleClass();
            styleClassWidth.Attributes["width"] = CssAttribute.FromRule("width: 10px");

            var styleClassHeight = new StyleClass();
            styleClassHeight.Attributes["height"] = CssAttribute.FromRule("height: 10px");

            var styleClassBgAndWidth = new StyleClass();
            styleClassBgAndWidth.Attributes["background-color"] = CssAttribute.FromRule("background-color: #008003");
            styleClassBgAndWidth.Attributes["width"] = CssAttribute.FromRule("width: 10px");
            
            elementDictionary.Add(tableDomObject1.Body.FirstElementChild, styleClassBgColor);
            elementDictionary.Add(tableDomObject2.Body.FirstElementChild, styleClassWidth);
            elementDictionary.Add(tableDomObject3.Body.FirstElementChild, styleClassHeight);
            elementDictionary.Add(tableDomObject4.Body.FirstElementChild, styleClassBgAndWidth);

            var result = StyleClassApplier.ApplyAllStyles(elementDictionary);

            Assert.Equal("<table id=\"tabletest1\" class=\"test1\" bgcolor=\"#008001\" style=\"background-color: #008001\"></table>", result.ElementAt(0).Key.OuterHtml);
            Assert.Equal("<table id=\"tabletest2\" class=\"test2\" bgcolor=\"\" width=\"10px\" style=\"width: 10px\"></table>", result.ElementAt(1).Key.OuterHtml);
            Assert.Equal("<table id=\"tabletest3\" class=\"test3\" bgcolor=\"\" height=\"10px\" style=\"height: 10px\"></table>", result.ElementAt(2).Key.OuterHtml);
            Assert.Equal("<table id=\"tabletest4\" class=\"test4\" bgcolor=\"#008003\" width=\"10px\" style=\"background-color: #008003;width: 10px\"></table>", result.ElementAt(3).Key.OuterHtml);
        }

		[Fact]
		public void ApplyInlineStylesWithoutImportant()
		{
			var document = new HtmlParser().ParseDocument("<div></div>");

			var clazz = new StyleClass();
			clazz.Attributes["color"] = CssAttribute.FromRule("color: #000 !important");

			var elementDictionary = new Dictionary<IElement, StyleClass> {
				{document.Body.FirstElementChild, clazz}
			};

			var result = StyleClassApplier.ApplyAllStyles(elementDictionary);

			Assert.Equal("<div style=\"color: #000\"></div>", result.ElementAt(0).Key.OuterHtml);
		}
    }
}
