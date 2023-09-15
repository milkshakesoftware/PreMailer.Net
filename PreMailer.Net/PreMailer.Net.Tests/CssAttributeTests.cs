﻿using Xunit;

namespace PreMailer.Net.Tests
{
	public class CssAttributeTests
	{
		[Fact]
		public void StandardUnimportantRule_ReturnsAttribute()
		{
			var attribute = CssAttribute.FromRule("color: red");

			Assert.Equal("color", attribute.Style);
			Assert.Equal("red", attribute.Value);
		}

		[Fact]
		public void MixedCaseRuleValue_RetainsCasing()
		{
			var attribute = CssAttribute.FromRule(" color: rED");

			Assert.Equal("color", attribute.Style);
			Assert.Equal("rED", attribute.Value);
		}

		[Fact]
		public void MixedCaseRule_RetainsCasing()
		{
			var attribute = CssAttribute.FromRule("Margin-bottom: 10px");

			Assert.Equal("Margin-bottom", attribute.Style);
			Assert.Equal("10px", attribute.Value);
		}

		[Fact]
		public void ImportantRule_ReturnsImportantAttribute()
		{
			var attribute = CssAttribute.FromRule("color: red !important");

			Assert.Equal("color", attribute.Style);
			Assert.Equal("red", attribute.Value);
			Assert.True(attribute.Important);
		}

		[Fact]
		public void ImportantRule_EmitsImportantAttributeOnlyWhenSpecified()
		{
			var attribute = CssAttribute.FromRule("color: red !important");

			Assert.Equal("color: red", attribute.ToString());
			Assert.Equal("color: red", attribute.ToString(emitImportant: false));
			Assert.Equal("color: red !important", attribute.ToString(emitImportant: true));
		}

		[Fact]
		public void ImportantRule_ReturnsValidCssWithoutWhitespaces()
		{
			var attribute = CssAttribute.FromRule("color:red!important");

			Assert.Equal("color", attribute.Style);
			Assert.Equal("red", attribute.Value);
			Assert.True(attribute.Important);
		}

		[Fact]
		public void NonRule_ReturnsNull()
		{
			var attribute = CssAttribute.FromRule(" ");

			Assert.Null(attribute);
		}

		[Fact]
		public void FromRule_OnlySplitsTheRuleAtTheFirstColonToSupportUrls()
		{
			var attribute = CssAttribute.FromRule("background: url('http://my.web.site.com/Content/email/content.png') repeat-y");

			Assert.Equal("url('http://my.web.site.com/Content/email/content.png') repeat-y", attribute.Value);
		}
	}
}
