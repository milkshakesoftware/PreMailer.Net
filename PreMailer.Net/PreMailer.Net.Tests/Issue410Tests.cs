using AngleSharp.Html.Parser;
using Xunit;

namespace PreMailer.Net.Tests
{
    public class Issue410Tests
    {
        [Fact]
        public void MoveCssInline_PreservesImportantInInlineStyles()
        {
            string input = @"<style> 
.test {
 color:red;
 }
 </style>
<body>
<p class=""test"" style=""font-weight: bold !important;"">test</p>
</body>";

            var result = PreMailer.MoveCssInline(input);

            Assert.Contains("font-weight: bold !important", result.Html);
        }
    }
}
