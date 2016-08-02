using Moq;
using PreMailer.Net.Downloaders;
using PreMailer.Net.Sources;
using System;
using AngleSharp.Parser.Html;
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

		private LinkTagCssSource CreateSUT(string baseUrl = "http://a.com", string path = "a.css")
		{
			var node = new HtmlParser().Parse(String.Format("<link href=\"{0}\" />", path));
			var sut = new LinkTagCssSource(node.Head.FirstElementChild, new Uri(baseUrl));

			return sut;
		}
	}
}
