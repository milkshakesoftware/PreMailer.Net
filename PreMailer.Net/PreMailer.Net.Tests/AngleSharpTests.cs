using AngleSharp;
using AngleSharp.Html.Parser;
using Xunit;

namespace PreMailer.Net.Tests;

public class AngleSharpTests
{
	[Fact]
	public void HtmlDocument_ToHtml_ShouldNotEffectHtmlEntities()
	{
		string htmlEncoded = "&lt;&amp;&gt;&nbsp;&copy;";
		string input = $"<html><head></head><body><div>{htmlEncoded}</div></body></html>";
		var document = new HtmlParser(new HtmlParserOptions()
		{
			IsNotConsumingCharacterReferences = true
		}).ParseDocument(input);
		var output = document.ToHtml();
		Assert.Equal<object>(input, output); // use object to get full string output
	}
}