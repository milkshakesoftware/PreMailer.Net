using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreMailer.Parsing;

namespace PreMailer.Net.Tests.Parsing
{
	[TestClass]
	public class SelectorParserTests
	{
		[TestMethod]
		public void ImplementsInterface()
		{
			var sut = new SelectorParser();

			Assert.IsInstanceOfType(sut, typeof(ISelectorParser));
		}

		[TestMethod]
		public void ParseSelector_ReturnsSelector()
		{
			var sut = new SelectorParser();

			var result = sut.ParseSelector(null);

			Assert.IsInstanceOfType(result, typeof(Selector));
		}

		[TestMethod]
		public void ParseSelector_SelectorContainsGivenName()
		{
			string expected = "a";
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector(expected);

			Assert.AreEqual(expected, result.SelectorName);
		}

		[TestMethod]
		public void ParseSelector_GivenNoSelector_SelectorTypeIsInline()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector(null);

			Assert.AreEqual(SelectorTypes.InlineStyle, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenIdSelector_SelectorTypeIsId()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("#saveButton");

			Assert.AreEqual(SelectorTypes.Id, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenClassNameSelector_SelectorTypeIsClassName()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector(".saveButton");

			Assert.AreEqual(SelectorTypes.ClassName, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenAttributeSelector_SelectorTypeIsAttribute()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("a[rel=delete]");

			Assert.AreEqual(SelectorTypes.Attribute, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenPseudoClassSelector_SelectorTypeIsPseudoClass()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("h2:first-child");

			Assert.AreEqual(SelectorTypes.PseudoClass, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenElementSelector_SelectorTypeIsElement()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("h2");

			Assert.AreEqual(SelectorTypes.Element, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenPseudoElementSelector_SelectorTypeIsPseudoElement()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("p::first-line");

			Assert.AreEqual(SelectorTypes.PseudoElement, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenIdWithClassNameSelector_SelectorTypeIsClassName()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("#saveButton.btn");

			Assert.AreEqual(SelectorTypes.ClassName, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenIdWithAttributeSelector_SelectorTypeIsAttribute()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("#saveButton[rel=button]");

			Assert.AreEqual(SelectorTypes.Attribute, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenIdWithAttributeSelectorContainingHashTag_SelectorTypeIsAttribute()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("#saveButton[href=#save]");

			Assert.AreEqual(SelectorTypes.Attribute, result.SelectorType);
		}

		[TestMethod]
		public void ParseSelector_GivenPseudoClassWithIdSelector_SelectorTypeIsId()
		{
			var sut = new SelectorParser();

			Selector result = sut.ParseSelector("h2:first-child#new");

			Assert.AreEqual(SelectorTypes.Id, result.SelectorType);
		}
	}
}