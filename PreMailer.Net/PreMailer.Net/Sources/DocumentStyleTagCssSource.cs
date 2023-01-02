using AngleSharp.Dom;
using PreMailer.Net.Extensions;

namespace PreMailer.Net.Sources
{
	public class DocumentStyleTagCssSource : ICssSource
	{
		private readonly IElement _node;

		public DocumentStyleTagCssSource(IElement node)
		{
			_node = node;
		}

		public string GetCss()
		{
			return _node.GetFirstTextNodeData() ?? "";
		}
	}
}