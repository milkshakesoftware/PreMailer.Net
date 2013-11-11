using CsQuery;

namespace PreMailer.Net.Sources
{
	public class DocumentStyleTagCssSource : ICssSource
	{
		private readonly IDomObject _node;

		public DocumentStyleTagCssSource(IDomObject node)
		{
			_node = node;
		}

		public string GetCss()
		{
			return _node.InnerHTML;
		}
	}
}