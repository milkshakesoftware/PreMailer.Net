﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using PreMailerDotNet;

namespace PreMailerDotNet.Tests
{
	[TestClass]
	public class PreMailerTests
	{

		[TestInitialize]
		public void TestInitialize()
		{
		}

		[TestMethod]
		public void MoveCssInline_RespectExistingStyleElement()
		{
			string input = "<html><head><style type=\"text/css\">.test { height: 100px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

			string premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"height: 100px;width: 100px;"));
		}

		[TestMethod]
		public void MoveCssInline_InlineStyleElementTakesPrecedence()
		{
			string input = "<html><head><style type=\"text/css\">.test { width: 150px; }</style></head><body><div class=\"test\" style=\"width: 100px;\">test</div></body></html>";

            string premailedOutput = PreMailer.MoveCssInline(input, false);

			Assert.IsTrue(premailedOutput.Contains("<div class=\"test\" style=\"width: 100px;"));
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

            string premailedOutput = PreMailer.MoveCssInline(htmlSource, false);
		}
	}
}