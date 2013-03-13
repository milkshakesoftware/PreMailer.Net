// No usings needed

namespace PreMailer.Parsing
{
	public interface IRuleSetParser
	{
		RuleSet ParseRuleSet(string selectors, string style);
	}
}