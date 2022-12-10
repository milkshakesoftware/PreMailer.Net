using System.Collections.Generic;
using AngleSharp.Dom;

namespace PreMailer.Net
{
	public static class StyleClassApplier
	{
		public static Dictionary<IElement, StyleClass> ApplyAllStyles(Dictionary<IElement, StyleClass> elementDictionary)
		{
			foreach (var styleClass in elementDictionary)
			{
				ApplyStyles(styleClass.Key, styleClass.Value);
			}

			return elementDictionary;
		}

		private static IElement ApplyStyles(IElement domElement, StyleClass clazz)
		{
			var styles = CssElementStyleResolver.GetAllStyles(domElement, clazz);

			foreach (var attributeToCss in styles)
			{
				SetAttribute(domElement, attributeToCss);
			}

			var styleAttr = domElement.Attributes["style"];
			if (styleAttr == null || string.IsNullOrEmpty(styleAttr.Value))
			{
				domElement.RemoveAttribute("style");
			}

			return domElement;
		}

		private static void SetAttribute(IElement domElement, AttributeToCss attributeToCss)
		{
			string name = attributeToCss.AttributeName;
			string value = attributeToCss.CssValue;

			//When rendering images, we need to prevent breaking the WIDTH and HEIGHT attributes. See PreMailerTests.MoveCssInline_HasStyle_DoesNotBreakImageWidthAttribute().
			//The old code could end up writing an image tag like <img width="206px"> which violates the HTML spec. It should render <img width="206">.
			if (domElement.NodeName == @"IMG"
				&& (name == "width" || name == "height")
				&& value.EndsWith("px"))
			{
				value = value.Replace("px", string.Empty);
			}

			// Quotation marks in CSS is common, but when applied to the style attribute we will use single quotes instead.
			if (value.Contains("\""))
			{
				value = value.Replace("\"", "'");
			}

			domElement.SetAttribute(name, value);
		}
	}
}