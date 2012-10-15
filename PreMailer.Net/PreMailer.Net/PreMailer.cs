using System;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using PreMailerDotNet.Parsing;

namespace PreMailerDotNet
{
	public static class PreMailer
	{
		/// <summary>
		/// Moves the CSS embedded in the specified htmlInput to inline style attributes.
		/// </summary>
		/// <param name="htmlInput">The HTML input.</param>
		/// <param name="removeStyleElements">if set to <c>true</c> the style elements are removed.</param>
		/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
		public static string MoveCssInline(string htmlInput, bool removeStyleElements)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(htmlInput);

			/*var styleNodes = doc.DocumentNode.SelectNodes("//style");

			if (styleNodes == null) return htmlInput; // no styles to move

			foreach (var style in styleNodes)
			{
				if (IsMobileStyleSheet(style))
				{
					continue;
				}

				CssParser cssParser = new CssParser();
				string cssBlock = style.InnerHtml;

				cssParser.AddStyleSheet(cssBlock);

				foreach (var item in cssParser.Styles)
				{
					//RWM: Just because one style fails to merge doesn't mean they all should.
					try
					{
						var styleClass = item.Value;
						var elements = doc.DocumentNode.QuerySelectorAll(styleClass.Selector);

						foreach (var element in elements)
						{
							HtmlAttribute styleAttribute = element.Attributes["style"];

							if (styleAttribute == null)
							{
								element.Attributes.Add("style", String.Empty);
								styleAttribute = element.Attributes["style"];
							}

							Selector sc = new Selector();// cssParser.ParseStyleClass("dummy", styleAttribute.Value);
							sc.Merge(styleClass, false);

							styleAttribute.Value = sc.ToString();
						}
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.Fail(ex.Message);
					}
				}

				if (removeStyleElements)
				{
					style.Remove();
				}
			}*/

			return doc.DocumentNode.OuterHtml;
		}

		private static bool IsMobileStyleSheet(HtmlNode style)
		{
			return style.Attributes["id"] != null && !String.IsNullOrWhiteSpace(style.Attributes["id"].Value) && style.Attributes["id"].Value.Equals("mobile", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}