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
		
		AddSpecialPremailerAttributes(attributeCssList, styleClass);
		
		var mergedStyleClass = new StyleClass();
		
		if (styleClass.Attributes.Count > 0)
		{
			mergedStyleClass.Merge(styleClass, true);
		}
		
		if (originalStyleAttr != null)
		{
			var parser = new CssParser();
			var originalStyleClass = parser.ParseStyleClass("inline", originalStyleAttr.Value);
			
			foreach (var attr in originalStyleClass.Attributes)
			{
				if (attr.Important)
				{
					mergedStyleClass.Attributes.Merge(attr);
				}
				else if (!mergedStyleClass.Attributes.TryGetValue(attr.Style, out var existing) || !existing.Important)
				{
					mergedStyleClass.Attributes.Merge(attr);
				}
			}
		}
		
		if (mergedStyleClass.Attributes.Count > 0)
		{
			attributeCssList.Add(new AttributeToCss { AttributeName = "style", CssValue = mergedStyleClass.ToString(emitImportant: true) });
		}
		
		attributeCssList.AddRange(CssStyleEquivalence.FindEquivalent(domElement, styleClass));
		
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
