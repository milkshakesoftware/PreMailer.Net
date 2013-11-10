using System;
using CsQuery;
using System.Collections.Generic;
using System.Linq;
using PreMailer.Net.Sources;

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

			var styleBlocks = new List<string>();

			foreach (var styleSource in doc.StyleSources())
			{
				styleBlocks.Add(styleSource.GetCss());
			}

			if (removeStyleElements)
				doc.RemoveStyleElements();

			styleBlocks
				.Join(removeElement: removeStyleElements)
				.FindElementsWithStyles(doc)
				.MergeStyleClasses(_cssParser, _cssSelectorParser)
				.ApplyStyles();

			return doc.Render();
		}
	}

	internal static class PreMailerExtensions
	{
		/// <summary>
		/// Returns a List of CSS Sources based on their order in the document given.<para/>
		/// These will be returned in their order of definition.
		/// </summary>
		internal static List<ICssSource> StyleSources(this CQ document, bool ignoreMobile = true)
		{
			// TODO: Add Source to Read CSS from LINK tags etc.
			// All we need to do here is update the selector in 'document.Find(...)' and then add
			// something that implements ICssSource to handle that type of link..
			// e.g. new LinkTagCssSource(node, baseUrl: "...");
			var result = new List<ICssSource>();
			foreach (var node in document.Find("style"))
			{
				if (node.NodeName == "STYLE")
					result.Add(new DocumentStyleTagCssSource(node, ignoreMobile));
			}

			return result;
		}

		internal static void RemoveStyleElements(this CQ document)
		{
			foreach (var node in document["style"])
			{
				node.Remove();
			}
		}

		internal static SortedList<string, StyleClass> Join(this IEnumerable<string> cssBlocks, bool removeElement)
		{
			var parser = new CssParser();

			foreach (var block in cssBlocks)
			{
				parser.AddStyleSheet(block);
			}

			return parser.Styles;
		}

		internal static Dictionary<IDomObject, List<StyleClass>> FindElementsWithStyles(
			this SortedList<string, StyleClass> stylesToApply, CQ document)
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

		internal static Dictionary<IDomObject, List<StyleClass>> SortBySpecificity(
			this Dictionary<IDomObject, List<StyleClass>> styles, CssParser cssParser, ICssSelectorParser selectorParser)
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

		internal static Dictionary<IDomObject, StyleClass> MergeStyleClasses(
			this Dictionary<IDomObject, List<StyleClass>> styles, CssParser cssParser, ICssSelectorParser selectorParser)
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