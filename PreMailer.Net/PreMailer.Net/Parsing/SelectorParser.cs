using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PreMailer.Parsing
{
	public class SelectorParser : ISelectorParser
	{
		private static Dictionary<string, SelectorTypes> _qualifierMap = new Dictionary<string, SelectorTypes>
		{
			{ "#", SelectorTypes.Id },
			{ ".", SelectorTypes.ClassName },
			{ "]", SelectorTypes.Attribute },
			{ ":", SelectorTypes.PseudoClass },
			{ "::", SelectorTypes.PseudoElement }
		};

		private static Dictionary<SelectorTypes, int> _specificityMap = new Dictionary<SelectorTypes, int>
		{
			{ SelectorTypes.InlineStyle, 1000 },
			{ SelectorTypes.Id, 100 },
			{ SelectorTypes.ClassName, 10 },
			{ SelectorTypes.Attribute, 10 },
			{ SelectorTypes.PseudoClass, 10 },
			{ SelectorTypes.Element, 1 },
			{ SelectorTypes.PseudoElement, 1 }
		};

		public virtual Selector ParseSelector(string cssSelector)
		{
			if (!StringExtensions.IsNullOrWhiteSpace(cssSelector))
			{
				cssSelector = cssSelector.Trim();
			}

			Selector selector = new Selector();
			selector.SelectorName = cssSelector;
			selector.SelectorType = GetCssSelectorType(cssSelector);
			selector.Specificity = GetCssSpecificity(cssSelector);

			return selector;
		}

		private int GetCssSpecificity(string cssSelector)
		{
			if (StringExtensions.IsNullOrWhiteSpace(cssSelector))
			{
				return _specificityMap[SelectorTypes.InlineStyle];
			}

			int specificity = 0;
			string[] selectorParts = CleanUp(cssSelector).Split("¤".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			foreach (var part in selectorParts)
			{
				var selectorType = GetCssSelectorType(part);
				specificity += _specificityMap[selectorType];
			}

			return specificity;
		}

		private static SelectorTypes GetCssSelectorType(string selector)
		{
			if (StringExtensions.IsNullOrWhiteSpace(selector))
			{
				return SelectorTypes.InlineStyle;
			}

			string[] selectorParts = selector.Trim().Split(' ');
			string decidingPart = selectorParts.Last();

			int lastIndex = -1;
			SelectorTypes currentSelectorType = SelectorTypes.Element;

			foreach (var quailifier in _qualifierMap)
			{
				int index = decidingPart.LastIndexOf(quailifier.Key);

				if (index > lastIndex || IsMoreExactPseudoMatch(lastIndex, currentSelectorType, quailifier, index))
				{
					lastIndex = index;
					currentSelectorType = quailifier.Value;
				}
			}

			return currentSelectorType;
		}

		private static bool IsMoreExactPseudoMatch(int lastIndex, SelectorTypes currentSelectorType, KeyValuePair<string, SelectorTypes> quailifier, int index)
		{
			return index > -1 && index < lastIndex && currentSelectorType == SelectorTypes.PseudoClass && quailifier.Value == SelectorTypes.PseudoElement;
		}

		private static string CleanUp(string s)
		{
			string temp = s;
			string reg = @">|\+";

			Regex r = new Regex(reg);
			temp = r.Replace(temp, String.Empty);

			// Replace attribute selectors with a dummy value, to avoid unintended split with selectors such as: div a[href=#remove]
			r = new Regex(@"(\[([^']*)\])");
			temp = r.Replace(temp, "[data-id=1]");

			temp = temp.Replace("#", "¤#");
			temp = temp.Replace(".", "¤.");
			temp = temp.Replace("[", "¤[");
			temp = temp.Replace("::", "¤__");
			temp = temp.Replace(":", "¤:");
			temp = temp.Replace("¤__", "¤::");
			temp = temp.Replace(" ", "¤");

			return temp;
		}
	}
}