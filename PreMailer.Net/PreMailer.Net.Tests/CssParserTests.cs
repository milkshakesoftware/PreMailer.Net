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
    }
}