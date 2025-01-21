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
	/// <summary>
	/// This class is used by the LinkTagCssSource class for donwloading/fetching import rules.
	/// </summary>
	public class ImportRuleCssSource : ICssSource
	{
		private readonly Uri _downloadUri;
		private readonly int _level;
		private readonly StringBuilder _contentBuilder;
		private readonly List<Uri> _importList;

		private static Regex _importRegex = new Regex("@import.*?[\"'](?<href>[^\"']+)[\"'].*?;", RegexOptions.Multiline | RegexOptions.IgnoreCase);

		public ImportRuleCssSource(string importUri, Uri baseUri, int level, StringBuilder contentBuilder, List<Uri> importList)
		{

			if (Uri.IsWellFormedUriString(importUri, UriKind.Relative) && baseUri != null)
			{
				_downloadUri = new Uri(baseUri, importUri);
			}
			else
			{
				// Assume absolute
				_downloadUri = new Uri(importUri);
			}

			_level = level;
			_contentBuilder = contentBuilder;
			_importList = importList;
		}

		public string GetCss()
		{
			Console.WriteLine($"{new string('\t', _level + 1)}GetCss scheme: {_downloadUri.Scheme}");

			if (IsSupported(_downloadUri.Scheme) && !_importList.Contains(_downloadUri))
			{
				_importList.Add(_downloadUri);
				DownloadContents();
			}
			else if (_importList.Contains(_downloadUri))
			{
				Console.WriteLine($"{new string('\t', _level + 1)}Already got import from '{_downloadUri}'");
			}
			return string.Empty;
		}

		private void DownloadContents()
		{
			string cssContents;

			try
			{
				Console.WriteLine($"{new string('\t', _level + 1)}Will download import from '{_downloadUri}'");

				cssContents = WebDownloader.SharedDownloader.DownloadString(_downloadUri);
			}
			catch (WebException ex)
			{
				Console.WriteLine($"Download failed with: {ex}");
				throw new WebException($"PreMailer.Net is unable to download the requested URL: {_downloadUri}", ex);
			}

			if (_level < 2 && cssContents != null) // Stop processing imports at level 2
			{
				FetchImportRules(_downloadUri, cssContents, _level + 1, _contentBuilder, _importList);
			}
			_contentBuilder.AppendLine(cssContents);
		}

		private static bool IsSupported(string scheme)
		{
			return
				scheme == "http" ||
				scheme == "https";
		}


		/// <summary>
		/// This is the entry point for the LinkTagCssSource class when fetching its content.
		/// </summary>
		/// <param name="downloadUri"></param>
		/// <param name="contents"></param>
		/// <returns></returns>
		public static string FetchImportRules(Uri downloadUri, string contents)
		{
			if (contents == null)
			{
				return string.Empty;
			}

			var contentBuilder = new StringBuilder();
			var importList = new List<Uri>();

			// First fetch the content of any import rule
			FetchImportRules(downloadUri, contents, 0, contentBuilder, importList);

			// If there is no import rule found, then just return the contents 
			if (importList.Count == 0)
			{
				return contents;
			}
			else
			{
				// Now we append the content from the LinkTagCssSource to the builder
				contentBuilder.AppendLine(contents);
				return contentBuilder.ToString();
			}
		}

		/// <summary>
		/// This methods gets recursively called from within this class.
		/// </summary>
		/// <param name="downloadUri"></param>
		/// <param name="contents"></param>
		/// <param name="level"></param>
		/// <param name="contentBuilder"></param>
		/// <param name="importList"></param>
		private static void FetchImportRules(Uri downloadUri, string contents, int level, StringBuilder contentBuilder, List<Uri> importList)
		{

			string indent = level > 0 ? new string('\t', level) : string.Empty;
			int cnt = 0;

			var matches = GetMatches(contents);
			if (matches.Count > 0)
			{

				var baseUri = GetBaseUri(downloadUri);

				Console.WriteLine($"{indent}Found {matches.Count} import declarations");

				foreach (Match match in matches)
				{
					var url = match.Groups["href"].Value;

					Console.WriteLine($"{indent}{++cnt}: {url}");

					var importRuleSource = new ImportRuleCssSource(url, baseUri, level, contentBuilder, importList);
					importRuleSource.GetCss();
				}
			}
		}

		/// <summary>
		/// Extracted as a separate method for testing purposes.
		/// </summary>
		/// <param name="contents"></param>
		/// <returns></returns>
		public static MatchCollection GetMatches(string contents)
		{
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
