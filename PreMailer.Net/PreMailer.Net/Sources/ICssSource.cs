namespace PreMailer.Net.Sources
{
    /// <summary>
    /// Arbitrary source of CSS code/defintions.
    /// </summary>
    public interface ICssSource
    {
        string GetCss();
    }
}