using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PreMailer.Parsing;

namespace PreMailer.Net.Tests.Parsing
{
	[TestClass]
	public class RuleSetParserTests
	{
		private Mock<ISelectorParser> _selectorParser;

		[TestInitialize]
		public void TestInitialize()
		{
			this._selectorParser = new Mock<ISelectorParser>();
			this._selectorParser.Setup(s => s.ParseSelector(It.IsAny<string>())).Returns(new Selector());
		}

		[TestMethod]
		public void ImplementsInterface()
		{
			var sut = new RuleSetParser();

			Assert.IsInstanceOfType(sut, typeof(IRuleSetParser));
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ParseRuleSet_GivenNullStyle_ThrowsArgumentNullException()
		{
			var sut = new RuleSetParser();

			sut.ParseRuleSet("a", null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void ParseRuleSet_GivenEmptyString_ThrowsArgumentException()
		{
			var sut = new RuleSetParser();

			sut.ParseRuleSet("a", String.Empty);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void ParseRuleSet_GivenWhiteSpace_ThrowsArgumentException()
		{
			var sut = new RuleSetParser();

			sut.ParseRuleSet("a", "   ");
		}

		[TestMethod]
		public void ParseRuleSet_ReturnsRuleSet()
		{
			var sut = new RuleSetParser();

			var result = sut.ParseRuleSet("a", "color: red;");

			Assert.IsInstanceOfType(result, typeof(RuleSet));
		}

		[TestMethod]
		public void ParseRuleSet_CallsSelectorParser()
		{
			string expected = "a";

			var sut = new RuleSetParser(this._selectorParser.Object);

			var result = sut.ParseRuleSet(expected, "color: red;");

			this._selectorParser.Verify(s => s.ParseSelector(expected));
		}

		[TestMethod]
		public void ParseRuleSet_RuleSetContainsSelectorFromParser()
		{
			var expected = new Selector();
			this._selectorParser.Setup(s => s.ParseSelector(It.IsAny<string>())).Returns(expected);

			var sut = new RuleSetParser(this._selectorParser.Object);

			var result = sut.ParseRuleSet("a", "color: red;");

			Assert.IsTrue(result.Selectors.Contains(expected));
		}

		[TestMethod]
		public void ParseRuleSet_GivenTwoSelectors_CallsSelectorParserTwice()
		{
			var sut = new RuleSetParser(this._selectorParser.Object);

			var result = sut.ParseRuleSet("a, div", "color: red;");

			this._selectorParser.Verify(s => s.ParseSelector(It.IsAny<string>()), Times.Exactly(2));
		}

		[TestMethod]
		public void ParseRuleSet_RuleSetSeparatesSelectorsByComma()
		{
			var expected = new Selector();
			this._selectorParser.Setup(s => s.ParseSelector("a")).Returns(new Selector());
			this._selectorParser.Setup(s => s.ParseSelector("div")).Returns(expected);

			var sut = new RuleSetParser(this._selectorParser.Object);

			var result = sut.ParseRuleSet("a,div", "color: red;");

			Assert.IsTrue(result.Selectors.Contains(expected));
		}

		[TestMethod]
		public void ParseRuleSet_ContainsStyleAttributeKey()
		{
			string expected = "color";

			var sut = new RuleSetParser(this._selectorParser.Object);

			var result = sut.ParseRuleSet("a", expected + ": red;");

			Assert.AreEqual(expected, result.Attributes.Keys[0]);
		}
	}
}