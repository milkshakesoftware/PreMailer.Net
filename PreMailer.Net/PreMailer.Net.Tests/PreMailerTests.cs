using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class PreMailerTests
	{
		private PreMailer sut;

		[TestInitialize]
		public void TestInitialize()
		{
			this.sut = new PreMailer();
		}

		[TestMethod]
		public void MoveCssInline_RespectExistingStyleElement()
		{
			string input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"height: 100px;width: 100px;"));
		}

		[TestMethod]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = sut.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px;"));
		}

        [TestMethod]
        public void MoveCssInline_InlineStyleElementTakesPrecedence2()
        {
            string input = "<html><head><style type=\"text/css\">.test { width: 150px; } .my { color: red; } </style></head><body><div class=\"test\"><p class=\"my\">ff</p></div></body></html>";

            string premailedOutput = sut.MoveCssInline(input, false);

            Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 150px;\"><p class=\"my\" style=\"color=red;\""));
        }

        [TestMethod]
        public void MoveCssInline_InlineStyleElementTakesPrecedenceWinthInherit()
        {
            string input = "<style type=\"text/css\">.test { width: 150px; } .my { color: red; } .test { width: 200px; font:bold; } </style><div class=\"test\"><p class=\"my\" style=\"color: green;\"><label style=\"color:black\">TEST</label></p></div>";

            string premailedOutput = sut.MoveCssInlineWithInherit(input, false);

            Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"font: bold;width: 200px;\"><p class=\"my\" style=\"color: green;font: bold;width: 200px;\"><label style=\"color: black;font: bold;width: 200px;"));
        }

		[TestMethod, Ignore]
		public void ManualIntegrationTest()
		{
			string testProjectDirectoryPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var directory = new DirectoryInfo(testProjectDirectoryPath);

			while (!directory.Name.Equals("PreMailer.Net.Tests"))
			{
				directory = directory.Parent;
			}

			testProjectDirectoryPath = directory.FullName;

			string htmlSource = File.ReadAllText(String.Join("\\", testProjectDirectoryPath, "testmail.html"));

			string premailedOutput = sut.MoveCssInline(htmlSource, false);
		}
	}
}