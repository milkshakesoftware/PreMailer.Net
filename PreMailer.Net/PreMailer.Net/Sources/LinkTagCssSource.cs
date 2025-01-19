using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using PreMailer.Net.Downloaders;

namespace PreMailer.Net.Sources
{
	public class LinkTagCssSource : ICssSource
	{
		private readonly Uri _downloadUri;
		private string _cssContents;

		private static Regex _importRegex = new Regex("@import.*?[\"'](?<href>[^\"']+)[\"'].*?;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
		private List<string> _cssImports;

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
				return _cssContents ?? (_cssContents = DownloadContents());
			}
			return string.Empty;
		}

		private string DownloadContents()
		{
			/*
			 * When we don't download the contents of the @import declarations, those style don't get applied.
			 * Many times a separate css file with variables or font declarations is included with an @import declaration.
			 * (Although it might be better not to use @import but to link css files with link elements)
			 * 
			 * Below is a simple implementation of downloading imports to a level of 2.
			 */

			var contents = DownloadContents(_downloadUri, new StringBuilder());
			return contents.ToString();
		}

		private StringBuilder DownloadContents(Uri downloadUri, StringBuilder cssContents = null, int level = 0)
		{
			string contents = null;
			string indent = level > 0 ? new string('\t', level) : string.Empty;

			Console.WriteLine($"{indent}Downloading '{downloadUri}' using {WebDownloader.SharedDownloader.GetType()}");

			try
			{
				contents = WebDownloader.SharedDownloader.DownloadString(downloadUri);
			}
			catch (WebException ex)
			{
				if (level == 0)
				{
					// Only throw when downloading the link resource

					Console.WriteLine($"{indent}Download failed with: {ex}");
					throw new WebException($"PreMailer.Net is unable to download the requested URL: {downloadUri}", ex);
				}
				// Allow imports to fail (contents will be null)
			}

			if (contents == null)
			{
				return cssContents;
			}

			// Get all import declarations from the source

			var matches = _importRegex.Matches(contents);
			if (matches.Count > 0 && level < 2) // Stop processing imports at level 2
			{
				_cssImports ??= new List<string>();
				var cnt = 0;

				Console.WriteLine($"{indent}Found {matches.Count} import declarations");

				// Construct a base url
				
				var baseUrl = new UriBuilder(downloadUri) { Port = -1, Query = string.Empty };

				// Strip of the css file segment
				var path = baseUrl.Path;
				baseUrl.Path = path.Substring(0, path.LastIndexOf('/') + 1);

				Console.WriteLine($"{indent}BaseUri={baseUrl}");

				foreach (Match match in matches)
				{
					var url = match.Groups["href"].Value;

					Console.WriteLine($"{indent}{++cnt}: {url}");
					
					if (url.StartsWith("/"))
					{
						// Relative to the root of the base url

						var builder = new UriBuilder(baseUrl.Scheme, baseUrl.Host) { Path = url, Port = -1 };
						url = builder.ToString();
					}
					else
					{
						// Relative to the current directory
						url = baseUrl.ToString() + url;
					}

					if (!_cssImports.Contains(url) && Uri.TryCreate(url, UriKind.Absolute, out var importUri) && IsSupported(importUri.Scheme))
					{
						// More than one css can reference the same css to import.
						// Only get the contents when whe don't already got it.

						_cssImports.Add(url);

						// Get the contents of the css to import
						DownloadContents(importUri, cssContents, level + 1);
					}
					else
					{
						Console.WriteLine($"{indent}\tSkipping {url}");
					}
				}
			}

			cssContents.AppendLine(contents);

			return cssContents;
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