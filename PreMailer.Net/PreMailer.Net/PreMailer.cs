using System;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace PreMailer.Net
{
	public class PreMailer
	{
		/// <summary>
		/// Moves the CSS embedded in the specified htmlInput to inline style attributes.
		/// </summary>
		/// <param name="htmlInput">The HTML input.</param>
		/// <param name="removeStyleElements">if set to <c>true</c> the style elements are removed.</param>
		/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
		public string MoveCssInline(string htmlInput, bool removeStyleElements)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(htmlInput);

			var styleNodes = doc.DocumentNode.SelectNodes("//style");

			foreach (var style in styleNodes)
			{
				if (style.Attributes["id"] != null && !String.IsNullOrWhiteSpace(style.Attributes["id"].Value) && style.Attributes["id"].Value.Equals("mobile", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

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
						sc.Merge(styleClass, false);

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