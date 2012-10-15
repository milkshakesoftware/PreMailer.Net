using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PreMailerDotNet.Parsing
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

		public Selector ParseSelector(string selectorName)
		{
			Selector selector = new Selector();
			selector.SelectorName = selectorName;
			selector.SelectorType = GetCssSelectorType(selectorName);

			return selector;
		}

		private static SelectorTypes GetCssSelectorType(string selector)
		{
			if (String.IsNullOrWhiteSpace(selector))
			{
				return SelectorTypes.InlineStyle;
			}

			string[] selectorParts = selector.Split(' ');
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
	}
}