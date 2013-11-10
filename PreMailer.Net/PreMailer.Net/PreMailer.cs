using System;
using CsQuery;
using System.Collections.Generic;
using System.Linq;

namespace PreMailer.Net
{
    public class PreMailer
    {
        private readonly CssParser _cssParser;
        private readonly CssSelectorParser _cssSelectorParser;

        public PreMailer()
        {
            _cssParser = new CssParser();
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

            styleNodes
                .Join(removeElement: removeStyleElements)
                .FindElementsWithStyles(doc)
                .MergeStyleClasses(_cssParser, _cssSelectorParser)
                .ApplyStyles();

            return doc.Render();
        }
    }

    internal static class PreMailerExtensions
    {
        internal static SortedList<string, StyleClass> Join(this IEnumerable<IDomObject> styleNodes, bool removeElement)
        {
            var parser = new CssParser();

            foreach (var style in styleNodes)
            {
                if (style.IsForMobile())
                    continue;

                var cssBlock = style.InnerHTML;

                parser.AddStyleSheet(cssBlock);

                if (removeElement)
                    style.Remove();
            }
            
            return parser.Styles;
        }

        internal static bool IsForMobile(this IDomObject styleNode)
        {
            return (styleNode.Attributes["id"] != null && !String.IsNullOrWhiteSpace(styleNode.Attributes["id"]) &&
                    styleNode.Attributes["id"].Equals("mobile", StringComparison.InvariantCultureIgnoreCase));
        }

        internal static Dictionary<IDomObject, List<StyleClass>> FindElementsWithStyles(this SortedList<string, StyleClass> stylesToApply, CQ document)
        {
            var result = new Dictionary<IDomObject, List<StyleClass>>();

            foreach (var style in stylesToApply)
            {
                var elementsForSelector = document[style.Value.Name];

                foreach (var el in elementsForSelector)
                {
                    var existing = result.ContainsKey(el) ? result[el] : new List<StyleClass>();
                    existing.Add(style.Value);
                    result[el] = existing;
                }
            }
            
            return result;
        }

        internal static Dictionary<IDomObject, List<StyleClass>> SortBySpecificity(this Dictionary<IDomObject, List<StyleClass>> styles, CssParser cssParser, ICssSelectorParser selectorParser)
        {
            var result = new Dictionary<IDomObject, List<StyleClass>>();

            foreach (var style in styles)
            {
                var sortedStyles = style.Value.OrderBy(x => selectorParser.GetSelectorSpecificity(x.Name)).ToList();

                if (String.IsNullOrWhiteSpace(style.Key.Attributes["style"]))
                {
                    style.Key.SetAttribute("style", String.Empty);
                }
                else // Ensure that existing inline styles always win.
                {
										sortedStyles.Add(cssParser.ParseStyleClass("inline", style.Key.Attributes["style"]));
                }

                result[style.Key] = sortedStyles;
            }

            return result;
        }
        
        internal static Dictionary<IDomObject, StyleClass> MergeStyleClasses(this Dictionary<IDomObject, List<StyleClass>> styles, CssParser cssParser, ICssSelectorParser selectorParser)
        {
            var result = new Dictionary<IDomObject, StyleClass>();
            var stylesBySpecificity = styles.SortBySpecificity(cssParser, selectorParser);

            foreach (var elemStyle in stylesBySpecificity)
            {
                // CSS Classes are assumed to be sorted by specifity now, so we can just merge these up.
                var merged = new StyleClass();
                foreach (var style in elemStyle.Value)
                {
                    merged.Merge(style, true);
                }

                result[elemStyle.Key] = merged;
            }

            return result;
        }
        
        internal static void ApplyStyles(this Dictionary<IDomObject, StyleClass> elementStyles)
        {
            foreach (var elemStyle in elementStyles)
            {
                var el = elemStyle.Key;
                el.SetAttribute("style", elemStyle.Value.ToString());
            }
        }
    }
}