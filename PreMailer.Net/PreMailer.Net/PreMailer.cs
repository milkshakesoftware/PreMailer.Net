using System;
using System.Linq;
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
            var doc = new HtmlDocument();
			doc.LoadHtml(htmlInput);

			var styleNodes = doc.DocumentNode.SelectNodes("//style");

			if (styleNodes == null) return htmlInput; // no styles to move

			foreach (var style in styleNodes)
			{
                if (style.Attributes["id"] != null && !String.IsNullOrEmpty(style.Attributes["id"].Value) && style.Attributes["id"].Value.Equals("mobile", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

                var cssParser = new CssParser();
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

        public string MoveCssInline2(string htmlInput, bool removeStyleElements)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlInput);

            var styleNodes = doc.DocumentNode.SelectNodes("//style");

            if (styleNodes == null)
                return htmlInput; // no styles to move

            var cssParser = new CssParser();

            foreach (var style in styleNodes)
            {
                if (style.Attributes["id"] != null && !String.IsNullOrEmpty(style.Attributes["id"].Value) && style.Attributes["id"].Value.Equals("mobile", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                cssParser.AddStyleSheet(style.InnerHtml);

                if (removeStyleElements)
                {
                    style.Remove();
                }
            }

            SetStyles(doc.DocumentNode, null, cssParser);

            return doc.DocumentNode.OuterHtml;
        }

        private void SetStyles(HtmlNode parent, StyleClass style, CssParser cssParser)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (cssParser == null)
                throw new ArgumentNullException("cssParser");

            if (style == null)
                style = new StyleClass();

            foreach (var node in parent.ChildNodes)
            {
                if ((new[] {"#text", "#document"}).Contains(node.Name))
                    continue;

                var @class = node.Attributes["class"];

                if (@class != null)
                {
                    StyleClass style1 = cssParser.Styles["." + @class.Value];
                    style.Merge(style1, true);
                }

                if (style.Attributes.Any())
                {
                    HtmlAttribute styleAttribute = node.Attributes["style"];
                    if (styleAttribute == null)
                        node.Attributes.Add("style", String.Empty);

                    styleAttribute = node.Attributes["style"];

                    StyleClass sc = cssParser.ParseStyleClass("dummy", styleAttribute.Value);
                    style.Merge(sc, true);

                    node.Attributes["style"].Value = style.ToString();
                }

                SetStyles(node, style, cssParser);
            }
        }
	}
}