using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PreMailer.Parsing
{
	public class CssParser : ICssParser
	{
		private readonly List<RuleSet> _ruleSets;

		private readonly IRuleSetParser _ruleSetParser;

		public CssParser()
		{
			this._ruleSets = new List<RuleSet>();
			this._ruleSetParser = new RuleSetParser();
		}

		public CssParser(IRuleSetParser ruleSetParser)
			: base()
		{
			if (ruleSetParser != null)
			{
				this._ruleSetParser = ruleSetParser;
			}
		}

		public void AddStyleSheet(string styleSheetContent)
		{
			ProcessStyleSheet(styleSheetContent);
		}

		public void AddStyleSheet(Uri url)
		{
			throw new System.NotImplementedException();
		}

		private void ProcessStyleSheet(string styleSheetContent)
		{
			string content = CleanUp(styleSheetContent);
			string[] parts = content.Split('}');

			foreach (string s in parts)
			{
				if (CleanUp(s).IndexOf('{') > -1)
				{
				}
			}
		}

		private static string CleanUp(string s)
		{
			string temp = s;
			string reg = "(/\\*(.|[\r\n])*?\\*/)|(//.*)";

			Regex r = new Regex(reg);
			temp = r.Replace(temp, "");
			temp = temp.Replace("\r", "").Replace("\n", "");

			return temp;
		}
	}
}