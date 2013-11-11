using System.Text.RegularExpressions;

namespace PreMailer.Net
{
	public class CssSelector
	{
		protected static Regex NotMatcher = new Regex(@":not\((.+)\)", RegexOptions.IgnoreCase & RegexOptions.Compiled);

		public string Selector { get; protected set; }

		public bool HasNotPsuedoClass
		{
			get
			{
				return NotMatcher.IsMatch(Selector);
			}
		}
		
		public string NotPsuedoClassContent
		{
			get
			{
				var match = NotMatcher.Match(Selector);
				return match.Success ? match.Groups[1].Value : null;
			}
		}

		public CssSelector(string selector)
		{
			Selector = selector;
		}

		public CssSelector StripNotPsuedoClassContent()
		{
			var stripped = NotMatcher.Replace(Selector, string.Empty);
			return new CssSelector(stripped);
		}

		public override string ToString()
		{
			return Selector;
		}
	}
}