using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class CssSelectorTests
	{
		[TestMethod]
		public void ContainsNotPsuedoClass_ElementWithPsuedoClass_ReturnsFalse()
		{
			var selector = new CssSelector("li:first-child");
			Assert.IsFalse(selector.HasNotPsuedoClass);
		}
		
		[TestMethod]
		public void ContainsNotPsuedoClass_ElementWithNotPsuedoClass_ReturnsTrue()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.IsTrue(selector.HasNotPsuedoClass);
		}
		
		[TestMethod]
		public void NotPsuedoClassContent_ElementWithPsuedoClass_ReturnsNull()
		{
			var selector = new CssSelector("li:first-child");
			Assert.IsNull(selector.NotPsuedoClassContent);
		}
		
		[TestMethod]
		public void NotPsuedoClassContent_ElementWithNotPsuedoClass_ReturnsContent()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.AreEqual(".ignored", selector.NotPsuedoClassContent);
		}
		
		[TestMethod]
		public void StripNotPsuedoClassContent_ElementWithPsuedoClass_ReturnsOriginalSelector()
		{
			var expected = "li:first-child";
			var selector = new CssSelector(expected);
			Assert.AreEqual(expected, selector.StripNotPsuedoClassContent().ToString());
		}
		
		[TestMethod]
		public void StripNotPsuedoClassContent_ElementWithNotPsuedoClass_ReturnsSelectorWithoutNot()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.AreEqual("li", selector.StripNotPsuedoClassContent().ToString());
		}
	}
}