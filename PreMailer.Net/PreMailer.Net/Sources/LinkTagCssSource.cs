using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using PreMailer.Net.Downloaders;

namespace PreMailer.Net.Sources
{
	public class LinkTagCssSource : ICssSource
	{
		private readonly Uri _downloadUri;
		private List<string> _cssContents;
		private ImportRuleCssSource _importRuleCssSource;
		public LinkTagCssSource(IElement node, Uri baseUri) : this(node, baseUri, new ImportRuleCssSource())
		{
		}

		public LinkTagCssSource(IElement node, Uri baseUri, ImportRuleCssSource importRuleCssSource)
		{
			_importRuleCssSource = importRuleCssSource;

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

		public IEnumerable<string> GetCss()
		{
			Console.WriteLine($"GetCss scheme: {_downloadUri.Scheme}");

			if (IsSupported(_downloadUri.Scheme))
			{
				return _cssContents ?? DownloadContents();
			}
			return default;
		}

		private List<string> DownloadContents()
		{
			string content;
			_cssContents ??= new();

			try
			{
				Console.WriteLine($"Will download from '{_downloadUri}' using {WebDownloader.SharedDownloader.GetType()}");

				content = WebDownloader.SharedDownloader.DownloadString(_downloadUri);
			}
			catch (WebException ex)
			{
				Console.WriteLine($"Download failed with: {ex}");
				throw new WebException($"PreMailer.Net is unable to download the requested URL: {_downloadUri}", ex);
			}

			// Fetch possible import rules
			var imports = _importRuleCssSource.GetCss(_downloadUri, content);

			if (imports != null)
			{
				_cssContents.AddRange(imports);
			}

			_cssContents.Add(content);

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