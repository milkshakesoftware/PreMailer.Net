using Xunit;
using Moq;
using PreMailer.Net.Downloaders;
using PreMailer.Net.Sources;
using System;
using System.Text;
using System.Collections.Generic;

namespace PreMailer.Net.Tests
{
	public class ImportRuleCssSourceTests
	{
		private readonly Mock<IWebDownloader> _webDownloader = new Mock<IWebDownloader>();

		public ImportRuleCssSourceTests()
		{
			WebDownloader.SharedDownloader = _webDownloader.Object;
		}

		[Fact]
		public void ItShould_DownloadAllImportedUrls_WhenCssContainsImportRules()
		{
			var baseUri = new Uri("https://a.com");
			var urls = new List<string>() { "variables.css?v234", "/fonts.css", "https://fonts.google.com/css/test-font" };

			var css = CreateCss(urls);
			var sut = new ImportRuleCssSource();

			sut.GetCss(baseUri, css);

			foreach (var url in urls)
			{
				_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.Equals(new Uri(baseUri, url)))));
			}
		}

		[Fact]
		public void ItShould_NotDownloadUrls_WhenLevelIsGreaterThanTwo()
		{
			var baseUri = new Uri("https://a.com");
			var urls = new List<string>() { "variables.css?v234", "/fonts.css", "https://fonts.google.com/css/test-font" };

			var css = CreateCss(urls);
			var sut = new ImportRuleCssSource();

			sut.GetCss(baseUri, css, 2);

			_webDownloader.Verify(w => w.DownloadString(It.IsAny<Uri>()), Times.Never());
		}

		[Fact]
		public void ItShould_NotDownloadUrls_WhenCssIsEmpty()
		{
			var baseUri = new Uri("https://a.com");
			var urls = new List<string>() { "variables.css?v234", "/fonts.css", "https://fonts.google.com/css/test-font" };

			var css = CreateCss(urls);
			var sut = new ImportRuleCssSource();

			sut.GetCss(baseUri, string.Empty);

			_webDownloader.Verify(w => w.DownloadString(It.IsAny<Uri>()), Times.Never());
		}

		[Fact]
		public void ItShould_NotDownloadUrls_WhenCssIsNull()
		{
			var baseUri = new Uri("https://a.com");
			var urls = new List<string>() { "variables.css?v234", "/fonts.css", "https://fonts.google.com/css/test-font" };

			var css = CreateCss(urls);
			var sut = new ImportRuleCssSource();

			sut.GetCss(baseUri, null);

			_webDownloader.Verify(w => w.DownloadString(It.IsAny<Uri>()), Times.Never());
		}

		[Fact]
		public void ItShould_NotDownloadUrls_WhenCssDoesNotContainImportRules()
		{
			var baseUri = new Uri("https://a.com");
			var urls = new List<string>() { "variables.css?v234", "/fonts.css", "https://fonts.google.com/css/test-font" };

			var css = string.Join(Environment.NewLine, urls);
			var sut = new ImportRuleCssSource();

			sut.GetCss(baseUri, css);

			_webDownloader.Verify(w => w.DownloadString(It.IsAny<Uri>()), Times.Never());
		}

		private string CreateCss(IEnumerable<string> imports)
		{
			var builder = new StringBuilder();
			foreach (var import in imports)
			{
				builder.AppendLine($"@import \"{import}\";");
			}

			return builder.ToString();
		}
	}
}
