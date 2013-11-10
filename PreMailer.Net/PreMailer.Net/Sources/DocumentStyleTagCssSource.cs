using System;
using CsQuery;

namespace PreMailer.Net.Sources
{
	public class DocumentStyleTagCssSource : ICssSource
	{
		private readonly bool _ignoreMobile;
		private readonly IDomObject _node;

		public DocumentStyleTagCssSource(IDomObject node, bool ignoreMobile = true)
		{
			_node = node;
			_ignoreMobile = ignoreMobile;
		}

		public string GetCss()
		{
			if (_ignoreMobile && IsForMobile(_node))
				return string.Empty;

			return _node.InnerHTML;
		}

		internal bool IsForMobile(IDomObject styleNode)
		{
			return (styleNode.Attributes["id"] != null && !String.IsNullOrWhiteSpace(styleNode.Attributes["id"]) &&
							styleNode.Attributes["id"].Equals("mobile", StringComparison.InvariantCultureIgnoreCase));
		}
	}
}