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

			AddSpecialPremailerAttributes(attributeCssList, styleClass);

			if (styleClass.Attributes.Count > 0)
				attributeCssList.Add(new AttributeToCss { AttributeName = "style", CssValue = styleClass.ToString(removeImportant: true) });

			attributeCssList.AddRange(CssStyleEquivalence.FindEquivalent(domElement, styleClass));

			return attributeCssList;
		}

		private static void AddSpecialPremailerAttributes(List<AttributeToCss> attributeCssList, StyleClass styleClass)
		{
			while (true)
			{
				var premailerRuleMatch = styleClass.Attributes.FirstOrDefault(a => a.Key.StartsWith(premailerAttributePrefix));

				var key = premailerRuleMatch.Key;
				var cssAttribute = premailerRuleMatch.Value;

				if (key == null)
					break;

				attributeCssList.Add(new AttributeToCss
				{
					AttributeName = key.Replace(premailerAttributePrefix, ""),
					CssValue = cssAttribute.Value
				});

				styleClass.Attributes.Remove(key);
			}
		}
	}
}