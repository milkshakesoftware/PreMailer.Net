using System;
using System.Linq;
using CsQuery;
using CsQuery.Implementation;

namespace PreMailer.Net {
    public class PreMailer {
        /// <summary>
        /// Moves the CSS embedded in the specified htmlInput to inline style attributes.
        /// </summary>
        /// <param name="htmlInput">The HTML input.</param>
        /// <param name="removeStyleElements">if set to <c>true</c> the style elements are removed.</param>
        /// <returns>Returns the html input, with styles moved to inline attributes.</returns>
        public string MoveCssInline(string htmlInput, bool removeStyleElements) {
            var doc = CQ.CreateDocument(htmlInput);
            var styleNodes = doc["style"];

            if (styleNodes == null || styleNodes.Length == 0) return htmlInput; // no styles to move

            foreach (var style in styleNodes) {
                if (style.Attributes["id"] != null && !String.IsNullOrWhiteSpace(style.Attributes["id"]) && style.Attributes["id"].Equals("mobile", StringComparison.InvariantCultureIgnoreCase)) {
                    continue;
                }

                var cssParser = new CssParser();
                string cssBlock = style.InnerHTML;

                cssParser.AddStyleSheet(cssBlock);

                foreach (var rule in cssParser.Styles) {
                    if (rule.Key.StartsWith("@media"))
                        continue;

                    var styleClass = rule.Value;
                    var elements = doc[styleClass.Name];

                    foreach (var element in elements) {
                        if (_elementsWithoutStyle.Contains(element.NodeName.ToLower()))
                            continue;

                        var elementStyle = element.Style;
                        if (elementStyle == null)
                            continue;
                        StyleClass sc = cssParser.ParseStyleClass("dummy", elementStyle.CssText ?? String.Empty);
                        sc.Merge(styleClass, true);
                        foreach (var attr in sc.Attributes)
                            elementStyle.SetStyle(attr.Key, attr.Value, false);
                    }
                }

                if (removeStyleElements) {
                    style.Remove();
                }
            }

            return doc.Render();
        }

        private static readonly string[] _elementsWithoutStyle = new string[] { "html", "head", "script", "noscript", "meta", "title", "style", "base" };
    }
}