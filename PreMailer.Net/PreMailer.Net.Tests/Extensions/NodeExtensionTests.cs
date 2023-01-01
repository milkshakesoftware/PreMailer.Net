#nullable enable

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using PreMailer.Net.Extensions;
using Xunit;

namespace PreMailer.Net.Tests.Extensions
{
	public class NodeExtensionTests
	{
		private static readonly IHtmlDocument _document = new HtmlParser().ParseDocument("");

		private IElement Div() => _document.CreateElement("div");

		private IText Text(string data) => _document.CreateTextNode(data);

		[Fact]
		public void GetClosestTextNodeData_WithNoChild_GetsNull()
		{
			var target = Div();

			var data = target.GetFirstTextNodeData();

			Assert.Null(data);
		}

		[Fact]
		public void GetClosestTextNodeData_WithNoTextChild_GetsNull()
		{
			var target = Div();
			target.AppendChild(Div());

			var data = target.GetFirstTextNodeData();

			Assert.Null(data);
		}

		[Fact]
		public void GetClosestTextNodeData_WithSingleTextChild_GetsData()
		{
			var target = Div();
			target.AppendChild(Text("somedata"));

			var data = target.GetFirstTextNodeData();

			Assert.Equal("somedata", data);
		}

		[Fact]
		public void GetClosestTextNodeData_WithMixedChildren_GetsData()
		{
			var target = Div();
			target.AppendChild(Div());
			target.AppendChild(Text("somedata"));

			var data = target.GetFirstTextNodeData();

			Assert.Equal("somedata", data);
		}

		[Fact]
		public void GetClosestTextNodeData_WithMultipleTextChildren_GetsDataOfFirst()
		{
			var target = Div();
			target.AppendChild(Text("somedata1"));
			target.AppendChild(Text("somedata2"));

			var data = target.GetFirstTextNodeData();

			Assert.Equal("somedata1", data);
		}
	}
}
