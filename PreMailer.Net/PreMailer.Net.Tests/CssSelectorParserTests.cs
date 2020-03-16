using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class CssSelectorParserTests
	{
		private ICssSelectorParser _parser;

		[TestInitialize]
		public void TestInitialize()
		{
			_parser = new CssSelectorParser();
		}

		[TestMethod]
		public void GetSelectorSpecificity_Null_Returns0()
		{
			var result = _parser.GetSelectorSpecificity(null);
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_Empty_Returns0()
		{
			var result = _parser.GetSelectorSpecificity(string.Empty);
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_Wildcard_Returns0()
		{
			var result = _parser.GetSelectorSpecificity("*");
			Assert.AreEqual(0, result);
		}

		// Examples from http://www.w3.org/TR/2001/CR-css3-selectors-20011113/#specificity
		[TestMethod]
		public void GetSelectorSpecificity_SingleElementName_Returns1()
		{
			var result = _parser.GetSelectorSpecificity("LI");
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_TwoElementNames_Returns2()
		{
			var result = _parser.GetSelectorSpecificity("UL LI");
			Assert.AreEqual(2, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_ThreeElementNames_Returns3()
		{
			var result = _parser.GetSelectorSpecificity("UL OL+LI");
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_ElementNameAndAttribute_Returns11()
		{
			var result = _parser.GetSelectorSpecificity("H1 + *[REL=up]");
			Assert.AreEqual(11, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_AttributeVariants_AllEqual()
		{
			var result1 = _parser.GetSelectorSpecificity("[REL ~= \"up\"]");
			var result2 = _parser.GetSelectorSpecificity("[REL ~= 'up']");
			var result3 = _parser.GetSelectorSpecificity("[REL|=up]");
			var result4 = _parser.GetSelectorSpecificity("[ REL^=up ]");
			var result5 = _parser.GetSelectorSpecificity("[REL$=up]");
			var result6 = _parser.GetSelectorSpecificity("[REL*=up]");

			Assert.IsTrue(
				result1 == result2 &&
				result2 == result3 &&
				result3 == result4 &&
				result4 == result5 &&
				result5 == result6
			);
		}

		[TestMethod]
		public void GetSelectorSpecificity_ThreeElementNamesAndOneClass_Returns13()
		{
			var result = _parser.GetSelectorSpecificity("UL OL LI.red");
			Assert.AreEqual(13, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneElementNameAndTwoClasses_Returns21()
		{
			var result = _parser.GetSelectorSpecificity("LI.red.level");
			Assert.AreEqual(21, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneId_Returns100()
		{
			var result = _parser.GetSelectorSpecificity("#x34y");
			Assert.AreEqual(100, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdAndElementInPseudoElement_Returns101()
		{
			var result = _parser.GetSelectorSpecificity("#s12:after");
			Assert.AreEqual(101, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdAndElementInNotPseudoClass_Returns101()
		{
			var result = _parser.GetSelectorSpecificity("#s12:not(FOO)");
			Assert.AreEqual(101, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdAndElementWithHyphenInNotPseudoClass_Returns101()
		{
			var result = _parser.GetSelectorSpecificity("#s12:not(FOO-BAR)");
			Assert.AreEqual(101, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdTenClassesOneElement_Returns1101()
		{
			var result = _parser.GetSelectorSpecificity("#id .class .class .class .class .class .class .class .class .class .class element");
			Assert.AreEqual(1101, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_TenIdsOneClassOneElement_Returns1011()
		{
			var result = _parser.GetSelectorSpecificity("#id #id #id #id #id #id #id #id #id #id .class element");
			Assert.AreEqual(1011, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_ElementWithPseudoClass_Returns11()
		{
			var result = _parser.GetSelectorSpecificity("li:first-child");
			Assert.AreEqual(11, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdOneClassTenElements_Returns1110()
		{
			var result = _parser.GetSelectorSpecificity("#id .class element element element element element element element element element element");
			Assert.AreEqual(1110, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithHyphens_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("my-element#my-id.my-class");
			Assert.AreEqual(111, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithUnderscores_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("my_element#my_id.my_class");
			Assert.AreEqual(111, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithNonAsciiBeginnings_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("ɟmyelement#ʇmyid.ɹmyclass");
			Assert.AreEqual(111, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithNonAsciiMiddles_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("my™element#myǝid.myɐclass");
			Assert.AreEqual(111, result);
		}

		[TestMethod]
		public void GetSelectorSpecificity_OneIdOneClassOneElementWithNonAsciiEndings_Returns111()
		{
			var result = _parser.GetSelectorSpecificity("myelement♫#myid♫.myclass♫");
			Assert.AreEqual(111, result);
		}

		[TestMethod]
		public void IsPseudoClass_SelectorWithoutPseudoClass_ReturnsFalse()
		{
			var result = _parser.IsPseudoClass("a");
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsPseudoClass_SelectorWithPseudoClass_ReturnsTrue()
		{
			var result = _parser.IsPseudoClass("a:active");
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void IsPseudoElement_SelectorWithoutPseudoElement_ReturnsFalse()
		{
			var result = _parser.IsPseudoElement("p");
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsPseudoElement_SelectorWithPseudoElement_ReturnsTrue()
		{
			var result = _parser.IsPseudoElement("p:first-line");
			Assert.IsTrue(result);
		}
	}
}