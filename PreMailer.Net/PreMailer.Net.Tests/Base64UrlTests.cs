using System;
using Xunit;

namespace PreMailer.Net.Tests
{
    public class Base64UrlTests
    {
        [Fact]
        public void MoveCssInline_WithUnquotedBase64BackgroundImage_ShouldPreserveBase64Data()
        {
            string input = "<style>.pt-logo { background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA); background-repeat: no-repeat; }</style><span class=\"pt-logo\"></span>";
            
            var result = PreMailer.MoveCssInline(input);
            
            Assert.Contains("background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA)", result.Html);
            Assert.Contains("background-repeat: no-repeat", result.Html);
        }
        
        [Fact]
        public void MoveCssInline_WithQuotedBase64BackgroundImage_ShouldPreserveBase64Data()
        {
            string input = "<style>.pt-logo { background-image: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA\"); background-repeat: no-repeat; }</style><span class=\"pt-logo\"></span>";
            
            var result = PreMailer.MoveCssInline(input);
            
            Assert.Contains("background-image: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA\")", result.Html);
            Assert.Contains("background-repeat: no-repeat", result.Html);
        }
        
        [Fact]
        public void MoveCssInline_WithSvgXmlBackgroundImage_ShouldPreserveEncodedData()
        {
            string input = "<style>.my-icon { background-image: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='%23fff' width='8' height='8' viewBox='0 0 8 8'%3e%3cpath d='M5.25 0l-4 4 4 4 1.5-1.5L4.25 4l2.5-2.5L5.25 0z'/%3e%3c/svg%3e\"); }</style><span class=\"my-icon\"></span>";
            
            var result = PreMailer.MoveCssInline(input);
            
            Assert.Contains("background-image: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='%23fff' width='8' height='8' viewBox='0 0 8 8'%3e%3cpath d='M5.25 0l-4 4 4 4 1.5-1.5L4.25 4l2.5-2.5L5.25 0z'/%3e%3c/svg%3e\")", result.Html);
        }
        
        [Fact]
        public void MoveCssInline_WithBase64DataWithDoubleEquals_ShouldPreserveEnding()
        {
            string input = "<style>.pt-logo { background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA==); }</style><span class=\"pt-logo\"></span>";
            
            var result = PreMailer.MoveCssInline(input);
            
            Assert.Contains("background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA==)", result.Html);
        }
    }
}
