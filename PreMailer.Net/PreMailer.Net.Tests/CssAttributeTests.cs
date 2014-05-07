using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssAttributeTests
    {
        [TestMethod]
        public void StandandUnimportantRule_ReturnsAttribute()
        {
            var attribute = CssAttribute.FromRule("color: red");

            Assert.AreEqual("color", attribute.Style);
            Assert.AreEqual("red", attribute.Value);
        }

        [TestMethod]
        public void MixedCaseRule_ReturnsLoweredAttribute()
        {
            var attribute = CssAttribute.FromRule(" color: rED");

            Assert.AreEqual("color", attribute.Style);
            Assert.AreEqual("red", attribute.Value);
        }

        [TestMethod]
        public void ImportantRule_ReturnsImportantAttribute()
        {
            var attribute = CssAttribute.FromRule("color: red !important");

            Assert.AreEqual("color", attribute.Style);
            Assert.AreEqual("red", attribute.Value);
            Assert.IsTrue(attribute.Important);
        }

        [TestMethod]
        public void NonRule_ReturnsNull()
        {
            var attribute = CssAttribute.FromRule(" ");

            Assert.IsNull(attribute);
        }
    }
}