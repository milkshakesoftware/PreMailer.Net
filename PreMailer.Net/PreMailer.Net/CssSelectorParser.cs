using System.Text.RegularExpressions;

namespace PreMailer.Net
{
    public class CssSelectorParser : ICssSelectorParser
    {
        private readonly Regex _idMatcher;
        private readonly Regex _attribMatcher;
        private readonly Regex _classMatcher;
        private readonly Regex _psuedoMatcher;
        private readonly Regex _elemMatcher;

        public CssSelectorParser()
        {
            _idMatcher = new Regex(@"#([\w]+)", RegexOptions.Compiled & RegexOptions.IgnoreCase);
            _attribMatcher = new Regex(@"\[[\w=]+\]", RegexOptions.Compiled & RegexOptions.IgnoreCase);
            _classMatcher = new Regex(@"\.([\w]+)", RegexOptions.Compiled & RegexOptions.IgnoreCase);
            _psuedoMatcher = new Regex(@":\w+", RegexOptions.Compiled & RegexOptions.IgnoreCase);
            _elemMatcher = new Regex(@"[a-zA-Z]+", RegexOptions.Compiled & RegexOptions.IgnoreCase);
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
            if (string.IsNullOrWhiteSpace(selector) || selector == "*")
                return 0;

            var buffer = selector;

            var ids = MatchCountAndStrip(_idMatcher, buffer, out buffer);
            var attributes = MatchCountAndStrip(_attribMatcher, buffer, out buffer);
            var classes = MatchCountAndStrip(_classMatcher, buffer, out buffer);
            var psuedo = MatchCountAndStrip(_psuedoMatcher, buffer, out buffer); // Psuedo Classes Are Ignored..
            var elementNames = MatchCountAndStrip(_elemMatcher, buffer, out buffer);

            var specifity =
                (ids * 100) +
                (classes * 10) +
                (attributes * 10) +
                (elementNames * 1);

            return specifity;
        }

        private static int MatchCountAndStrip(Regex regex, string selector, out string result)
        {
            var matches = regex.Matches(selector);

            result = regex.Replace(selector, string.Empty);

            return matches.Count;
        }
    }
}