using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PreMailer.Parsing
{
	public class CssParser : ICssParser
	{
		private SortedList<string, Selector> _scc;

		public SortedList<string, Selector> Styles
		{
			get { return this._scc; }
			set { this._scc = value; }
		}

		public CssParser()
		{
			this._scc = new SortedList<string, Selector>();
		}

		public void LoadStyleSheet(string filePath)
		{
			throw new System.NotImplementedException();
		}

		public void AddStyleSheet(string styleSheetContent)
		{
			ProcessStyleSheet(styleSheetContent);
		}

		private void ProcessStyleSheet(string styleSheetContent)
		{
			string content = CleanUp(styleSheetContent);
			string[] parts = content.Split('}');

			foreach (string s in parts)
			{
				if (CleanUp(s).IndexOf('{') > -1)
				{
					FillStyleClass(s);
				}
			}
		}

		/// <summary>
		/// Fills the style class.
		/// </summary>
		/// <param name="s">The style block.</param>
		private void FillStyleClass(string s)
		{
			Selector sc = null;
			string[] parts = s.Split('{');
			string styleName = CleanUp(parts[0]).Trim();

			if (this._scc.ContainsKey(styleName))
			{
				sc = this._scc[styleName];
				this._scc.Remove(styleName);
			}
			else
			{
				sc = new Selector();
			}

			this.FillStyleClass(sc, styleName, parts[1]);

			//this._scc.Add(sc.Selector, sc);
		}

		/// <summary>
		/// Fills the style class.
		/// </summary>
		/// <param name="sc">The style class.</param>
		/// <param name="styleName">Name of the style.</param>
		/// <param name="style">The styles.</param>
		private void FillStyleClass(Selector sc, string styleName, string style)
		{
			/*sc.Selector = styleName;

			string[] atrs = CleanUp(style).Split(';');

			foreach (string a in atrs)
			{
				if (a.Contains(":"))
				{
					string _key = a.Split(':')[0].Trim();

					if (sc.Attributes.ContainsKey(_key))
					{
						sc.Attributes.Remove(_key);
					}

					sc.Attributes.Add(_key, a.Split(':')[1].Trim().ToLower());
				}
			}*/
		}

		private string CleanUp(string s)
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