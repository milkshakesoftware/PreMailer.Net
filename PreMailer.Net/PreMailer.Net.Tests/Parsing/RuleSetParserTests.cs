using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreMailer.Parsing;

namespace PreMailer.Net.Tests.Parsing
{
	[TestClass]
	public class RuleSetParserTests
	{
		[TestInitialize]
		public void TestInitialize()
		{
		}

		[TestMethod]
		public void ImplementsInterface()
		{
			var sut = new RuleSetParser();

			Assert.IsInstanceOfType(sut, typeof(IRuleSetParser));
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ParseStyleClass_GivenNullStyle_ThrowsArgumentNullException()
		{
			var sut = new RuleSetParser();

			sut.ParseRuleSet("a", null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void ParseStyleClass_GivenEmptyString_ThrowsArgumentException()
		{
			var sut = new RuleSetParser();

			sut.ParseRuleSet("a", String.Empty);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void ParseStyleClass_GivenWhiteSpace_ThrowsArgumentException()
		{
			var sut = new RuleSetParser();

			sut.ParseRuleSet("a", "   ");
		}

		[TestMethod]
		public void ParseStyleClass_ReturnsRuleSet()
		{
			var sut = new RuleSetParser();

			var result = sut.ParseRuleSet("a", "color: red;");

			Assert.IsInstanceOfType(result, typeof(RuleSet));
		}
	}
}