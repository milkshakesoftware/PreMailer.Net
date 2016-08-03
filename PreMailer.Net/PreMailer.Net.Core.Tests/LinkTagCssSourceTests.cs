using AngleSharp.Parser.Html;
using Moq;
using PreMailer.Net.Downloaders;
using PreMailer.Net.Sources;
using System;
using Xunit;

namespace PreMailer.Net.Tests
{

	public class LinkTagCssSourceTests
	{
		private readonly Mock<IWebDownloader> _webDownloader = new Mock<IWebDownloader>();

		public LinkTagCssSourceTests()
		{
			WebDownloader.SharedDownloader = _webDownloader.Object;
		}

		[Fact]
		public void ImplementsInterface()
		{
			LinkTagCssSource sut = CreateSUT();

			ICssSource sut2 = sut as ICssSource;

			Assert.True(sut2 != null);
		}

		[Fact]
		public void GetCSS_CallsWebDownloader_WithSpecifiedDomain()
		{
			string baseUrl = "http://a.co";

			LinkTagCssSource sut = CreateSUT(baseUrl: baseUrl);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.Scheme == "http" && u.Host == "a.co")));
		}

		[Fact]
		public void GetCSS_CallsWebDownloader_WithSpecifiedBundle()
		{
			string path = "/Content/css?v=7V7TZzP9Wo7LiH9_q-r5mRBdC_N0lA_YJpRL_1V424E1";

			LinkTagCssSource sut = CreateSUT(path: path, link: "<link href=\"{0}\" rel=\"stylesheet\"/>");
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.PathAndQuery == path)));
		}

		[Fact]
		public void GetCSS_CallsWebDownloader_WithSpecifiedPath()
		{
			string path = "b.css";

			LinkTagCssSource sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.PathAndQuery == "/" + path)));
		}

		[Fact]
		public void GetCSS_AbsoluteUrlInHref_CallsWebDownloader_WithSpecifiedPath()
		{
			string path = "http://b.co/a.css";

			LinkTagCssSource sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(new Uri(path)));
		}

		[Fact]
		public void GetCSS_DoesNotCallWebDownloader_WhenSchemeNotSupported()
		{
			string path = "chrome-extension://fcdjadjbdihbaodagojiomdljhjhjfho/css/atd.css";

			LinkTagCssSource sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(new Uri(path)), Times.Never);
		}

		private LinkTagCssSource CreateSUT(string baseUrl = "http://a.com", string path = "a.css", string link = "<link href=\"{0}\" />")
		{
			var node = new HtmlParser().Parse(String.Format(link, path));
			var sut = new LinkTagCssSource(node.Head.FirstElementChild, new Uri(baseUrl));

			return sut;
		}
	}
}
