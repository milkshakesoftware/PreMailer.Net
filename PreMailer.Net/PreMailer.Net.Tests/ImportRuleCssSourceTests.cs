using Xunit;
using Moq;
using PreMailer.Net.Downloaders;
using PreMailer.Net.Sources;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

		[Fact]
		public void ItShould_LoadImportsRecursively_UntilLevelTwo()
		{
			// Arrange
			var baseUri = new Uri("https://a.com");
			var level0Css = "@import \"level1.css\";";
			var level1Css = "@import \"level2.css\";";
			var level2Css = "@import \"level3.css\";";
			var level3Css = "h1 { color: red; }";

			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/level1.css")))
				.Returns(level1Css);
			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/level2.css")))
				.Returns(level2Css);

			var sut = new ImportRuleCssSource();

			// Act
			var result = sut.GetCss(baseUri, level0Css).ToList();

			// Assert
			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/level1.css")), Times.Once);
			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/level2.css")), Times.Once);
			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/level3.css")), Times.Never);
			Assert.Equal(2, result.Count); // Should only contain level1.css and level2.css contents
		}

		[Fact]
		public void ItShould_CacheDownloadedImports_AndNotDownloadTwice()
		{
			// Arrange
			var baseUri = new Uri("https://a.com");
			var css = "@import \"shared.css\"; @import \"also-shared.css\";";
			var secondCss = "@import \"shared.css\";"; // References same file

			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/shared.css")))
				.Returns("h1 { color: red; }");
			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/also-shared.css")))
				.Returns("h2 { color: blue; }");

			var sut = new ImportRuleCssSource();

			// Act
			var firstResult = sut.GetCss(baseUri, css);
			var secondResult = sut.GetCss(baseUri, secondCss);

			// Assert
			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/shared.css")), Times.Once);
		}

		[Fact]
		public void ItShould_PreserveImportOrder_WhenProcessingImports()
		{
			// Arrange
			var baseUri = new Uri("https://a.com");
			var css = "@import \"first.css\"; @import \"second.css\"; @import \"third.css\";";
			var firstCss = "first { order: 1; }";
			var secondCss = "second { order: 2; }";
			var thirdCss = "third { order: 3; }";

			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/first.css")))
				.Returns(firstCss);
			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/second.css")))
				.Returns(secondCss);
			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/third.css")))
				.Returns(thirdCss);

			var sut = new ImportRuleCssSource();

			// Act
			var result = sut.GetCss(baseUri, css).ToList();

			// Assert
			Assert.Equal(3, result.Count);
			Assert.Equal(firstCss, result[0]);
			Assert.Equal(secondCss, result[1]);
			Assert.Equal(thirdCss, result[2]);
		}

		[Fact]
		public void ItShould_HandleCircularImports_WithoutInfiniteLoop()
		{
			// Arrange
			var baseUri = new Uri("https://a.com");
			var css = "@import \"circular1.css\";";
			var circular1Css = "@import \"circular2.css\";";
			var circular2Css = "@import \"circular1.css\";"; // Creates a circle

			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/circular1.css")))
				.Returns(circular1Css);
			_webDownloader
				.Setup(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/circular2.css")))
				.Returns(circular2Css);

			var sut = new ImportRuleCssSource();

			// Act
			var result = sut.GetCss(baseUri, css).ToList();

			// Assert
			Assert.Equal(2, result.Count); // Should contain both files exactly once
			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/circular1.css")), Times.Once);
			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.ToString() == "https://a.com/circular2.css")), Times.Once);
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
