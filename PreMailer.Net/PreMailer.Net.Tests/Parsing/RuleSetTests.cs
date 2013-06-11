using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreMailer.Parsing;

namespace PreMailer.Tests.Parsing
{
	[TestClass]
	public class RuleSetTests
	{
		[TestMethod]
		public void Merge_AddsStyleAttributesFromGivenRuleSet()
		{
			RuleSet inlineStyle = new RuleSet();
			inlineStyle.Selectors.Add(new Selector { SelectorType = SelectorTypes.InlineStyle, Specificity = 1000 });

			RuleSet classStyle = new RuleSet();
			classStyle.Selectors.Add(new Selector { SelectorType = SelectorTypes.ClassName, Specificity = 10 });
			classStyle.Attributes.Add("color", "blue");

			inlineStyle.Merge(classStyle, classStyle.Selectors.First());

			Assert.AreEqual(classStyle.Attributes["color"], inlineStyle.Attributes["color"]);
		}

		[TestMethod]
		public void Merge_GivenRuleSet_WithLessSpecificity_KeepsOwnStyle()
		{
			RuleSet inlineStyle = new RuleSet();
			inlineStyle.Selectors.Add(new Selector { SelectorType = SelectorTypes.InlineStyle, Specificity = 1000 });
			inlineStyle.Attributes.Add("color", "blue");

			RuleSet classStyle = new RuleSet();
			classStyle.Selectors.Add(new Selector { SelectorType = SelectorTypes.ClassName, Specificity = 10 });
			classStyle.Attributes.Add("color", "red");

			inlineStyle.Merge(classStyle, classStyle.Selectors.First());

			Assert.AreEqual("blue", inlineStyle.Attributes["color"]);
		}

		[TestMethod]
		public void Merge_GivenRuleSet_WithMoreSpecificity_ReplacesOwnStyle()
		{
			RuleSet inlineStyle = new RuleSet();
			inlineStyle.Selectors.Add(new Selector { SelectorType = SelectorTypes.InlineStyle, Specificity = 1000 });
			inlineStyle.Attributes.Add("color", "blue");

			RuleSet classStyle = new RuleSet();
			classStyle.Selectors.Add(new Selector { SelectorType = SelectorTypes.ClassName, Specificity = 1001 });
			classStyle.Attributes.Add("color", "red");

			inlineStyle.Merge(classStyle, classStyle.Selectors.First());

			Assert.AreEqual("red", inlineStyle.Attributes["color"]);
		}
	}
}