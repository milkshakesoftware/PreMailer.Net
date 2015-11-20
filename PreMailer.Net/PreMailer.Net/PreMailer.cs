using CsQuery;
using PreMailer.Net.Sources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PreMailer.Net
{
    public class PreMailer
    {
        private readonly CQ _document;
        private bool _removeStyleElements;
        private bool _stripIdAndClassAttributes;
        private string _ignoreElements;
        private string _css;
        private readonly Uri _baseUri;
        private readonly CssParser _cssParser;
        private readonly CssSelectorParser _cssSelectorParser;
        private readonly List<string> _warnings;

        /// <summary>
        /// Constructor for the PreMailer class
        /// </summary>
        /// <param name="html">The HTML input.</param>
        /// <param name="baseUri">Url that all relative urls will be off of</param>
        public PreMailer(string html, Uri baseUri = null)
        {
            _baseUri = baseUri;
            _document = CQ.CreateDocument(html);
            _warnings = new List<string>();
            _cssParser = new CssParser();
            _cssSelectorParser = new CssSelectorParser();
        }

        /// <summary>
        /// In-lines the CSS within the HTML given.
        /// </summary>
        /// <param name="html">The HTML input.</param>
        /// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
        /// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
        /// <param name="css">A string containing a style-sheet for inlining.</param>
        /// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
        /// <param name="removeComments">True to remove comments, false to leave them intact</param>
        /// <returns>Returns the html input, with styles moved to inline attributes.</returns>
        public static InlineResult MoveCssInline(string html, bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false)
        {
            return new PreMailer(html).MoveCssInline(removeStyleElements, ignoreElements, css, stripIdAndClassAttributes, removeComments);
        }

        /// <summary>
        /// In-lines the CSS within the HTML given.
        /// </summary>
        /// /// <param name="baseUri">The base url that will be used to resolve any relative urls</param>
        /// <param name="html">The HTML input.</param>
        /// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
        /// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
        /// <param name="css">A string containing a style-sheet for inlining.</param>
        /// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
        /// <param name="removeComments">True to remove comments, false to leave them intact</param>
        /// <returns>Returns the html input, with styles moved to inline attributes.</returns>
        public static InlineResult MoveCssInline(Uri baseUri, string html, bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false)
        {
            return new PreMailer(html, baseUri).MoveCssInline(removeStyleElements, ignoreElements, css, stripIdAndClassAttributes, removeComments);
        }

        /// <summary>
        /// In-lines the CSS for the current HTML
        /// </summary>
        /// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
        /// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
        /// <param name="css">A string containing a style-sheet for inlining.</param>
        /// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
        /// <param name="removeComments">True to remove comments, false to leave them intact</param>
        /// <returns>Returns the html input, with styles moved to inline attributes.</returns>
        public InlineResult MoveCssInline(bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false)
        {
            // Store the variables used for inlining the CSS
            _removeStyleElements = removeStyleElements;
            _stripIdAndClassAttributes = stripIdAndClassAttributes;
            _ignoreElements = ignoreElements;
            _css = css;

            // Gather all of the CSS that we can work with.
            var cssSourceNodes = CssSourceNodes();
            var cssLinkNodes = CssLinkNodes();
            var cssSources = new List<ICssSource>(ConvertToStyleSources(cssSourceNodes));
            cssSources.AddRange(ConvertToStyleSources(cssLinkNodes));

            var cssBlocks = GetCssBlocks(cssSources);

            if (_removeStyleElements)
            {
                RemoveStyleElements(cssSourceNodes);
                RemoveStyleElements(cssLinkNodes);
            }

            var joinedBlocks = Join(cssBlocks);
            var validSelectors = CleanUnsupportedSelectors(joinedBlocks);
            var elementsWithStyles = FindElementsWithStyles(validSelectors);
            var mergedStyles = MergeStyleClasses(elementsWithStyles);

            StyleClassApplier.ApplyAllStyles(mergedStyles);

            if (_stripIdAndClassAttributes)
                StripElementAttributes("id", "class");

            var html = _document.Render(removeComments ? DomRenderingOptions.RemoveComments : DomRenderingOptions.Default);
            return new InlineResult(html, _warnings);
        }

        /// <summary>
        /// Function to add Google analytics tracking tags to the HTML document
        /// </summary>
        /// <param name="source">Source tracking tag</param>
        /// <param name="medium">Medium tracking tag</param>
        /// <param name="campaign">Campaign tracking tag</param>
        /// <param name="content">Content tracking tag</param>
        /// <param name="domain">Optional domain check; if it does not match the URL will be skipped</param>
        /// <returns>Reference to the instance so you can chain calls.</returns>
        public PreMailer AddAnalyticsTags(string source, string medium, string campaign, string content, string domain = null)
        {
            var tracking = "utm_source=" + source + "&utm_medium=" + medium + "&utm_campaign=" + campaign + "&utm_content=" + content;
            foreach (var tag in _document["a[href]"])
            {
                var href = tag.Attributes["href"];
                if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase) && (domain == null || DomainMatch(domain, href)))
                {
                    tag.SetAttribute("href", href + (href.IndexOf("?", StringComparison.Ordinal) >= 0 ? "&" : "?") + tracking);
                }
            }
            return this;
        }

        /// <summary>
        /// Function to check if the domain in a URL matches
        /// </summary>
        /// <param name="domain">Domain to check</param>
        /// <param name="url">URL to parse</param>
        /// <returns>True if the domain matches, false if not</returns>
        private bool DomainMatch(string domain, string url)
        {
            if (url.Contains(@"://"))
            {
                url = url.Split(new[] { @"://" }, 2, StringSplitOptions.None)[1];
            }
            url = url.Split('/')[0];
            return string.Compare(domain, url, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns the blocks of CSS within the documents supported CSS sources.<para/>
        /// Blocks are returned in the order they are declared within the document.
        /// </summary>
        private IEnumerable<string> GetCssBlocks(IEnumerable<ICssSource> cssSources)
        {
            var styleBlocks = new List<string>();

            foreach (var styleSource in cssSources)
            {
                styleBlocks.Add(styleSource.GetCss());
            }

            return styleBlocks;
        }

        /// <summary>
        /// Returns a list of CSS sources ('style', 'link' tags etc.) based on the elements given.<para/>
        /// These will be returned in their order of definition.
        /// </summary>
        private IEnumerable<ICssSource> ConvertToStyleSources(CQ nodesWithStyles)
        {
            var result = new List<ICssSource>();
            var nodes = nodesWithStyles;
            foreach (var node in nodes)
            {
                switch (node.NodeName)
                {
                    case "STYLE":
                        result.Add(new DocumentStyleTagCssSource(node));
                        break;

                    case "LINK":
                        result.Add(new LinkTagCssSource(node, _baseUri));
                        break;
                }
            }

            if (!String.IsNullOrWhiteSpace(_css))
            {
                result.Add(new StringCssSource(_css));
            }

            return result;
        }

        /// <summary>
        /// Returns a collection of CQ 'sytle' nodes that can be used to source CSS content.<para/>
        /// </summary>
        private CQ CssSourceNodes()
        {
            var elements = _document.Find("style").Not(_ignoreElements).Filter(elem =>
            {
                var mediaAttribute = elem.GetAttribute("media");

                return string.IsNullOrWhiteSpace(mediaAttribute) || CssParser.SupportedMediaQueriesRegex.IsMatch(mediaAttribute);
            });
            return elements;
        }

        /// <summary>
        /// Returns a collection of CQ 'link' nodes that can be used to source CSS content.<para/>
        /// </summary>
        private CQ CssLinkNodes()
        {
            return _document.Find("link").Not(_ignoreElements)
                .Filter(e => e.Attributes
                    .Any(a => a.Key.Equals("href", StringComparison.OrdinalIgnoreCase) &&
                    a.Value.EndsWith(".css", StringComparison.OrdinalIgnoreCase)));
        }


        private void RemoveStyleElements(CQ cssSourceNodes)
        {
            foreach (var node in cssSourceNodes)
            {
                node.Remove();
            }
        }

        private static SortedList<string, StyleClass> Join(IEnumerable<string> cssBlocks)
        {
            var parser = new CssParser();

            foreach (var block in cssBlocks)
            {
                parser.AddStyleSheet(block);
            }

            return parser.Styles;
        }

        private SortedList<string, StyleClass> CleanUnsupportedSelectors(SortedList<string, StyleClass> selectors)
        {
            var result = new SortedList<string, StyleClass>();
            var failedSelectors = new List<StyleClass>();

            foreach (var selector in selectors)
            {
                if (_cssSelectorParser.IsSupportedSelector(selector.Key))
                    result.Add(selector.Key, selector.Value);
                else
                    failedSelectors.Add(selector.Value);
            }

            if (!failedSelectors.Any())
                return selectors;

            foreach (var failedSelector in failedSelectors)
            {
                _warnings.Add(String.Format(
                        "PreMailer.Net is unable to process the pseudo class/element '{0}' due to a limitation in CsQuery.",
                        failedSelector.Name));
            }

            return result;
        }

        private Dictionary<IDomObject, List<StyleClass>> FindElementsWithStyles(
                SortedList<string, StyleClass> stylesToApply)
        {
            var result = new Dictionary<IDomObject, List<StyleClass>>();

            foreach (var style in stylesToApply)
            {
                var elementsForSelector = _document[style.Value.Name];

                foreach (var el in elementsForSelector)
                {
                    var existing = result.ContainsKey(el) ? result[el] : new List<StyleClass>();
                    existing.Add(style.Value);
                    result[el] = existing;
                }
            }

            return result;
        }

        private Dictionary<IDomObject, List<StyleClass>> SortBySpecificity(
                Dictionary<IDomObject, List<StyleClass>> styles)
        {
            var result = new Dictionary<IDomObject, List<StyleClass>>();

            foreach (var style in styles)
            {
                if (style.Key.Attributes != null)
                {
                    var sortedStyles = style.Value.OrderBy(x => _cssSelectorParser.GetSelectorSpecificity(x.Name)).ToList();

                    if (String.IsNullOrWhiteSpace(style.Key.Attributes["style"]))
                    {
                        style.Key.SetAttribute("style", String.Empty);
                    }
                    else // Ensure that existing inline styles always win.
                    {
                        sortedStyles.Add(_cssParser.ParseStyleClass("inline", style.Key.Attributes["style"]));
                    }

                    result[style.Key] = sortedStyles;
                }
            }

            return result;
        }

        private Dictionary<IDomObject, StyleClass> MergeStyleClasses(
                Dictionary<IDomObject, List<StyleClass>> styles)
        {
            var result = new Dictionary<IDomObject, StyleClass>();
            var stylesBySpecificity = SortBySpecificity(styles);

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

        private void StripElementAttributes(params string[] attributeNames)
        {
            StringCollection selectors = new StringCollection();

            foreach (string attribute in attributeNames)
            {
                selectors.Add(String.Format("*[{0}]", attribute));
            }

            CQ elementsWithAttributes = _document.Find(String.Join(",", selectors.Cast<string>().ToList()));
            foreach (var item in elementsWithAttributes)
            {
                foreach (string attribute in attributeNames)
                {
                    item.RemoveAttribute(attribute);
                }
            }
        }
    }
}