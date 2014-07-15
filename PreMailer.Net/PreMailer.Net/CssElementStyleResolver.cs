using System.Collections.Generic;
using CsQuery;

namespace PreMailer.Net
{
   public class CssElementStyleResolver
   {
        public static IEnumerable<AttributeToCss> GetAllStyles(IDomObject domElement, StyleClass styleClass)
        {
            var attributeCssList = new List<AttributeToCss>
                {
                    new AttributeToCss {AttributeName = "style", CssValue = styleClass.ToString()}
                };

            attributeCssList.AddRange(CssStyleEquivalence.FindEquivalent(domElement, styleClass));

            return attributeCssList;
        }
    }
}