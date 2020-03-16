using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class CssAttributeTests
	{
		[TestMethod]
		public void StandardUnimportantRule_ReturnsAttribute()
		{
			var attribute = CssAttribute.FromRule("color: red");

			Assert.AreEqual("color", attribute.Style);
			Assert.AreEqual("red", attribute.Value);
		}

		[TestMethod]
		public void MixedCaseRuleValue_RetainsCasing()
		{
			var attribute = CssAttribute.FromRule(" color: rED");

			Assert.AreEqual("color", attribute.Style);
			Assert.AreEqual("rED", attribute.Value);
		}

		[TestMethod]
		public void MixedCaseRule_RetainsCasing()
		{
			var attribute = CssAttribute.FromRule("Margin-bottom: 10px");

			Assert.AreEqual("Margin-bottom", attribute.Style);
			Assert.AreEqual("10px", attribute.Value);
		}

		[TestMethod]
		public void ImportantRule_ReturnsImportantAttribute()
		{
			var attribute = CssAttribute.FromRule("color: red !important");

			Assert.AreEqual("color", attribute.Style);
			Assert.AreEqual("red", attribute.Value);
			Assert.IsTrue(attribute.Important);
		}

		[TestMethod]
		public void ImportantRule_ReturnsValidCssWithoutWhitespaces()
		{
			var attribute = CssAttribute.FromRule("color:red!important");

			Assert.AreEqual("color", attribute.Style);
			Assert.AreEqual("red", attribute.Value);
			Assert.IsTrue(attribute.Important);
		}

		[TestMethod]
		public void NonRule_ReturnsNull()
		{
			var attribute = CssAttribute.FromRule(" ");

			Assert.IsNull(attribute);
		}

		[TestMethod]
		public void FromRule_OnlySplitsTheRuleAtTheFirstColonToSupportUrls()
		{
			var attribute = CssAttribute.FromRule("background: url('http://my.web.site.com/Content/email/content.png') repeat-y");

			Assert.AreEqual("url('http://my.web.site.com/Content/email/content.png') repeat-y", attribute.Value);
		}
	}
}
