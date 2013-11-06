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
	    public void MoveCssInline_CssWithHigherSpecificity_AppliesMoreSpecificCss()
	    {
	        string input = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html><head><title>Y U NO WORK?</title></head><body><div id=""content""><style>.tblGenFixed td {background-color:#fff;}.tblGenFixed td.s0 {background-color:#ead1dc;}</style><table dir='ltr' border=0 cellpadding=0 cellspacing=0 class='tblGenFixed' id='tblMain'><tr dir='ltr'><td colspan=6 dir='ltr' class='s0'>Complete My AFF Course</tr></div></body></html>";

	        string premailedOutput = sut.MoveCssInline(input, false);

            Assert.IsTrue(premailedOutput.Contains("style=\"background-color: #ead1dc;\""));
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