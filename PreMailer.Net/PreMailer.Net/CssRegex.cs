using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PreMailer.Net {
    internal class CssRegex {
        private static string PossibleValues(params string[] values)
        {
            return String.Format(@"([\s]*^)?({0})([\s]*$)?", String.Join("|", values));
        }

        // Base types
        internal static readonly string RE_NL = @"(\n|\r\n|\r|\f)";
        internal static readonly string RE_NON_ASCII = @"([\x00-\xFF])";
        internal static readonly string RE_UNICODE = @"(\\\\[0-9a-f]{1,6}(\r\n|[ \n\r\t\f])*)";
        internal static readonly string RE_ESCAPE = RE_UNICODE + @"|(\\\\[^\n\r\f0-9a-f])";
        internal static readonly string RE_IDENT = String.Format(@"[\-]?([_a-z]|{0}|{1})([_a-z0-9\-]|{0}|{1})*", RE_NON_ASCII, RE_ESCAPE);
        
        // General strings
        internal static readonly string RE_STRING1 = @"(\""(.[^\n\r\f\\""]*|\\\\" + RE_NL + "|" + RE_ESCAPE + @")*\"")";
        internal static readonly string RE_STRING2 = @"(\'(.[^\n\r\f\\\']*|\\\\" + RE_NL + "|" + RE_ESCAPE + ")*\')";
        internal static readonly string RE_STRING = RE_STRING1 + RE_STRING2;

        internal static readonly string RE_INHERIT = PossibleValues("inherit");
        internal static readonly string RE_URI = @"(url\([\s]*([\s]*" + RE_STRING + @"[\s]*)[\s]*\))|(url\([\s]*([!#$%&*\-~]|" + RE_NON_ASCII + "|" + RE_ESCAPE + @")*[\s]*)\)";
        internal static readonly string URI_RX = @"url\((""([^""]*)""|'([^']*)'|([^)]*))\)";
        internal static readonly string RE_GRADIENT = @"[-a-z]*gradient\([-a-z0-9 .,#%()]*\)";

        // Initial parsing
        internal static readonly string RE_AT_IMPORT_RULE = @"@import[\s]+(url\()?[""''""]?(.[^'""\s""']*)[""''""]?\)?([\w\s\,^\])]*)\)?;?";

        internal static readonly string IMPORTANT_IN_PROPERTY_RX = @"[\s]*!important\b[\s]*";

        internal static readonly string RE_INSIDE_OUTSIDE = PossibleValues("inside", "outside");
        internal static readonly string RE_SCROLL_FIXED = PossibleValues("scroll", "fixed");
        internal static readonly string RE_REPEAT = PossibleValues(@"repeat(\-x|\-y)*|no\-repeat");

        internal static readonly string RE_LIST_STYLE_TYPE = PossibleValues("disc", "circle", "square",
            "decimal-leading-zero", "decimal", "lower-roman",
            "upper-roman", "lower-greek", "lower-alpha", "lower-latin", "upper-alpha",
            "upper-latin", "hebrew", "armenian", "georgian", "cjk-ideographic", "hiragana",
            "hira-gana-iroha", "katakana-iroha", "katakana", "none");

        internal static readonly string STRIP_CSS_COMMENTS_RX = @"\/\*.*?\*\/";
        internal static readonly string STRIP_HTML_COMMENTS_RX = @"\<\!\-\-|\-\-\>";

        // Special units
        internal static readonly string BOX_MODEL_UNITS_RX = @"(auto|inherit|0|([\-]*([0-9]+|[0-9]*\.[0-9]+)(e[mx]+|px|[cm]+m|p[tc+]|in|\%)))([\s;]|\Z)";
        internal static readonly string RE_LENGTH_OR_PERCENTAGE = @"([\-]*(([0-9]*\.[0-9]+)|[0-9]+)(e[mx]+|px|[cm]+m|p[tc+]|in|\%))";
        internal static readonly string RE_BACKGROUND_POSITION = @"((((" + RE_LENGTH_OR_PERCENTAGE + @")|left|center|right|top|bottom)[\s]*){1,2})";
        internal static readonly string FONT_UNITS_RX = @"(([x]+\-)*small|medium|large[r]*|auto|inherit|([0-9]+|[0-9]*\.[0-9]+)(e[mx]+|px|[cm]+m|p[tc+]|in|\%)*)";
        internal static readonly string RE_BORDER_STYLE = @"([\s]*^)?(none|hidden|dotted|dashed|solid|double|dot-dash|dot-dot-dash|wave|groove|ridge|inset|outset)([\s]*$)?";
        internal static readonly string RE_BORDER_UNITS = BOX_MODEL_UNITS_RX + @"(thin|medium|thick)";


        // Patterns for specificity calculations
        internal static readonly string NON_ID_ATTRIBUTES_AND_PSEUDO_CLASSES_RX = @"
            (\.[\w]+)                     # classes
            |
            \[(\w+)                       # attributes
            |
            (\:(                          # pseudo classes
                link|visited|active
                |hover|focus
                |lang
                |target
                |enabled|disabled|checked|indeterminate
                |root
                |nth-child|nth-last-child|nth-of-type|nth-last-of-type
                |first-child|last-child|first-of-type|last-of-type
                |only-child|only-of-type
                |empty|contains
            ))";

        internal static readonly string ELEMENTS_AND_PSEUDO_ELEMENTS_RX = @"
            ((^|[\s\+\>\~]+)[\w]+       # elements
            |
            \:{1,2}(                    # pseudo-elements
                after|before
                |first-letter|first-line
                |selection
            )
            )";

        // Colours
        internal static readonly string RE_COLOUR_NUMERIC = @"((hsl|rgb)[\s]*\([\s-]*[\d]+(\.[\d]+)?[%\s]*,[\s-]*[\d]+(\.[\d]+)?[%\s]*,[\s-]*[\d]+(\.[\d]+)?[%\s]*\))";
        internal static readonly string RE_COLOUR_NUMERIC_ALPHA = @"((hsla|rgba)[\s]*\([\s-]*[\d]+(\.[\d]+)?[%\s]*,[\s-]*[\d]+(\.[\d]+)?[%\s]*,[\s-]*[\d]+(\.[\d]+)?[%\s]*,[\s-]*[\d]+(\.[\d]+)?[%\s]*\))";
        internal static readonly string RE_COLOUR_HEX = @"(#([0-9a-f]{6}|[0-9a-f]{3})([\s;]|$))";
        internal static readonly string RE_COLOUR_NAMED = @"([\s]*^)?(aqua|black|blue|fuchsia|gray|green|lime|maroon|navy|olive|orange|purple|red|silver|teal|white|yellow|transparent)([\s]*$)?";
        internal static readonly string RE_COLOUR = RE_COLOUR_NUMERIC + RE_COLOUR_NUMERIC_ALPHA + RE_COLOUR_HEX + RE_COLOUR_NAMED;
    }
}
