using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PreMailer.Net
{
	public class CssSelectorParser : ICssSelectorParser
	{
		private readonly Regex _idMatcher;
		private readonly Regex _attribMatcher;
		private readonly Regex _classMatcher;
		private readonly Regex _pseudoClassMatcher;
		private readonly Regex _elemMatcher;
		private readonly Regex _pseudoElemMatcher;

		public CssSelectorParser()
		{
			_idMatcher = new Regex(@"#([\w]+)", RegexOptions.Compiled & RegexOptions.IgnoreCase);
			_attribMatcher = new Regex(@"\[[\w=]+\]", RegexOptions.Compiled & RegexOptions.IgnoreCase);
			_classMatcher = new Regex(@"\.([\w]+)", RegexOptions.Compiled & RegexOptions.IgnoreCase);
			_pseudoClassMatcher = BuildPseudoClassesRegex();
			_elemMatcher = new Regex(@"[a-zA-Z]+", RegexOptions.Compiled & RegexOptions.IgnoreCase);
			_pseudoElemMatcher = BuildPseudoElementsRegex();
		}

		/// <summary>
		/// Static method to quickly find the specificity of a single CSS selector.<para/>
		/// Don't use this when parsing a lot of selectors, create an instance of <see cref="CssSelectorParser"/> and use that instead.
		/// </summary>
		/// <param name="selector">CSS Selector</param>
		/// <returns>Specificity score of the given selector.</returns>
		public static int SelectorSpecificity(string selector)
		{
			var instance = new CssSelectorParser();
			return instance.GetSelectorSpecificity(selector);
		}

		/// <summary>
		/// Finds the specificity of a CSS selector.<para />
		/// Using this instance method is more performant for checking many selectors since the Regex's are compiled.
		/// </summary>
		/// <param name="selector">CSS Selector</param>
		/// <returns>Specificity score of the given selector.</returns>
		public int GetSelectorSpecificity(string selector)
		{
			return CalculateSpecificity(selector).ToInt();
		}

		public CssSpecificity CalculateSpecificity(string selector)
		{
			if (string.IsNullOrWhiteSpace(selector) || selector == "*")
				return CssSpecificity.None;

			var cssSelector = new CssSelector(selector);

			var result = CssSpecificity.None;
			if (cssSelector.HasNotPseudoClass)
			{
				result += CalculateSpecificity(cssSelector.NotPseudoClassContent);
			}

			var buffer = cssSelector.StripNotPseudoClassContent().ToString();

			var ids = MatchCountAndStrip(_idMatcher, buffer, out buffer);
			var attributes = MatchCountAndStrip(_attribMatcher, buffer, out buffer);
			var classes = MatchCountAndStrip(_classMatcher, buffer, out buffer);
			var pseudoClasses = MatchCountAndStrip(_pseudoClassMatcher, buffer, out buffer);
			var elementNames = MatchCountAndStrip(_elemMatcher, buffer, out buffer);
			var pseudoElements = MatchCountAndStrip(_pseudoElemMatcher, buffer, out buffer);

			var specificity = new CssSpecificity(ids, (classes + attributes + pseudoClasses), (elementNames + pseudoElements));
			return result + specificity;
		}

		public bool IsPseudoClass(string selector)
		{
			return _pseudoClassMatcher.IsMatch(selector);
		}

		public bool IsPseudoElement(string selector)
		{
			return _pseudoElemMatcher.IsMatch(selector);
		}

		private static int MatchCountAndStrip(Regex regex, string selector, out string result)
		{
			var matches = regex.Matches(selector);

			result = regex.Replace(selector, string.Empty);

			return matches.Count;
		}

		private static Regex BuildPseudoClassesRegex()
		{
			return BuildOrRegex(PseudoClasses, ":", x => x.Replace("()", @"\(\w+\)"));
		}

		private static string[] PseudoClasses
		{
			get
			{
				// Taken from https://developer.mozilla.org/en-US/docs/Web/CSS/Pseudo-classes
				return new[]
				{
					"active",
					"checked",
					"default",
					"dir()",
					"disabled",
					"empty",
					"enabled",
					"first",
					"first-child",
					"first-of-type",
					"fullscreen",
					"focus",
					"hover",
					"indeterminate",
					"in-range",
					"invalid",
					"lang()",
					"last-child",
					"last-of-type",
					"left",
					"link",
					"not()",
					"nth-child()",
					"nth-last-child()",
					"nth-last-of-type()",
					"nth-of-type()",
					"only-child",
					"only-of-type",
					"optional",
					"out-of-range",
					"read-only",
					"read-write",
					"required",
					"right",
					"root",
					"scope",
					"target",
					"valid",
					"visited"
				}
				.Reverse().ToArray(); // NOTE: Reversal is imporant to ensure 'first-line' is processed before 'first'.
			}
		}

		private static Regex BuildPseudoElementsRegex()
		{
			return BuildOrRegex(PseudoElements, "::?");
		}

		private static string[] PseudoElements
		{
			get
			{
				// Taken from: https://developer.mozilla.org/en-US/docs/Web/CSS/Pseudo-elements
				return new[]
				{
					"after",
					"before",
					"first-letter",
					"first-line",
					"selection"
				};
			}
		}

		private static Regex BuildOrRegex(string[] items, string prefix, Func<string, string> mutator = null)
		{
			var sb = new StringBuilder();
			sb.Append(prefix);
			sb.Append("(");
			for (var i = 0; i < items.Length; i++)
			{
				var @class = items[i];

				if (mutator != null)
				{
					@class = mutator(@class);
				}

				sb.Append(@class);

				if (i < (items.Length - 1))
					sb.Append("|");
			}

			sb.Append(")");
			return new Regex(sb.ToString(), RegexOptions.IgnoreCase);
		}
	}
}