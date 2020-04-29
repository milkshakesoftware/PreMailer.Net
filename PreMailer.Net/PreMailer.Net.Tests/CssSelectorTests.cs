using Xunit;

namespace PreMailer.Net.Tests
{
	public class CssSelectorTests
	{
		[Fact]
		public void ContainsNotPseudoClass_ElementWithPseudoClass_ReturnsFalse()
		{
			var selector = new CssSelector("li:first-child");
			Assert.False(selector.HasNotPseudoClass);
		}
		
		[Fact]
		public void ContainsNotPseudoClass_ElementWithNotPseudoClass_ReturnsTrue()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.True(selector.HasNotPseudoClass);
		}
		
		[Fact]
		public void NotPseudoClassContent_ElementWithPseudoClass_ReturnsNull()
		{
			var selector = new CssSelector("li:first-child");
			Assert.Null(selector.NotPseudoClassContent);
		}
		
		[Fact]
		public void NotPseudoClassContent_ElementWithNotPseudoClass_ReturnsContent()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.Equal(".ignored", selector.NotPseudoClassContent);
		}
		
		[Fact]
		public void StripNotPseudoClassContent_ElementWithPseudoClass_ReturnsOriginalSelector()
		{
			var expected = "li:first-child";
			var selector = new CssSelector(expected);
			Assert.Equal(expected, selector.StripNotPseudoClassContent().ToString());
		}
		
		[Fact]
		public void StripNotPseudoClassContent_ElementWithNotPseudoClass_ReturnsSelectorWithoutNot()
		{
			var selector = new CssSelector("li:not(.ignored)");
			Assert.Equal("li", selector.StripNotPseudoClassContent().ToString());
		}
	}
}