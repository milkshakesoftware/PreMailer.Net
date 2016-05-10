// No usings needed

namespace PreMailer.Net.Sources
{
	public class StringCssSource : ICssSource
	{
		private readonly string _css;

		public StringCssSource(string css)
		{
			this._css = css;
		}

		public string GetCss()
		{
			return _css;
		}
	}
}