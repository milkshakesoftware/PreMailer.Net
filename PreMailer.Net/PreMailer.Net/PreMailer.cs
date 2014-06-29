using CsQuery;
using PreMailer.Net.Sources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PreMailer.Net
{
	public class PreMailer
	{
		private readonly CQ _document;
		private readonly bool _removeStyleElements;
		private readonly string _ignoreElements;
		private readonly CssParser _cssParser;
		private readonly CssSelectorParser _cssSelectorParser;
		private readonly List<string> _warnings;

		private PreMailer(string html, bool removeStyleElements = false, string ignoreElements = null)
		{
			_document = CQ.CreateDocument(html);
			_removeStyleElements = removeStyleElements;
			_ignoreElements = ignoreElements;
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
		/// <returns>Returns the html input, with styles moved to inline attributes.</returns>
		public static InlineResult MoveCssInline(string html, bool removeStyleElements = false, string ignoreElements = null)
		{
			var pm = new PreMailer(html, removeStyleElements, ignoreElements);
			return pm.Process();
		}

		private InlineResult Process()
		{
			// Gather all of the CSS that we can work with.
			var cssSourceNodes = CssSourceNodes();
			var cssSources = ConvertToStyleSources(cssSourceNodes);
			var cssBlocks = GetCssBlocks(cssSources);


			if (_removeStyleElements)
				RemoveStyleElements(cssSourceNodes);

			var joinedBlocks = Join(cssBlocks);
			var validSelectors = CleanUnsupportedSelectors(joinedBlocks);
			var elementsWithStyles = FindElementsWithStyles(validSelectors);
			var mergedStyles = MergeStyleClasses(elementsWithStyles);
			ApplyStyles(mergedStyles);

			var html = _document.Render();
			return new InlineResult(html, _warnings);
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
				if (node.NodeName == "STYLE")
					result.Add(new DocumentStyleTagCssSource(node));
			}

			return result;
		}

		/// <summary>
		/// Returns a collection of CQ nodes that can be used to source CSS content.<para/>
		/// Currently, only 'style' tags are supported.
		/// </summary>
		private CQ CssSourceNodes()
		{
			// TODO: Add Source to Read CSS from LINK tags etc.
			// All we need to do here is update the selector in 'document.Find(...)' and then add
			// something that implements ICssSource to handle that type of link..
			// e.g. new LinkTagCssSource(node, baseUrl: "...");
            var elements = _document.Find("style").Not(_ignoreElements).Filter(elem =>
            {
                var mediaAttribute = elem.GetAttribute("media");

                return string.IsNullOrWhiteSpace(mediaAttribute) || CssParser.SupportedMediaQueriesRegex.IsMatch(mediaAttribute);
            });
			return elements;
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

	    private void ApplyStyles(Dictionary<IDomObject, StyleClass> elementStyles)
	    {
	        foreach (var elemStyle in elementStyles)
	        {
	            var el = elemStyle.Key;
                el.SetAttribute("style", elemStyle.Value.ToString());

	            var styleClass = elemStyle.Value;
                if (el.HasAttribute("bgcolor") && styleClass.Attributes.ContainsKey("background-color"))
	            {
	                var cssAttribute = styleClass.Attributes["background-color"];
                    el.SetAttribute("bgcolor",cssAttribute.Value);
	            }
	        }
	    }
	}
}