using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreMailer.Parsing;

namespace PreMailer.Net.Tests.Parsing
{
	/// <summary>
	/// Summary description for CssParserTests
	/// </summary>
	[TestClass]
	public class CssParserTests
	{
		[TestInitialize()]
		public void TestInitialize()
		{
		}

		[TestMethod]
		public void ImplementsInterface()
		{
			var sut = new CssParser();

			Assert.IsInstanceOfType(sut, typeof(ICssParser));
		}

		[TestMethod]
		public void Constructor_InstantiatesStylesList()
		{
			var sut = new CssParser();

			Assert.IsInstanceOfType(sut.Styles, typeof(SortedList<string, Selector>));
		}
	}
}