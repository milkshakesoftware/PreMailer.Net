using System.Collections.Generic;
using CsQuery;

namespace PreMailer.Net
{
   public class CssElementStyleResolver
   {
        public static IEnumerable<AttributeToCss> GetAllStyles(IDomObject domElement, StyleClass clazz)
        {
            var attributeCssList = new List<AttributeToCss>
                {
                    new AttributeToCss {AttributeName = "style", CssValue = clazz.ToString()}
                };

            attributeCssList.AddRange(CssStyleEquivalence.FindEquivalent(domElement, clazz));

            return attributeCssList;
        }
    }
}