using System.Collections.Generic;

namespace PreMailer.Net.Sources
{
	public class StringCssSource : ICssSource
	{
		private readonly string _css;

		public StringCssSource(string css)
		{
			this._css = css;
		}

		public IEnumerable<string> GetCss()
		{
			return [_css];
		}
	}
}