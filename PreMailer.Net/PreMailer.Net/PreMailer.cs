using System;
using System.Collections.Generic;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace PreMailer.Net
{
    public class PreMailer
    {
        private readonly CssSelectorParser _cssSelectorParser;

        public PreMailer()
        {
            _cssSelectorParser = new CssSelectorParser();
        }

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

            if (styleNodes == null) return htmlInput; // no styles to move

            foreach (var style in styleNodes)
            {
                if (style.Attributes["id"] != null && !String.IsNullOrWhiteSpace(style.Attributes["id"].Value) &&
                    style.Attributes["id"].Value.Equals("mobile", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                CssParser cssParser = new CssParser();
                string cssBlock = style.InnerHtml;

                cssParser.AddStyleSheet(cssBlock);

                var elemStyles = new Dictionary<HtmlNode, List<StyleClass>>();

                // First up - remember each style definition that is to be applied to each element.
                foreach (var item in cssParser.Styles)
                {
                    var styleClass = item.Value;
                    var elements = doc.DocumentNode.QuerySelectorAll(styleClass.Name);

                    foreach (var element in elements)
                    {
                        var existingStyles = elemStyles.ContainsKey(element)
                            ? elemStyles[element]
                            : new List<StyleClass>();

                        existingStyles.Add(item.Value);

                        elemStyles[element] = existingStyles;
                    }
                }

                // Now we know all the styles that should be applied to each element.
                // Sort them by their specificity and then apply them in turn, merging and allowing overwrite
                foreach (var elemStyle in elemStyles)
                {
                    var sortedStyleDefs =
                        elemStyle.Value.OrderBy(x => _cssSelectorParser.GetSelectorSpecificity(x.Name)).ToList();
                    var styleAttribute = elemStyle.Key.Attributes["style"];

                    if (styleAttribute == null)
                    {
                        elemStyle.Key.Attributes.Add("style", String.Empty);
                        styleAttribute = elemStyle.Key.Attributes["style"];
                    }
                    else // Ensure that existing inline styles always win.
                    {
                        sortedStyleDefs.Add(
                            cssParser.ParseStyleClass("inline", elemStyle.Key.Attributes["style"].Value));
                    }

                    foreach (var styleDef in sortedStyleDefs)
                    {
                        var sc = cssParser.ParseStyleClass("dummy", styleAttribute.Value);
                        sc.Merge(styleDef, true);

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