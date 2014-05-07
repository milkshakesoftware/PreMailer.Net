using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class CssParserTests
	{
		[TestMethod]
		public void AddStylesheet_ContainsAtCharsetRule_ShouldStripRuleAndParseStylesheet()
		{
			var stylesheet = "@charset utf-8; div { width: 100% }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.IsTrue(parser.Styles.ContainsKey("div"));
		}

		[TestMethod]
		public void AddStylesheet_ContainsAtPageSection_ShouldStripRuleAndParseStylesheet()
		{
			var stylesheet = "@page :first { margin: 2in 3in; } div { width: 100% }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.AreEqual(1, parser.Styles.Count);
			Assert.IsTrue(parser.Styles.ContainsKey("div"));
		}

		[TestMethod]
		public void AddStylesheet_ContainsUnsupportedMediaQuery_ShouldStrip()
		{
			var stylesheet = "@media print { div { width: 90%; } }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.AreEqual(0, parser.Styles.Count);
		}

		[TestMethod]
		public void AddStylesheet_ContainsUnsupportedMediaQueryAndNormalRules_ShouldStripMediaQueryAndParseRules()
		{
			var stylesheet = "div { width: 600px; } @media only screen and (max-width:620px) { div { width: 100% } } p { font-family: serif; }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.AreEqual(2, parser.Styles.Count);

			Assert.IsTrue(parser.Styles.ContainsKey("div"));
			Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);

			Assert.IsTrue(parser.Styles.ContainsKey("p"));
			Assert.AreEqual("serif", parser.Styles["p"].Attributes["font-family"].Value);
		}

		[TestMethod]
		public void AddStylesheet_ContainsSupportedMediaQuery_ShouldParseQueryRules()
		{
			var stylesheet = "@media only screen { div { width: 600px; } }";

			var parser = new CssParser();
			parser.AddStyleSheet(stylesheet);

			Assert.AreEqual(1, parser.Styles.Count);

			Assert.IsTrue(parser.Styles.ContainsKey("div"));
			Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
		}
	}
}