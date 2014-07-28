using System.Collections.Generic;
using System.Linq;
using CsQuery;

namespace PreMailer.Net
{
    public class CssStyleEquivalence
    {
        private static readonly Dictionary<string, string> _linkedAttributes = new Dictionary<string, string>
            {
                {"bgcolor", "background-color"},
                {"width", "width"},
                {"height", "height"}
            };


        public static IList<AttributeToCss> FindEquivalent(IDomObject domobject, StyleClass styles)
        {

            return (from attributeRuleMatch in _linkedAttributes
                    where domobject.HasAttribute(attributeRuleMatch.Key) && styles.Attributes.ContainsKey(attributeRuleMatch.Value)
                    select new AttributeToCss
                        {
                            AttributeName = attributeRuleMatch.Key, CssValue = styles.Attributes[attributeRuleMatch.Value].Value
                        }).ToList();
        }
    }
}