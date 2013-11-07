using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssSelectorParserTests
    {
        private CssSelectorParser _parser;

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
        public void GetSelectorSpecificity_OneIdAndElementInPsuedoElement_Returns101()
        {
            var result = _parser.GetSelectorSpecificity("#s12:not(FOO)");
            Assert.AreEqual(101, result);
        }
    }
}