namespace PreMailer.Net
{
    public interface ICssSelectorParser
    {
        int GetSelectorSpecificity(string selector);
    }
}