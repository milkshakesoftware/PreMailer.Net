using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace PreMailer.Net
{
	public class PreMailer
	{
		public string MoveCssInline(string htmlInput, bool removeStyleElements)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(htmlInput);

			var styleNodes = doc.DocumentNode.SelectNodes("//style");

			foreach (var style in styleNodes)
			{
				CssParser cssParser = new CssParser();
				string cssBlock = style.InnerHtml;

				cssParser.AddStyleSheet(cssBlock);

				foreach (var item in cssParser.Styles)
				{
					var styleClass = item.Value;
					var elements = doc.DocumentNode.QuerySelectorAll(styleClass.Name);

					foreach (var element in elements)
					{
						HtmlAttribute styleAttribute = element.Attributes["style"];

						if (styleAttribute == null)
						{
							element.Attributes.Add("style", String.Empty);
							styleAttribute = element.Attributes["style"];
						}

						StyleClass sc = cssParser.ParseStyleClass("dummy", styleAttribute.Value);
						sc.Merge(styleClass);

						styleAttribute.Value = sc.ToString();
					}
				}

				if (removeStyleElements)
				{
					style.Remove();
				}
			}

			return doc.DocumentNode.OuterHtml;
		}
	}
}