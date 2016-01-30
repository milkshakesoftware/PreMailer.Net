using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PreMailer.Net
{
    public class CssSelectorParser : ICssSelectorParser
    {
        #region "CSS definitions"
        // These definitions have been taken from https://www.w3.org/TR/css3-selectors/#lex
        private static readonly string Css_NonAscii = @"[^\0-\177]";
        private static readonly string Css_Unicode = @"(\\[0-9a-f]{1,6}(\r\n|[ \t\r\n\f])?)";
        private static readonly string Css_Escape = string.Format(@"({0}|\\[^\r\n\f0-9a-f])", Css_Unicode);
        private static readonly string Css_NmStart = string.Format(@"([_a-z]|{0}|{1})", Css_NonAscii, Css_Escape);
        private static readonly string Css_NmChar = string.Format(@"([_a-z0-9-]|{0}|{1})", Css_NonAscii, Css_Escape);

        private static readonly string Css_Ident = string.Format(@"(-?{0}{1}*)", Css_NmStart, Css_NmChar);

        private static readonly string Css_Nl = @"(\n|\r\n|\r|\f)";
        private static readonly string Css_String1 = string.Format(@"(""([^\n\r\f\\""]|\\{0}|{1}|{2})*"")", Css_Nl, Css_NonAscii, Css_Escape);
        private static readonly string Css_String2 = string.Format(@"('([^\n\r\f\\']|\\{0}|{1}|{2})*')", Css_Nl, Css_NonAscii, Css_Escape);
        private static readonly string Css_String = string.Format(@"({0}|{1})", Css_String1, Css_String2);
        #endregion

        // These definitions have been taken from https://www.w3.org/TR/css3-selectors/#grammar
        private static readonly Regex IdMatcher = new Regex(String.Format(@"#{0}", Css_Ident), RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex AttribMatcher = new Regex(String.Format(@"\[\s*{0}\s*(([$*^~|]?=)\s*({0}|{1})\s*)?\]", Css_Ident, Css_String), RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex ClassMatcher = new Regex(String.Format(@"\.{0}", Css_Ident), RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex ElemMatcher = new Regex(Css_Ident, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex PseudoClassMatcher = BuildOrRegex(PseudoClasses, ":", x => x.Replace("()", String.Format(@"\({0}\)", Css_Ident)));
		private static readonly Regex PseudoElemMatcher = BuildOrRegex(PseudoElements, "::?");
		private static readonly Regex PseudoUnimplemented = BuildOrRegex(UnimplementedPseudoSelectors, "::?");

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

            var ids = MatchCountAndStrip(IdMatcher, buffer, out buffer);
            var attributes = MatchCountAndStrip(AttribMatcher, buffer, out buffer);
            var classes = MatchCountAndStrip(ClassMatcher, buffer, out buffer);
            var pseudoClasses = MatchCountAndStrip(PseudoClassMatcher, buffer, out buffer);
            var elementNames = MatchCountAndStrip(ElemMatcher, buffer, out buffer);
            var pseudoElements = MatchCountAndStrip(PseudoElemMatcher, buffer, out buffer);

            var specificity = new CssSpecificity(ids, (classes + attributes + pseudoClasses), (elementNames + pseudoElements));
            return result + specificity;
        }

        public bool IsPseudoClass(string selector)
        {
            return PseudoClassMatcher.IsMatch(selector);
        }

        public bool IsPseudoElement(string selector)
        {
            return PseudoElemMatcher.IsMatch(selector);
        }

        /// <summary>
        /// Determines if the given CSS selector is supported. This is basically determined by what <seealso cref="CQ"/> supports.
        /// </summary>
        /// <param name="key"></param>
        /// <remarks>See https://github.com/jamietre/CsQuery#features for more information.</remarks>
        public bool IsSupportedSelector(string key)
        {
			return !PseudoUnimplemented.IsMatch(key);
        }

        private static int MatchCountAndStrip(Regex regex, string selector, out string result)
        {
            var matches = regex.Matches(selector);

            result = regex.Replace(selector, string.Empty);

            return matches.Count;
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

        private static string[] UnimplementedPseudoSelectors
        {
            get
            {
                // Based on: https://github.com/jamietre/CsQuery#missing-css-selectors
                return new[]
	            {
                    "link",
                    "hover",
                    "active",
                    "focus",
                    "visited",
                    "target",
                    "first-letter",
                    "first-line",
                    "before",
                    "after"
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
            return new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}