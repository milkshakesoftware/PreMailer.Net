using CsQuery;
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
			LinkTagCssSource sut = CreateSUT();

			Assert.IsInstanceOfType(sut, typeof(ICssSource));
		}

		[TestMethod]
		public void GetCSS_CallsWebDownloader_WithSpecifiedDomain()
		{
			string baseUrl = "http://a.co";

			LinkTagCssSource sut = CreateSUT(baseUrl: baseUrl);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.Scheme == "http" && u.Host == "a.co")));
		}

		[TestMethod]
		public void GetCSS_CallsWebDownloader_WithSpecifiedPath()
		{
			string path = "b.css";

			LinkTagCssSource sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(It.Is<Uri>(u => u.PathAndQuery == "/" + path)));
		}

		[TestMethod]
		public void GetCSS_AbsoluteUrlInHref_CallsWebDownloader_WithSpecifiedPath()
		{
			string path = "http://b.co/a.css";

			LinkTagCssSource sut = CreateSUT(path: path);
			sut.GetCss();

			_webDownloader.Verify(w => w.DownloadString(new Uri(path)));
		}

		private LinkTagCssSource CreateSUT(string baseUrl = "http://a.com", string path = "a.css")
		{
			var node = CQ.CreateFragment(String.Format("<link href=\"{0}\" />", path));
			var sut = new LinkTagCssSource(node.FirstElement(), new Uri(baseUrl));

			return sut;
		}
	}
}
