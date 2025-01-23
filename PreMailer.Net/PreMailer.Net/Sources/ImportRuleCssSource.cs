using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using PreMailer.Net.Downloaders;

namespace PreMailer.Net.Sources
{
	/// <summary>
	/// This class is used by the LinkTagCssSource class for downloading/fetching import rules.
	/// </summary>
	public class ImportRuleCssSource
	{
		private Dictionary<Uri, string> _importList = new Dictionary<Uri, string>();
		private static Regex _importRegex = new Regex("@import.*?[\"'](?<href>[^\"']+)[\"'].*?;", RegexOptions.Multiline | RegexOptions.IgnoreCase);

		public IEnumerable<string> GetCss(Uri linkedStylesheetUrl, string contents, int level = 0)
		{
			if (level >= 2 || string.IsNullOrEmpty(contents))
			{
				return _importList.Values;
			}

			var baseUri = GetBaseUri(linkedStylesheetUrl);
			var matches = GetMatches(contents);

			foreach (Match match in matches)
			{
				var href = match.Groups["href"].Value;
				Uri url = default;

				if (Uri.IsWellFormedUriString(href, UriKind.Relative))
				{
					url = new Uri(baseUri, href);
				}
				else
				{
					url = new Uri(href);
				}

				if (!_importList.ContainsKey(url))
				{
					var content = DownloadContents(url);

					_importList.Add(url, content);

					GetCss(url, content, level + 1);
				}
			}

			return _importList.Values;
		}

		private string DownloadContents(Uri downloadUri)
		{
			string contents;

			try
			{
				contents = WebDownloader.SharedDownloader.DownloadString(downloadUri);
			}
			catch (WebException ex)
			{
				throw new WebException($"PreMailer.Net is unable to download the requested URL: {downloadUri}", ex);
			}

			return contents;
		}

		private static bool IsSupported(string scheme) => scheme == "http" || scheme == "https";

		private static MatchCollection GetMatches(string contents)
		{
			if (string.IsNullOrEmpty(contents))
			{
				return default;
			}

			return _importRegex.Matches(contents);
		}

		private static Uri GetBaseUri(Uri downloadUri)
		{
			var baseUrl = new UriBuilder(downloadUri)
			{
				Port = -1 /* Excludes the port number */,
				Query = string.Empty
			};

			// Strip of the css file segment
			var path = baseUrl.Path;
			baseUrl.Path = path.Substring(0, path.LastIndexOf('/') + 1);

			return baseUrl.Uri;
		}
	}
}
