using Xunit;

namespace PreMailer.Net.Tests
{
    public class StyleClassTests
    {
        [Fact]
        public void SingleAttribute_ShouldNotAppendSemiColonToString()
        {
            var clazz = new StyleClass();
            clazz.Attributes["color"] = CssAttribute.FromRule("color: red");

            Assert.Equal("color: red", clazz.ToString());
        }

        [Fact]
        public void MultipleAttributes_ShouldJoinAttributesWithSemiColonInString()
        {
            var clazz = new StyleClass();
            clazz.Attributes["color"] = CssAttribute.FromRule("color: red");
            clazz.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            Assert.Equal("color: red;height: 100%", clazz.ToString());
        }

        [Fact]
        public void Merge_ShouldAddNonExistingAttributesToClass()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.True(target.Attributes.ContainsKey("height"));
            Assert.Equal("100%", target.Attributes["height"].Value);
        }

        [Fact]
        public void Merge_ShouldOverrideExistingEntriesIfSpecified()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50%");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.True(target.Attributes.ContainsKey("height"));
            Assert.Equal("100%", target.Attributes["height"].Value);
        }

        [Fact]
        public void Merge_ShouldNotOverrideExistingEntriesIfNotSpecified()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50%");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, false);

            Assert.True(target.Attributes.ContainsKey("height"));
            Assert.Equal("50%", target.Attributes["height"].Value);
        }

        [Fact]
        public void Merge_ShouldNotOverrideExistingImportantEntriesIfNewEntryIsNotImportant()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50% !important");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.True(target.Attributes.ContainsKey("height"));
            Assert.Equal("50%", target.Attributes["height"].Value);
        }

        [Fact]
        public void Merge_ShouldOverrideExistingImportantEntriesIfNewEntryIsImportant()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["height"] = CssAttribute.FromRule("height: 50% !important");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100% !important");

            target.Merge(donator, true);

            Assert.True(target.Attributes.ContainsKey("height"));
            Assert.Equal("100%", target.Attributes["height"].Value);
        }

        [Fact]
        public void Merge_ShouldOverrideExistingEntriesIfSpecifiedIgnoringCasing()
        {
            var target = new StyleClass();
            var donator = new StyleClass();

            target.Attributes["color"] = CssAttribute.FromRule("color: red");
            target.Attributes["HEight"] = CssAttribute.FromRule("height: 50%");
            donator.Attributes["height"] = CssAttribute.FromRule("height: 100%");

            target.Merge(donator, true);

            Assert.True(target.Attributes.ContainsKey("height"));
            Assert.Equal("100%", target.Attributes["height"].Value);
        }

        [Fact]
        public void Merge_ShouldEmitImportantOnlyWhenSpecified()
        {
			var clazz = new StyleClass();
			clazz.Attributes["color"] = CssAttribute.FromRule("color: red !important");

			Assert.Equal("color: red", clazz.ToString());
			Assert.Equal("color: red", clazz.ToString(emitImportant: false));
			Assert.Equal("color: red !important", clazz.ToString(emitImportant: true));
		}
	}
}