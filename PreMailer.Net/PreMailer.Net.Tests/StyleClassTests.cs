using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class StyleClassTests
    {
        [TestMethod]
        public void SingleAttribute_ShouldNotAppendSemiColonToString()
        {
	        var clazz = new StyleClass
	        {
		        Attributes =
		        {
			        ["color"] = CssAttribute.FromRule("color: red")
		        }
	        };

	        Assert.AreEqual("color: red", clazz.ToString());
        }

        [TestMethod]
        public void MultipleAttributes_ShouldJoinAttributesWithSemiColonInString()
        {
	        var clazz = new StyleClass
	        {
		        Attributes =
		        {
			        ["color"] = CssAttribute.FromRule("color: red"),
			        ["height"] = CssAttribute.FromRule("height: 100%")
		        }
	        };

	        Assert.AreEqual("color: red;height: 100%", clazz.ToString());
        }

        [TestMethod]
        public void Merge_ShouldAddNonExistingAttributesToClass()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.IsTrue(target.Attributes.ContainsKey("height"));
            Assert.AreEqual("100%", target.Attributes["height"].Value);
        }

        [TestMethod]
        public void Merge_ShouldOverrideExistingEntriesIfSpecified()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50%");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.IsTrue(target.Attributes.ContainsKey("height"));
            Assert.AreEqual("100%", target.Attributes["height"].Value);
        }

        [TestMethod]
        public void Merge_ShouldNotOverrideExistingEntriesIfNotSpecified()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50%");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, false);

            Assert.IsTrue(target.Attributes.ContainsKey("height"));
            Assert.AreEqual("50%", target.Attributes["height"].Value);
        }

        [TestMethod]
        public void Merge_ShouldNotOverrideExistingImportantEntriesIfNewEntryIsNotImportant()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50% !important");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.IsTrue(target.Attributes.ContainsKey("height"));
            Assert.AreEqual("50%", target.Attributes["height"].Value);
        }

        [TestMethod]
        public void Merge_ShouldOverrideExistingImportantEntriesIfNewEntryIsImportant()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50% !important");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100% !important");

            target.Merge(donator, true);

            Assert.IsTrue(target.Attributes.ContainsKey("height"));
            Assert.AreEqual("100%", target.Attributes["height"].Value);
        }

        [TestMethod]
        public void Merge_ShouldOverrideExistingEntriesIfSpecifiedIgnoringCasing()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["HEight"] = CssAttribute.FromRule("height: 50%");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.IsTrue(target.Attributes.ContainsKey("height"));
            Assert.AreEqual("100%", target.Attributes["height"].Value);
        }
    }
}