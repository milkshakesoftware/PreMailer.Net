// No usings needed

namespace PreMailerDotNet.Parsing
{
	public interface IRuleSetParser
	{
		RuleSet ParseRuleSet(string selectors, string style);
	}
}