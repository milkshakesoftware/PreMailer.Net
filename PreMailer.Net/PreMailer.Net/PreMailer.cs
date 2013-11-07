using System;
using CsQuery;
using System.Collections.Generic;
using System.Linq;

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
            var doc = CQ.CreateDocument(htmlInput);
            var styleNodes = doc["style"];

            if (styleNodes == null || styleNodes.Length == 0) return htmlInput; // no styles to move

            foreach (var style in styleNodes)
            {
                if (style.Attributes["id"] != null && !String.IsNullOrWhiteSpace(style.Attributes["id"]) &&
                    style.Attributes["id"].Equals("mobile", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                CssParser cssParser = new CssParser();
                string cssBlock = style.InnerHTML;

                cssParser.AddStyleSheet(cssBlock);

                var elemStyles = new Dictionary<IDomObject, List<StyleClass>>();

                // First up - remember each style definition that is to be applied to each element.
                foreach (var item in cssParser.Styles)
                {
                    var styleClass = item.Value;
                    var elements = doc[styleClass.Name];

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
                    var sortedStyleDefs = elemStyle.Value.OrderBy(x => _cssSelectorParser.GetSelectorSpecificity(x.Name)).ToList();

                    if (String.IsNullOrWhiteSpace(elemStyle.Key.Attributes["style"]))
                    {
                        elemStyle.Key.SetAttribute("style", String.Empty);
                    }
                    else // Ensure that existing inline styles always win.
                    {
												sortedStyleDefs.Add(cssParser.ParseStyleClass("inline", elemStyle.Key.Attributes["style"]));
                    }

                    foreach (var styleDef in sortedStyleDefs)
                    {
												var sc = cssParser.ParseStyleClass("dummy", elemStyle.Key.Attributes["style"]);
                        sc.Merge(styleDef, true);

												elemStyle.Key.SetAttribute("style", sc.ToString());
                    }
                }

                if (removeStyleElements)
                {
                    style.Remove();
                }
            }

            return doc.Render();
        }
    }
}