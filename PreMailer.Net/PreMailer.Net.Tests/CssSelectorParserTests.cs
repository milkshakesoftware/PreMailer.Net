using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssSelectorParserTests
    {
        [TestMethod]
        public void Parse_Null_ReturnsZeroCounts()
        {
            var result = CssSelectorParser.Parse(null);
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(0, result.ElementNames);
            Assert.AreEqual(0, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_Empty_ReturnsZeroCounts()
        {
            var result = CssSelectorParser.Parse(string.Empty);
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(0, result.ElementNames);
            Assert.AreEqual(0, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_Wildcard_ReturnsZeroCounts()
        {
            var result = CssSelectorParser.Parse("*");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(0, result.ElementNames);
            Assert.AreEqual(0, result.Specificity);
        }

        // Examples from http://www.w3.org/TR/2001/CR-css3-selectors-20011113/#specificity
        [TestMethod]
        public void Parse_SingleElementName_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("LI");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(1, result.ElementNames);
            Assert.AreEqual(1, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_TwoElementNames_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("UL LI");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(2, result.ElementNames);
            Assert.AreEqual(2, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_ThreeElementNames_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("UL OL+LI");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(3, result.ElementNames);
            Assert.AreEqual(3, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_ElementNameAndAttribute_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("H1 + *[REL=up]");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(0, result.Classes);
            Assert.AreEqual(1, result.Attributes);
            Assert.AreEqual(1, result.ElementNames);
            Assert.AreEqual(11, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_ThreeElementNamesAndOneClass_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("UL OL LI.red");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(1, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(3, result.ElementNames);
            Assert.AreEqual(13, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_OneElementNameAndTwoClasses_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("LI.red.level");
            Assert.AreEqual(0, result.Ids);
            Assert.AreEqual(2, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(1, result.ElementNames);
            Assert.AreEqual(21, result.Specificity);
        }
        
        [TestMethod]
        public void Parse_OneId_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("#x34y");
            Assert.AreEqual(1, result.Ids);
            Assert.AreEqual(0, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(0, result.ElementNames);
            Assert.AreEqual(100, result.Specificity);
        }

        [TestMethod]
        public void Parse_OneIdAndElementInPsuedoElement_ReturnsExpectedResult()
        {
            var result = CssSelectorParser.Parse("#s12:not(FOO)");
            Assert.AreEqual(1, result.Ids);
            Assert.AreEqual(0, result.Classes);
            Assert.AreEqual(0, result.Attributes);
            Assert.AreEqual(1, result.ElementNames);
            Assert.AreEqual(101, result.Specificity);
        }
    }
}