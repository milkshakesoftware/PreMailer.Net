using System.Collections.Generic;
using CsQuery;

namespace PreMailer.Net
{
    public class StyleClassApplier
    {
        private static IDomObject ApplyStyles(IDomObject domElement, StyleClass clazz)
        {
            var styles = CssElementStyleResolver.GetAllStyles(domElement, clazz);

            foreach (var attributeToCss in styles)
            {
                domElement.SetAttribute(attributeToCss.AttributeName, attributeToCss.CssValue);
            }

            return domElement;
        }

        public static Dictionary<IDomObject, StyleClass> ApplyAllStyles(Dictionary<IDomObject, StyleClass> elementDictionary)
        {
            foreach (var styleClass in elementDictionary)
            {
                ApplyStyles(styleClass.Key, styleClass.Value);
            }

            return elementDictionary;
        }
    }
}