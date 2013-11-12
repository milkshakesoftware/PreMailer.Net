using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class CssSelectorTests
	{
		[TestMethod]
		public void ContainsNotPseudoClass_ElementWithPseudoClass_ReturnsFalse()
		{
			var selector = new CssSelector("li:first-child");
			Assert.IsFalse(selector.HasNotPseudoClass);
		}
		
		[TestMethod]
		public void ContainsNotPseudoClass_ElementWithNotPseudoClass_ReturnsTrue()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.IsTrue(selector.HasNotPseudoClass);
		}
		
		[TestMethod]
		public void NotPseudoClassContent_ElementWithPseudoClass_ReturnsNull()
		{
			var selector = new CssSelector("li:first-child");
			Assert.IsNull(selector.NotPseudoClassContent);
		}
		
		[TestMethod]
		public void NotPseudoClassContent_ElementWithNotPseudoClass_ReturnsContent()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.AreEqual(".ignored", selector.NotPseudoClassContent);
		}
		
		[TestMethod]
		public void StripNotPseudoClassContent_ElementWithPseudoClass_ReturnsOriginalSelector()
		{
			var expected = "li:first-child";
			var selector = new CssSelector(expected);
			Assert.AreEqual(expected, selector.StripNotPseudoClassContent().ToString());
		}
		
		[TestMethod]
		public void StripNotPseudoClassContent_ElementWithNotPseudoClass_ReturnsSelectorWithoutNot()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.AreEqual("li", selector.StripNotPseudoClassContent().ToString());
		}
	}
}