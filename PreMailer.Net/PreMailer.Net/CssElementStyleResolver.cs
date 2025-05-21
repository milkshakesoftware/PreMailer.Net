using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;

namespace PreMailer.Net
{
	public static class CssElementStyleResolver
	{
		private const string premailerAttributePrefix = "-premailer-";

	public static IEnumerable<AttributeToCss> GetAllStyles(IElement domElement, StyleClass styleClass)
	{
		var attributeCssList = new List<AttributeToCss>();
		var originalStyleAttr = domElement.Attributes["style"];
		var hasImportantInOriginalStyle = originalStyleAttr != null && originalStyleAttr.Value.Contains("!important");

		AddSpecialPremailerAttributes(attributeCssList, styleClass);

		if (styleClass.Attributes.Count > 0)
			attributeCssList.Add(new AttributeToCss { AttributeName = "style", CssValue = styleClass.ToString(emitImportant: true) });

		attributeCssList.AddRange(CssStyleEquivalence.FindEquivalent(domElement, styleClass));

		if (hasImportantInOriginalStyle)
		{
			var styleAttr = attributeCssList.FirstOrDefault(a => a.AttributeName == "style");
			if (styleAttr != null && !styleAttr.CssValue.Contains("!important"))
			{
				var parser = new CssParser();
				var originalStyleClass = parser.ParseStyleClass("inline", originalStyleAttr.Value);
				
				var currentStyleClass = parser.ParseStyleClass("inline", styleAttr.CssValue);
				
				foreach (var attr in originalStyleClass.Attributes)
				{
					if (attr.Important && !currentStyleClass.Attributes.ContainsKey(attr.Style))
					{
						currentStyleClass.Attributes.Merge(attr);
					}
				}
				
				styleAttr.CssValue = currentStyleClass.ToString(emitImportant: true);
			}
		}

		return attributeCssList;
	}

		private static void AddSpecialPremailerAttributes(List<AttributeToCss> attributeCssList, StyleClass styleClass)
		{
			while (true)
			{
				var premailerRuleMatch = styleClass.Attributes.FirstOrDefault(a => a.Style.StartsWith(premailerAttributePrefix));

				if (premailerRuleMatch == null)
					break;

				var key = premailerRuleMatch.Style;
				var cssAttribute = premailerRuleMatch.Value;

				attributeCssList.Add(new AttributeToCss
				{
					AttributeName = key.Replace(premailerAttributePrefix, ""),
					CssValue = cssAttribute
				});

				styleClass.Attributes.Remove(key);
			}
		}
	}
}
