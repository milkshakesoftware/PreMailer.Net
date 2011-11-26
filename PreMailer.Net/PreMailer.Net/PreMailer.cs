using System;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Collections.Generic;

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
            return MoveCssInline(htmlInput, removeStyleElements, true);
        }

        /// <summary>
        /// Moves the CSS embedded in the specified htmlInput to inline style attributes.
        /// </summary>
        /// <param name="htmlInput">The HTML input.</param>
        /// <param name="removeStyleElements">if set to <c>true</c> the style elements are removed.</param>
        /// <param name="throwOnNotSupported">if set to <c>false</c> the not supported styles will be ignored.</param>
        /// <returns>Returns the html input, with styles moved to inline attributes.</returns>
        public string MoveCssInline(string htmlInput, bool removeStyleElements, bool throwOnNotSupported)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlInput);

            var styleNodes = doc.DocumentNode.SelectNodes("//style");
            if (styleNodes != null)
            {
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

                        IEnumerable<HtmlNode> elements = System.Linq.Enumerable.Empty<HtmlNode>();
                        try { elements = doc.DocumentNode.QuerySelectorAll(styleClass.Name); }
                        catch (FormatException ex) { if (throwOnNotSupported) throw ex; }

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
            }

            return doc.DocumentNode.OuterHtml;
        }
    }
}