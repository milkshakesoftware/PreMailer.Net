using System;
using Xunit;

namespace PreMailer.Net.Tests
{
    public class PreserveEntitiesTests
    {
        [Fact]
        public void MoveCssInline_GivenCopyEntity_PreservesEntity()
        {
            string htmlEncoded = "&copy;";
            string input = $"<html><head></head><body><div>{htmlEncoded}</div></body></html>";
            
            var premailedOutput = PreMailer.MoveCssInline(input, preserveEntities: true);

            Assert.Contains(htmlEncoded, premailedOutput.Html);
            Assert.DoesNotContain("©", premailedOutput.Html);
        }
    }
}
