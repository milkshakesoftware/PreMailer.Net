using AngleSharp.Dom;
using PreMailer.Net.Downloaders;
using System;
using System.Linq;

namespace PreMailer.Net.Sources
{
	public class LinkTagCssSource : ICssSource
	{
		private static readonly string[] _validUriSchemes = new string[] { "http", "https", "ftp", "file" };
		private readonly Uri _downloadUri;
		private string _cssContents;

		public LinkTagCssSource(IElement node, Uri baseUri)
		{
			// There must be an href
			var href = node.Attributes.First(a => a.Name.Equals("href", StringComparison.OrdinalIgnoreCase)).Value;

			if (Uri.IsWellFormedUriString(href, UriKind.Relative) && baseUri != null)
			{
				_downloadUri = new Uri(baseUri, href);
			}
			else
			{
				// Assume absolute
				_downloadUri = new Uri(href);
			}
		}

		public string GetCss()
		{
			if (IsSupported(_downloadUri.Scheme))
			{
				return _cssContents ?? (_cssContents = WebDownloader.SharedDownloader.DownloadString(_downloadUri));
			}

			return string.Empty;
		}

		private bool IsSupported(string scheme)
		{
			return _validUriSchemes.Contains(scheme?.ToLowerInvariant());
		}
	}
}