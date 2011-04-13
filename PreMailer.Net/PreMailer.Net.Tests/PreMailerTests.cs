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
		[TestMethod]
		public void TestMethod1()
		{
			string testProjectDirectoryPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var directory = new DirectoryInfo(testProjectDirectoryPath);

			while (!directory.Name.Equals("PreMailer.Net.Tests"))
			{
				directory = directory.Parent;
			}

			testProjectDirectoryPath = directory.FullName;

			string htmlSource = File.ReadAllText(String.Join("\\", testProjectDirectoryPath, "testmail.html"));

			PreMailer pm = new PreMailer();

			string premailedOutput = pm.MoveCssInline(htmlSource, false);
		}
	}
}