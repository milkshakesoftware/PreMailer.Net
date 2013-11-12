namespace PreMailer.Net
{
	public interface ICssSelectorParser
	{
		int GetSelectorSpecificity(string selector);
		bool IsPseudoElement(string selector);
		bool IsPseudoClass(string selector);
	}
}