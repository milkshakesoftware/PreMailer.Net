using System;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using PreMailer.Net.Downloaders;

namespace PreMailer.Net.Sources
{
	public class LinkTagCssSource : ICssSource
	{
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
			Console.WriteLine($"GetCss scheme: {_downloadUri.Scheme}");

			if (IsSupported(_downloadUri.Scheme))
			{
                try
                {
					Console.WriteLine($"Will download from '{_downloadUri}' using {WebDownloader.SharedDownloader.GetType()}");

										return _cssContents ?? (_cssContents = WebDownloader.SharedDownloader.DownloadString(_downloadUri));
                } catch (WebException)
                {
                    throw new WebException($"PreMailer.Net is unable to fetch the requested URL: {_downloadUri}");
                }
            }
			return string.Empty;
		}

		private static bool IsSupported(string scheme)
		{
			return
				scheme == "http" ||
				scheme == "https" ||
				scheme == "ftp" ||
				scheme == "file";
		}
	}
}