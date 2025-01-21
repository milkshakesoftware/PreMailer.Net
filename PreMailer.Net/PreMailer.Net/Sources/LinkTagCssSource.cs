using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
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
				return _cssContents ?? DownloadContents();
			}
			return string.Empty;
		}

		private string DownloadContents()
		{
			try
			{
				Console.WriteLine($"Will download from '{_downloadUri}' using {WebDownloader.SharedDownloader.GetType()}");

				_cssContents = WebDownloader.SharedDownloader.DownloadString(_downloadUri);
			}
			catch (WebException ex)
			{
				Console.WriteLine($"Download failed with: {ex}");
				throw new WebException($"PreMailer.Net is unable to download the requested URL: {_downloadUri}", ex);
			}

			// Fetch possible import rules
			_cssContents = ImportRuleCssSource.FetchImportRules(_downloadUri, _cssContents);

			return _cssContents;
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