using System.Collections.Generic;
using CsQuery;

namespace PreMailer.Net
{
    public class StyleClassApplier
    {
        public static Dictionary<IDomObject, StyleClass> ApplyAllStyles(Dictionary<IDomObject, StyleClass> elementDictionary)
        {
            foreach (var styleClass in elementDictionary)
            {
                ApplyStyles(styleClass.Key, styleClass.Value);
            }

            return elementDictionary;
        }

        private static IDomObject ApplyStyles(IDomObject domElement, StyleClass clazz)
        {
            var styles = CssElementStyleResolver.GetAllStyles(domElement, clazz);

            foreach (var attributeToCss in styles)
            {
                PrepareAttribute(domElement, attributeToCss);
                //domElement.SetAttribute(attributeToCss.AttributeName, attributeToCss.CssValue);
            }

            return domElement;
        }

        private static void PrepareAttribute(IDomObject domElement, AttributeToCss attributeToCss)
        {
            string name = attributeToCss.AttributeName;
            string value = attributeToCss.CssValue;

            //When rendering images, we need to prevent breaking the WIDTH and HEIGHT attributes. See PreMailerTests.MoveCssInline_HasStyle_DoesNotBreakImageWidthAttribute().
            //The old code could end up writing an image tag like <img width="206px"> which violates the HTML spec. It should render <img width="206">.
            if (domElement.NodeName == @"IMG"
                && (name == "width" || name == "height")
                && value.EndsWith("px"))
            {
                value = value.Replace("px", string.Empty);
            }

            domElement.SetAttribute(name, value);
        }
    }
}