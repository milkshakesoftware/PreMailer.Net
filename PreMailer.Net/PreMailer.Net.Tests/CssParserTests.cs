using System.Linq;
using Xunit;

namespace PreMailer.Net.Tests
{
	public class CssParserTests
	{
		[Fact]
		public void AddStylesheet_ContainsAtCharsetRule_ShouldStripRuleAndParseStylesheet()
		{
			var stylesheet = "@charset utf-8; div { width: 100% }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.True(parser.Styles.ContainsKey("div"));
		}

		[Fact]
		public void AddStylesheet_ContainsAtPageSection_ShouldStripRuleAndParseStylesheet()
		{
			var stylesheet = "@page :first { margin: 2in 3in; } div { width: 100% }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.Single(parser.Styles);
			Assert.True(parser.Styles.ContainsKey("div"));
		}

		[Fact]
		public void AddStylesheet_ContainsUnsupportedMediaQuery_ShouldStrip()
		{
			var stylesheet = "@media print { div { width: 90%; } }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.Empty(parser.Styles);
		}

		[Fact]
		public void AddStylesheet_ContainsUnsupportedMediaQueryAndNormalRules_ShouldStripMediaQueryAndParseRules()
		{
			var stylesheet = "div { width: 600px; } @media only screen and (max-width:620px) { div { width: 100% } } p { font-family: serif; }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.Equal(2, parser.Styles.Count);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);

			Assert.True(parser.Styles.ContainsKey("p"));
			Assert.Equal("serif", parser.Styles["p"].Attributes["font-family"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsSupportedMediaQuery_ShouldParseQueryRules()
		{
			var stylesheet = "@media only screen { div { width: 600px; } }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsImportStatement_ShouldStripOutImportStatement()
		{
			var stylesheet = "@import url(http://google.com/stylesheet); div { width : 600px; }";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}


		[Fact]
		public void AddStylesheet_ContainsImportStatementTest_ShouldStripOutImportStatement()
		{
			var stylesheet = "@import 'stylesheet.css'; div { width : 600px; }";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsMinifiedImportStatement_ShouldStripOutImportStatement()
		{
			var stylesheet = "@import url(http://google.com/stylesheet);div{width:600px;}";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsMultipleImportStatement_ShouldStripOutImportStatements()
		{
			var stylesheet = "@import url(http://google.com/stylesheet); @import url(http://jquery.com/stylesheet1); @import url(http://amazon.com/stylesheet2); div { width : 600px; }";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsImportStatementWithMediaQuery_ShouldStripOutImportStatements()
		{
			var stylesheet = "@import url(http://google.com/stylesheet) mobile; div { width : 600px; }";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsMultipleImportStatementWithMediaQueries_ShouldStripOutImportStatements()
		{
			var stylesheet = "@import url(http://google.com/stylesheet) mobile; @import url(http://google.com/stylesheet) mobile; @import url(http://google.com/stylesheet) mobile; div { width : 600px; }";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			Assert.Single(parser.Styles);

			Assert.True(parser.Styles.ContainsKey("div"));
			Assert.Equal("600px", parser.Styles["div"].Attributes["width"].Value);
		}

		[Fact]
		public void AddStylesheet_ContainsEncodedImage()
		{
			var stylesheet = @"#logo
{
	content: url('data:image/jpeg; base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=');
	max-width: 200px;
	height: auto;
}";
			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);
			var attributes = parser.Styles["#logo"].Attributes;
		}

		[Fact]
		public void AddStylesheet_ShouldSetStyleClassPositions()
		{
			var stylesheet1 = "#id .class1 element { color: #fff; } #id .class2 element { color: #aaa; }";
			var stylesheet2 = "#id .class3 element { color: #000; } #id .class2 element { color: #bbb; }";
			var parser = new CssParser();

			parser.AddStyleSheet(stylesheet1);
			parser.AddStyleSheet(stylesheet2);

			Assert.Equal(1, parser.Styles.Values[0].Position);
			Assert.Equal(4, parser.Styles.Values[1].Position);
			Assert.Equal(3, parser.Styles.Values[2].Position);
        }

		[Fact]
		public void AddStylesheet_ShouldKeepTheRightOrderOfCssAttributes()
		{
			var stylesheet1 = @"
				.my-div { background-color: blue; }
				.my-div { background: red; }
				.my-div { background-color: green; }
			";
			var parser = new CssParser();

			parser.AddStyleSheet(stylesheet1);

			Assert.Single(parser.Styles);

			var attributes = parser.Styles.First().Value.Attributes.ToArray();

			Assert.True(attributes[0] is {Style: "background", Value: "red"});
			Assert.True(attributes[1] is {Style: "background-color", Value: "green" });
		}

        [Fact]
        public void AddStylesheet_ContainsSingleQuotes_ShouldParseStylesheet()
        {
            var stylesheet = "a { color: #fff; font-family: 'Segoe UI', Verdana, Arial, sans-serif; text-decoration: underline; }";
            var parser = new CssParser();

            parser.AddStyleSheet(stylesheet);

            Assert.NotNull(parser.Styles["a"]);
            Assert.NotNull(parser.Styles["a"].Attributes["font-family"]);
            Assert.Equal(3, parser.Styles["a"].Attributes.Count);
        }

        [Fact]
        public void AddStylesheet_ContainsDoubleQuotes_ShouldParseStylesheet()
        {
            var stylesheet = "a { color: #fff; font-family: \"Segoe UI\", Verdana, Arial, sans-serif; text-decoration: underline; }";
            var parser = new CssParser();

            parser.AddStyleSheet(stylesheet);

            Assert.NotNull(parser.Styles["a"]);
            Assert.NotNull(parser.Styles["a"].Attributes["font-family"]);
            Assert.Equal(3, parser.Styles["a"].Attributes.Count);
        }

        [Fact]
        public void AddStylesheet_ContainsPremailerSrcWithProtocolAgnosticUrl_ShouldParseCorrectly()
        {
            var stylesheet = "img.logo { -premailer-src: \"//example.com/logo.png\"; width: 100px; height: 80px; }";
            var parser = new CssParser();

            parser.AddStyleSheet(stylesheet);

            Assert.NotNull(parser.Styles["img.logo"]);
            Assert.NotNull(parser.Styles["img.logo"].Attributes["-premailer-src"]);
            Assert.Equal("\"//example.com/logo.png\"", parser.Styles["img.logo"].Attributes["-premailer-src"].Value);
            Assert.Equal(3, parser.Styles["img.logo"].Attributes.Count);
        }

        [Fact]
        public void AddStylesheet_ContainsMultiplePremailerAttributes_ShouldParseCorrectly()
        {
            var stylesheet = "img.logo { -premailer-src: \"https://example.com/logo.png\"; -premailer-height: 80px; -premailer-width: 100px; width: 100px; height: 80px; }";
            var parser = new CssParser();

            parser.AddStyleSheet(stylesheet);

            Assert.NotNull(parser.Styles["img.logo"]);
            Assert.NotNull(parser.Styles["img.logo"].Attributes["-premailer-src"]);
            Assert.Equal("\"https://example.com/logo.png\"", parser.Styles["img.logo"].Attributes["-premailer-src"].Value);
            Assert.Equal("80px", parser.Styles["img.logo"].Attributes["-premailer-height"].Value);
            Assert.Equal("100px", parser.Styles["img.logo"].Attributes["-premailer-width"].Value);
            Assert.Equal(5, parser.Styles["img.logo"].Attributes.Count);
        }
    }
}
