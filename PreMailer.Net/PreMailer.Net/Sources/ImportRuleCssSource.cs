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
		private Dictionary<Uri, string> _importList;

		private static Regex _importRegex = new Regex("@import.*?[\"'](?<href>[^\"']+)[\"'].*?;", RegexOptions.Multiline | RegexOptions.IgnoreCase);

		public ImportRuleCssSource(string importUri, Uri baseUri, int level, Dictionary<Uri, string> importList)
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
			_importList = importList;
		}

		public IEnumerable<string> GetCss()
		{
			Console.WriteLine($"{new string('\t', _level + 1)}GetCss scheme: {_downloadUri.Scheme}");

			if (IsSupported(_downloadUri.Scheme) && !_importList.ContainsKey(_downloadUri))
			{
				DownloadContents();
			}
			else if (_importList.ContainsKey(_downloadUri))
			{
				Console.WriteLine($"{new string('\t', _level + 1)}Already got import from '{_downloadUri}'");
			}

			// Everything is added to the _importList which is passed in with the constructor.
			// So, we don't have to return anything here.

			return default;
		}

		private void DownloadContents()
		{
			string contents;
			var indent = new string('\t', _level + 1);

			try
			{
				Console.WriteLine($"{indent}Will download import from '{_downloadUri}'");

				contents = WebDownloader.SharedDownloader.DownloadString(_downloadUri);
			}
			catch (WebException ex)
			{
				Console.WriteLine($"Download failed with: {ex}");
				throw new WebException($"PreMailer.Net is unable to download the requested URL: {_downloadUri}", ex);
			}

			if (_level < 2 && contents != null) // Stop processing imports at level 2
			{
				FetchImportRules(_downloadUri, contents, _level + 1, ref _importList);
			}
			
			// Prevent a recursive import of the same url
			if (!_importList.ContainsKey( _downloadUri))
			{
				_importList.Add(_downloadUri, contents);
			}
			else
			{
				Console.WriteLine($"{indent}An import added {_downloadUri} in the meantime!");
			}

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
		public static IList<string> FetchImportRules(Uri downloadUri, string contents)
		{
			if (contents == null) // Is the case with testing
			{
				return null;
			}


			var importList = new Dictionary<Uri, string>();

			// First fetch the content of any import rule
			FetchImportRules(downloadUri, contents, 0, ref importList);

			return importList.Values.ToList();

		}

		/// <summary>
		/// This methods gets recursively called from within this class.
		/// </summary>
		/// <param name="downloadUri"></param>
		/// <param name="contents"></param>
		/// <param name="level"></param>
		/// <param name="contentBuilder"></param>
		/// <param name="importList"></param>
		private static void FetchImportRules(Uri downloadUri, string contents, int level, ref Dictionary<Uri, string> importList)
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

					var importRuleSource = new ImportRuleCssSource(url, baseUri, level, importList);
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
