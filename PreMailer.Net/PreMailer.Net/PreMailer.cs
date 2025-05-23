using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Xhtml;
using PreMailer.Net.Extensions;
using PreMailer.Net.Html;
using PreMailer.Net.Sources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PreMailer.Net
{
	public class PreMailer : IDisposable
	{
		private readonly IHtmlDocument _document;
		private bool _removeStyleElements;
		private bool _stripIdAndClassAttributes;
		private string _ignoreElements;
		private string _css;
		private readonly Uri _baseUri;
		private readonly CssParser _cssParser;
		private readonly CssSelectorParser _cssSelectorParser;
		private readonly List<string> _warnings;

		/// <inheritdoc />
		/// <summary>
		/// Constructor for the PreMailer class
		/// </summary>
		/// <param name="html">The HTML input.</param>
		/// <param name="baseUri">Url that all relative urls will be off of</param>
		public PreMailer(string html, Uri baseUri = null)
			: this(new HtmlParser().ParseDocument(html), baseUri)
		{
		}

		/// <summary>
		/// Constructor for the PreMailer class
		/// </summary>
		/// <param name="htmlDocument">The <seealso cref="IHtmlDocument">HtmlDocument</seealso> input.</param>
		/// <param name="baseUri">Url that all relative urls will be off of</param>
		public PreMailer(IHtmlDocument htmlDocument, Uri baseUri = null)
		{
			_baseUri = baseUri;
			_document = htmlDocument;
			_warnings = new List<string>();
			_cssParser = new CssParser();
			_cssSelectorParser = new CssSelectorParser();
		}

		/// <summary>
		/// Constructor for the PreMailer class
		/// </summary>
		/// <param name="stream">The HTML stream.</param>
		/// <param name="baseUri">Url that all relative urls will be off of</param>
		public PreMailer(Stream stream, Uri baseUri = null)
		{
			_baseUri = baseUri;
			_document = new HtmlParser().ParseDocument(stream);
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
	/// <param name="customFormatter">Custom formatter to use</param>
	/// <param name="preserveMediaQueries">If set to true and removeStyleElements is true, it will instead preserve unsupported media queries in the style node and remove the other css, instead of removing the whole style node</param>
	/// <param name="useEmailFormatter">If set to true, empty HTML tags will be preserved as full tags instead of being converted to self-closing tags, and HTML entities like &copy; will be preserved</param>
	/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
	public static InlineResult MoveCssInline(string html, bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false, IMarkupFormatter customFormatter = null, bool preserveMediaQueries = false, bool useEmailFormatter = false)
	{
		return new PreMailer(html).MoveCssInline(removeStyleElements, ignoreElements, css, stripIdAndClassAttributes, removeComments, customFormatter, preserveMediaQueries, useEmailFormatter);
	}

	/// <summary>
	/// In-lines the CSS within the HTML given.
	/// </summary>
	/// <param name="stream">The Stream input.</param>
	/// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
	/// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
	/// <param name="css">A string containing a style-sheet for inlining.</param>
	/// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
	/// <param name="removeComments">True to remove comments, false to leave them intact</param>
	/// <param name="customFormatter">Custom formatter to use</param>
	/// <param name="preserveMediaQueries">If set to true and removeStyleElements is true, it will instead preserve unsupported media queries in the style node and remove the other css, instead of removing the whole style node</param>
	/// <param name="useEmailFormatter">If set to true, empty HTML tags will be preserved as full tags instead of being converted to self-closing tags, and HTML entities like &copy; will be preserved</param>
	/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
	public static InlineResult MoveCssInline(Stream stream, bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false, IMarkupFormatter customFormatter = null, bool preserveMediaQueries = false, bool useEmailFormatter = false)
	{
		return new PreMailer(stream).MoveCssInline(removeStyleElements, ignoreElements, css, stripIdAndClassAttributes, removeComments, customFormatter, preserveMediaQueries, useEmailFormatter);
	}

	/// <summary>
	/// In-lines the CSS within the HTML given.
	/// </summary>
	/// /// <param name="baseUri">The base url that will be used to resolve any relative urls</param>
	/// <param name="baseUri">The Url that all relative urls will be off of.</param>
	/// <param name="html">The HTML input.</param>
	/// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
	/// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
	/// <param name="css">A string containing a style-sheet for inlining.</param>
	/// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
	/// <param name="removeComments">True to remove comments, false to leave them intact</param>
	/// <param name="customFormatter">Custom formatter to use</param>
	/// <param name="preserveMediaQueries">If set to true and removeStyleElements is true, it will instead preserve unsupported media queries in the style node and remove the other css, instead of removing the whole style node</param>
	/// <param name="useEmailFormatter">If set to true, empty HTML tags will be preserved as full tags instead of being converted to self-closing tags, and HTML entities like &copy; will be preserved</param>
	/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
	public static InlineResult MoveCssInline(Uri baseUri, string html, bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false, IMarkupFormatter customFormatter = null, bool preserveMediaQueries = false, bool useEmailFormatter = false)
	{
		return new PreMailer(html, baseUri).MoveCssInline(removeStyleElements, ignoreElements, css, stripIdAndClassAttributes, removeComments, customFormatter, preserveMediaQueries, useEmailFormatter);
	}

	/// <summary>
	/// In-lines the CSS within the HTML given.
	/// </summary>
	/// /// <param name="baseUri">The base url that will be used to resolve any relative urls</param>
	/// <param name="baseUri">The Url that all relative urls will be off of.</param>
	/// <param name="stream">The HTML input.</param>
	/// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
	/// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
	/// <param name="css">A string containing a style-sheet for inlining.</param>
	/// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
	/// <param name="removeComments">True to remove comments, false to leave them intact</param>
	/// <param name="customFormatter">Custom formatter to use</param>
	/// <param name="preserveMediaQueries">If set to true and removeStyleElements is true, it will instead preserve unsupported media queries in the style node and remove the other css, instead of removing the whole style node</param>
	/// <param name="useEmailFormatter">If set to true, empty HTML tags will be preserved as full tags instead of being converted to self-closing tags, and HTML entities like &copy; will be preserved</param>
	/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
	public static InlineResult MoveCssInline(Uri baseUri, Stream stream, bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false, IMarkupFormatter customFormatter = null, bool preserveMediaQueries = false, bool useEmailFormatter = false)
	{
		return new PreMailer(stream, baseUri).MoveCssInline(removeStyleElements, ignoreElements, css, stripIdAndClassAttributes, removeComments, customFormatter, preserveMediaQueries, useEmailFormatter);
	}

	/// <summary>
	/// In-lines the CSS for the current HTML
	/// </summary>
	/// <param name="removeStyleElements">If set to <c>true</c> the style elements are removed.</param>
	/// <param name="ignoreElements">CSS selector for STYLE elements to ignore (e.g. mobile-specific styles etc.)</param>
	/// <param name="css">A string containing a style-sheet for inlining.</param>
	/// <param name="stripIdAndClassAttributes">True to strip ID and class attributes</param>
	/// <param name="removeComments">True to remove comments, false to leave them intact</param>
	/// <param name="customFormatter">Custom formatter to use</param>
	/// <param name="preserveMediaQueries">If set to true and removeStyleElements is true, it will instead preserve unsupported media queries in the style node and remove the other css, instead of removing the whole style node</param>
	/// <param name="useEmailFormatter">If set to true, empty HTML tags will be preserved as full tags instead of being converted to self-closing tags, and HTML entities like &copy; will be preserved</param>
	/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
	public InlineResult MoveCssInline(bool removeStyleElements = false, string ignoreElements = null, string css = null, bool stripIdAndClassAttributes = false, bool removeComments = false, IMarkupFormatter customFormatter = null, bool preserveMediaQueries = false, bool useEmailFormatter = false)
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
				RemoveStyleElements(cssSourceNodes, preserveMediaQueries);
				RemoveStyleElements(cssLinkNodes);
			}

			var joinedBlocks = Join(cssBlocks);
			var validSelectors = CleanUnsupportedSelectors(joinedBlocks);
			var elementsWithStyles = FindElementsWithStyles(validSelectors);
			var mergedStyles = MergeStyleClasses(elementsWithStyles);

			StyleClassApplier.ApplyAllStyles(mergedStyles);

			if (_stripIdAndClassAttributes)
				StripElementAttributes("id", "class");

			if (removeComments)
			{
				RemoveHtmlComments();

				RemoveCssComments(cssSourceNodes);
			}

			IMarkupFormatter markupFormatter = customFormatter;
			
			if (markupFormatter == null)
			{
				if (useEmailFormatter)
				{
					markupFormatter = EmailHtmlMarkupFormatter.Instance;
				}
				else
				{
					markupFormatter = GetMarkupFormatterForDocType();
				}
			}

			using (var sw = new StringWriter())
			{
				_document.ToHtml(sw, markupFormatter);

				return new InlineResult(sw.GetStringBuilder(), _warnings);
			}
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
			var tracking = $"utm_source={source}&utm_medium={medium}&utm_campaign={campaign}&utm_content={content}";
			foreach (var tag in _document.QuerySelectorAll("a[href]"))
			{
				var href = tag.Attributes["href"];
				if (href.Value.StartsWith("http", StringComparison.OrdinalIgnoreCase) && (domain == null || DomainMatch(domain, href.Value)))
				{
					var hrefValue = href.Value;
					var anchor = "";
					if (href.Value.Contains("#"))
					{
						anchor = href.Value.Substring(href.Value.IndexOf("#", StringComparison.Ordinal));
						hrefValue = hrefValue.Replace(anchor, "");
					}
					tag.SetAttribute("href", hrefValue + (hrefValue.IndexOf("?", StringComparison.Ordinal) >= 0 ? "&" : "?") + tracking + anchor);
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
			return cssSources.SelectMany(styleSource => styleSource.GetCss()).ToList();
		}

		private void RemoveCssComments(IEnumerable<IElement> cssSourceNodes)
		{
			foreach (var element in cssSourceNodes)
			{
				var regex = new Regex(@"/\*[\s\S]*?\*/");
				element.InnerHtml = regex.Replace(element.InnerHtml, string.Empty);
			}
		}

		/// <summary>
		/// Returns a list of CSS sources ('style', 'link' tags etc.) based on the elements given.<para/>
		/// These will be returned in their order of definition.
		/// </summary>
		private IEnumerable<ICssSource> ConvertToStyleSources(IEnumerable<IElement> nodesWithStyles)
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
		/// Returns a collection of CQ 'style' nodes that can be used to source CSS content.<para/>
		/// </summary>
		private IEnumerable<IElement> CssSourceNodes()
		{
			IEnumerable<IElement> elements = _document.QuerySelectorAll("style");

			if (!String.IsNullOrEmpty(_ignoreElements))
			{
				elements = elements.Not(_ignoreElements);
			}

			elements = elements.Where(elem =>
			{
				var mediaAttribute = elem.GetAttribute("media");

				return string.IsNullOrWhiteSpace(mediaAttribute) || CssParser._supportedMediaQueriesRegex.IsMatch(mediaAttribute);
			});

			return elements;
		}

		/// <summary>
		/// Returns a collection of CQ 'link' nodes that can be used to source CSS content.<para/>
		/// </summary>
		private IEnumerable<IElement> CssLinkNodes()
		{
			IEnumerable<IElement> elements = _document.QuerySelectorAll("link");

			if (!String.IsNullOrEmpty(_ignoreElements))
			{
				elements = elements.Not(_ignoreElements);
			}

			return elements.Where(e => e.Attributes
				.Any(a => a.Name.Equals("href", StringComparison.OrdinalIgnoreCase) &&
						 (a.Value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
						 (e.Attributes.Any(r => r.Name.Equals("rel", StringComparison.OrdinalIgnoreCase) &&
												r.Value.Equals("stylesheet", StringComparison.OrdinalIgnoreCase))))));
		}


		private void RemoveStyleElements(IEnumerable<IElement> cssSourceNodes, bool preserveMediaQueries = false)
		{
			foreach (var node in cssSourceNodes)
			{
				if (preserveMediaQueries)
				{
					var css = node.GetFirstTextNodeData();
					var unsupportedMediaQueries = CssParser.GetUnsupportedMediaQueries(css);
					if (unsupportedMediaQueries.Any())
					{
						node.InnerHtml = $"{string.Join("\n", unsupportedMediaQueries)}";
					}
					else
					{
						node.Remove();
					}
				}
				else
				{
					node.Remove();
				}
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
				_warnings.Add($"PreMailer.Net is unable to process the pseudo class/element '{failedSelector.Name}' due to a limitation in CsQuery.");
			}

			return result;
		}

		private Dictionary<IElement, List<StyleClass>> FindElementsWithStyles(
				SortedList<string, StyleClass> stylesToApply)
		{
			var result = new Dictionary<IElement, List<StyleClass>>();

			var selectorParser = _document.Context.GetService<AngleSharp.Css.Parser.ICssSelectorParser>();

			// Parse selectors
			var styles = stylesToApply.Select(x => new
			{
				Style = x.Value,
				Selector = selectorParser.ParseSelector(x.Value.Name)
			}).Where(x => x.Selector != null).ToList();

			foreach (var el in _document.DescendantsAndSelf<IElement>())
			{
				foreach (var style in styles)
				{
					if (style.Selector.Match(el))
					{
						var existing = result.ContainsKey(el) ? result[el] : new List<StyleClass>();
						existing.Add(style.Style);
						result[el] = existing;
					}
				}
			}

			return result;
		}

		private Dictionary<IElement, List<StyleClass>> SortBySpecificity(
				Dictionary<IElement, List<StyleClass>> styles)
		{
			var result = new Dictionary<IElement, List<StyleClass>>();
			var specificityCache = new Dictionary<string, int>();

			foreach (var style in styles)
			{
				if (style.Key.Attributes != null)
				{
					var sortedStyles = style.Value.OrderBy(x => GetSelectorSpecificity(specificityCache, x.Name)).ThenBy(x => x.Position).ToList();
					var styleAttr = style.Key.Attributes["style"];

					if (styleAttr == null || String.IsNullOrWhiteSpace(styleAttr.Value))
					{
						style.Key.SetAttribute("style", String.Empty);
					}
					else // Ensure that existing inline styles always win.
					{
						sortedStyles.Add(_cssParser.ParseStyleClass("inline", styleAttr.Value));
					}

					result[style.Key] = sortedStyles;
				}
			}

			return result;
		}

		private Dictionary<IElement, StyleClass> MergeStyleClasses(
				Dictionary<IElement, List<StyleClass>> styles)
		{
			var result = new Dictionary<IElement, StyleClass>();
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
				selectors.Add($"*[{attribute}]");
			}

			var elementsWithAttributes = _document.QuerySelectorAll(String.Join(",", selectors.Cast<string>().ToList()));
			foreach (var item in elementsWithAttributes)
			{
				foreach (var attribute in attributeNames)
				{
					item.RemoveAttribute(attribute);
				}
			}
		}

	private IMarkupFormatter GetMarkupFormatterForDocType()
	{
		if (_document != null && _document.Doctype != null && _document.Doctype.PublicIdentifier != null && _document.Doctype.PublicIdentifier.Contains("XHTML"))
		{
			return XhtmlMarkupFormatter.Instance;
		}

		return HtmlMarkupFormatter.Instance;
	}

		private int GetSelectorSpecificity(Dictionary<string, int> cache, string selector)
		{
			selector = selector ?? "";
			int specificity;

			if (!cache.TryGetValue(selector, out specificity))
			{
				specificity = _cssSelectorParser.GetSelectorSpecificity(selector);
				cache[selector] = specificity;
			}

			return specificity;
		}

		private void RemoveHtmlComments()
		{
			var comments = _document.Descendants<IComment>().ToList();

			foreach (var comment in comments)
			{
				comment.Remove();
			}
		}

		/// <summary>
		/// Access underlying IHTMLDocument
		/// </summary>
		public IHtmlDocument Document => _document;

		/// <summary>
		/// Dispose underlying document
		/// </summary>
		public void Dispose()
		{
			_document.Dispose();
		}
	}
}
