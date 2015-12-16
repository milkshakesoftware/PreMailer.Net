using CsQuery;
using PreMailer.Net.Downloaders;
using System;
using System.Linq;

namespace PreMailer.Net.Sources
{
	public class LinkTagCssSource : ICssSource
	{
		private Uri _downloadUri;
		private string _cssContents;

		public LinkTagCssSource(IDomObject node, Uri baseUri)
		{
			// There must be an href
			var href = node.Attributes.First(a => a.Key.Equals("href", StringComparison.OrdinalIgnoreCase)).Value;

			if (Uri.IsWellFormedUriString(href, UriKind.Relative) && baseUri != null)
				_downloadUri = new Uri(baseUri, href);
			else // Assume absolute
				_downloadUri = new Uri(href);
		}

		public string GetCss()
		{
			if (_cssContents == null)
			{
				_cssContents = WebDownloader.SharedDownloader.DownloadString(_downloadUri);
			}

			return _cssContents;
		}
	}
}
