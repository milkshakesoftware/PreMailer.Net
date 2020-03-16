using AngleSharp.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PreMailer.Net.Downloaders;
using PreMailer.Net.Sources;
using System;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class LinkTagCssSourceTests
	{
		private readonly Mock<IWebDownloader> _webDownloader = new Mock<IWebDownloader>();

		public LinkTagCssSourceTests()
		{
			WebDownloader.SharedDownloader = _webDownloader.Object;
		}

		[TestMethod]
		public void ImplementsInterface()
		{
			var sut = CreateSUT();

			Assert.IsInstanceOfType(sut, typeof(ICssSource));
		}

		[TestMethod]
		public void GetCSS_CallsWebDownloader_WithSpecifiedDomain()
		{
			var baseUrl = "http://a.co";

			var sut = CreateSUT(baseUrl: baseUrl);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.Scheme == "http" && u.Host == "a.co")));
		}

		[TestMethod]
		public void GetCSS_CallsWebDownloader_WithSpecifiedPath()
		{
			var path = "b.css";

			var sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.PathAndQuery == "/" + path)));
		}

		[TestMethod]
		public void GetCSS_CallsWebDownloader_WithSpecifiedBundle()
		{
			var path = "/Content/css?v=7V7TZzP9Wo7LiH9_q-r5mRBdC_N0lA_YJpRL_1V424E1";

			var sut = CreateSUT(path: path, link: "<link href=\"{0}\" rel=\"stylesheet\"/>");
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.PathAndQuery == path)));
		}

		[TestMethod]
		public void GetCSS_AbsoluteUrlInHref_CallsWebDownloader_WithSpecifiedPath()
		{
			var path = "http://b.co/a.css";

			var sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(new Uri(path)));
		}

		[TestMethod]
		public void GetCSS_DoesNotCallWebDownloader_WhenSchemeNotSupported()
		{
			var path = "chrome-extension://fcdjadjbdihbaodagojiomdljhjhjfho/css/atd.css";

			var sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(new Uri(path)), Times.Never);
		}

		private LinkTagCssSource CreateSUT(string baseUrl = "http://a.com", string path = "a.css", string link = "<link href=\"{0}\" />")
		{
			var node = new HtmlParser().ParseDocument(String.Format(link, path));
			var sut = new LinkTagCssSource(node.Head.FirstElementChild, new Uri(baseUrl));

			return sut;
		}
	}
}
