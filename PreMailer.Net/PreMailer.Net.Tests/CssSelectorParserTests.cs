using Xunit;

namespace PreMailer.Net.Tests
{
	public class CssSelectorParserTests
	{
		private ICssSelectorParser _parser;

		public CssSelectorParserTests()
		{
			_parser = new CssSelectorParser();
		}

		[Fact]
		public void GetSelectorSpecificity_Null_Returns0()
		{
			var result = _parser.GetSelectorSpecificity(null);
			Assert.Equal(0, result);
		}

		[Fact]
		public void GetSelectorSpecificity_Empty_Returns0()
		{
			var result = _parser.GetSelectorSpecificity(string.Empty);
			Assert.Equal(0, result);
		}

		[Fact]
		public void GetSelectorSpecificity_Wildcard_Returns0()
		{
			var result = _parser.GetSelectorSpecificity("*");
			Assert.Equal(0, result);
		}

		// Examples from http://www.w3.org/TR/2001/CR-css3-selectors-20011113/#specificity
		[Fact]
		public void GetSelectorSpecificity_SingleElementName_Returns1()
		{
			var result = _parser.GetSelectorSpecificity("LI");
			Assert.Equal(1, result);
		}

		[Fact]
		public void GetSelectorSpecificity_TwoElementNames_Returns2()
		{
			var result = _parser.GetSelectorSpecificity("UL LI");
			Assert.Equal(2, result);
		}

		[Fact]
		public void GetSelectorSpecificity_ThreeElementNames_Returns3()
		{
			var result = _parser.GetSelectorSpecificity("UL OL+LI");
			Assert.Equal(3, result);
		}

		[Fact]
		public void GetSelectorSpecificity_ElementNameAndAttribute_Returns11()
		{
			var result = _parser.GetSelectorSpecificity("H1 + *[REL=up]");
			Assert.Equal(11, result);
		}

		[Fact]
		public void GetSelectorSpecificity_AttributeVariants_AllEqual()
		{
			var result1 = _parser.GetSelectorSpecificity("[REL ~= \"up\"]");
			var result2 = _parser.GetSelectorSpecificity("[REL ~= 'up']");
			var result3 = _parser.GetSelectorSpecificity("[REL|=up]");
			var result4 = _parser.GetSelectorSpecificity("[ REL^=up ]");
			var result5 = _parser.GetSelectorSpecificity("[REL$=up]");
			var result6 = _parser.GetSelectorSpecificity("[REL*=up]");

			Assert.True(
				result1 == result2 &&
				result2 == result3 &&
				result3 == result4 &&
				result4 == result5 &&
				result5 == result6
			);
		}

		[Fact]
		public void GetSelectorSpecificity_ThreeElementNamesAndOneClass_Returns13()
		{
			var result = _parser.GetSelectorSpecificity("UL OL LI.red");
			Assert.Equal(13, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneElementNameAndTwoClasses_Returns21()
		{
			var result = _parser.GetSelectorSpecificity("LI.red.level");
			Assert.Equal(21, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneId_Returns100()
		{
			var result = _parser.GetSelectorSpecificity("#x34y");
			Assert.Equal(100, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdAndElementInPseudoElement_Returns101()
		{
			var result = _parser.GetSelectorSpecificity("#s12:after");
			Assert.Equal(101, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdAndElementInNotPseudoClass_Returns101()
		{
			var result = _parser.GetSelectorSpecificity("#s12:not(FOO)");
			Assert.Equal(101, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdAndElementWithHyphenInNotPseudoClass_Returns101()
		{
			var result = _parser.GetSelectorSpecificity("#s12:not(FOO-BAR)");
			Assert.Equal(101, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdTenClassesOneElement_Returns1101()
		{
			var result = _parser.GetSelectorSpecificity("#id .class .class .class .class .class .class .class .class .class .class element");
			Assert.Equal(1101, result);
		}

		[Fact]
		public void GetSelectorSpecificity_TenIdsOneClassOneElement_Returns1011()
		{
			var result = _parser.GetSelectorSpecificity("#id #id #id #id #id #id #id #id #id #id .class element");
			Assert.Equal(1011, result);
		}

		[Fact]
		public void GetSelectorSpecificity_ElementWithPseudoClass_Returns11()
		{
			var result = _parser.GetSelectorSpecificity("li:first-child");
			Assert.Equal(11, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdOneClassTenElements_Returns1110()
		{
			var result = _parser.GetSelectorSpecificity("#id .class element element element element element element element element element element");
			Assert.Equal(1110, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithHyphens_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("my-element#my-id.my-class");
			Assert.Equal(111, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithUnderscores_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("my_element#my_id.my_class");
			Assert.Equal(111, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithNonAsciiBeginnings_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("ɟmyelement#ʇmyid.ɹmyclass");
			Assert.Equal(111, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithNonAsciiMiddles_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("my™element#myǝid.myɐclass");
			Assert.Equal(111, result);
		}

		[Fact]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithNonAsciiEndings_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("myelement♫#myid♫.myclass♫");
			Assert.Equal(111, result);
		}

		[Fact]
		public void IsPseudoClass_SelectorWithoutPseudoClass_ReturnsFalse()
		{
			var result = _parser.IsPseudoClass("a");
			Assert.False(result);
		}

		[Fact]
		public void IsPseudoClass_SelectorWithPseudoClass_ReturnsTrue()
		{
			var result = _parser.IsPseudoClass("a:active");
			Assert.True(result);
		}

		[Fact]
		public void IsPseudoElement_SelectorWithoutPseudoElement_ReturnsFalse()
		{
			var result = _parser.IsPseudoElement("p");
			Assert.False(result);
		}

		[Fact]
		public void IsPseudoElement_SelectorWithPseudoElement_ReturnsTrue()
		{
			var result = _parser.IsPseudoElement("p:first-line");
			Assert.True(result);
		}
	}
}