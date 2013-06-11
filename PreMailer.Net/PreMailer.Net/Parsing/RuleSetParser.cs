using System;

namespace PreMailer.Parsing
{
	public class RuleSetParser : IRuleSetParser
	{
		private readonly ISelectorParser _selectorParser;

		public RuleSetParser()
		{
			this._selectorParser = new SelectorParser();
		}

		public RuleSetParser(ISelectorParser selectorParser)
		{
			if (selectorParser != null)
			{
				this._selectorParser = selectorParser;
			}
			else
			{
				this._selectorParser = new SelectorParser();
			}
		}

		public virtual RuleSet ParseRuleSet(string selectors, string style)
		{
			if (style == null)
			{
				throw new ArgumentNullException("style");
			}

			if (StringExtensions.IsNullOrWhiteSpace(style))
			{
				throw new ArgumentException("The style parameter is empty!", "style");
			}

			RuleSet rule = new RuleSet();

			this.ParseSelectors(selectors, rule);
			ParseStyleAttributes(style, rule);

			return rule;
		}

		private void ParseSelectors(string selectors, RuleSet rule)
		{
			string[] selectorParts = selectors.Split(',');

			foreach (var part in selectorParts)
			{
				rule.Selectors.Add(this._selectorParser.ParseSelector(part));
			}
		}

		private static void ParseStyleAttributes(string style, RuleSet rule)
		{
			string[] styleParts = style.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			foreach (var part in styleParts)
			{
				string[] attributeAndValueParts = part.Split(':');

				rule.Attributes.Add(attributeAndValueParts[0], "n");
			}
		}
	}
}