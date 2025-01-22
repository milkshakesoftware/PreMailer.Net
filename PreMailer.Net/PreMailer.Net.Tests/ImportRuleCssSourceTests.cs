using AngleSharp.Html.Parser;
using Xunit;
using Moq;
using PreMailer.Net.Downloaders;
using PreMailer.Net.Sources;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;


namespace PreMailer.Net.Tests
{
	public class ImportRuleCssSourceTests
	{
		private readonly Mock<IWebDownloader> _webDownloader = new Mock<IWebDownloader>();


		[Fact]
		public void FetchImportRules()
		{
			var baseUri = new Uri("https://a.com");
			var urls = new List<string>() { "variables.css", "/fonts.css", "https://fonts.google.com/css/test-font" };

			var css = CreateCss(urls);
			var contents = ImportRuleCssSource.FetchImportRules(baseUri, css);

			foreach (var url in urls)
			{
				_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.Equals(new Uri(baseUri, url)))));
			}

		}

		private string CreateCss(IEnumerable<string> imports)
		{
			WebDownloader.SharedDownloader = _webDownloader.Object;

			var builder = new StringBuilder();
			foreach (var import in imports)
			{
				builder.AppendLine($"@import \"{import}\";");
			}

			return builder.ToString();
		}
	}
}
