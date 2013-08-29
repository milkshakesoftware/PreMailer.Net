using System;
using ApprovalTests.Reporters;
using System.IO;
using NUnit.Framework;

namespace PreMailer.Net.Tests
{
	[TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class PreMailerTests
	{
		private PreMailer _sut;

		[SetUp]
		public void TestInitialize()
		{
			_sut = new PreMailer();
		}

		[Test]
		public void MoveCssInline_RespectExistingStyleElement()
		{
			string input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = _sut.MoveCssInline(input, false);

            Console.WriteLine(premailedOutput);
            Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px; height: 100px;"));
		}

		[Test]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = _sut.MoveCssInline(input, false);

            Console.WriteLine(premailedOutput);
			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px"));
		}

		[Test]
		public void ManualIntegrationTest()
		{
			string htmlSource = File.ReadAllText("testmail.html");

			string premailedOutput = _sut.MoveCssInline(htmlSource, false);
            Console.WriteLine(premailedOutput);

            ApprovalTests.Approvals.Verify(premailedOutput);
        }
	}
}