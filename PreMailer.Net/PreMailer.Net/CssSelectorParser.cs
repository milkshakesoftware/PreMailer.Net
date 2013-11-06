using System.Data.Odbc;
using System.Text.RegularExpressions;

namespace PreMailer.Net
{
    public class CssSelectorParser
    {
        public static CssSelectorParseResult Parse(string selector)
        {
            if (string.IsNullOrWhiteSpace(selector) || selector == "*")
                return CssSelectorParseResult.Empty();

            var buffer = selector;

            var ids = MatchCountAndStrip(@"#([\w]+)", buffer, out buffer);
            var attributes = MatchCountAndStrip(@"\[[\w=]+\]", buffer, out buffer);
            var classes = MatchCountAndStrip(@"\.([\w]+)", buffer, out buffer);
            var psuedo = MatchCountAndStrip(@":\w+", buffer, out buffer); // Psuedo Classes Are Ignored..
            var elementNames = MatchCountAndStrip(@"[a-zA-Z]+", buffer, out buffer);

            var specifity =
                (ids * 100) +
                (classes * 10) +
                (attributes * 10) +
                (elementNames * 1);

            return new CssSelectorParseResult(ids, classes, attributes, elementNames, specifity);
        }

        private static int MatchCountAndStrip(string regex, string selector, out string result)
        {
            var re = new Regex(regex, RegexOptions.IgnoreCase);
            var matches = re.Matches(selector);

            result = re.Replace(selector, string.Empty);

            return matches.Count;
        }
    }

    public class CssSelectorParseResult
    {
        public int Ids { get; protected set; }
        public int Classes { get; protected set; }
        public int Attributes { get; protected set; }
        public int ElementNames { get; protected set; }
        public int Specificity { get; protected set; }

        public CssSelectorParseResult(int ids, int classes, int attributes, int elementNames, int specificity)
        {
            Ids = ids;
            Classes = classes;
            Attributes = attributes;
            ElementNames = elementNames;
            Specificity = specificity;
        }

        public static CssSelectorParseResult Empty()
        {
            return new CssSelectorParseResult(0, 0, 0, 0, 0);
        }
    }
}