using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PreMailer.Net
{
	public class CssParser
	{
		private readonly List<string> _styleSheets;
		private int styleCount = 0;

		public SortedList<string, StyleClass> Styles { get; set; }

		public CssParser()
		{
			_styleSheets = new List<string>();
			Styles = new SortedList<string, StyleClass>();
		}

		public void AddStyleSheet(string styleSheetContent)
		{
			_styleSheets.Add(styleSheetContent);
			ProcessStyleSheet(styleSheetContent);
		}

		public string GetStyleSheet(int index)
		{
			return _styleSheets[index];
		}

		public StyleClass ParseStyleClass(string className, string style)
		{
			var sc = new StyleClass { Name = className };

			FillStyleClass(sc, className, style);

			return sc;
		}

		private void ProcessStyleSheet(string styleSheetContent)
		{
			string content = CleanUp(styleSheetContent);
			string[] parts = content.Split('}');

			foreach (string s in parts)
			{
				if (s.IndexOf('{') > -1)
				{
					FillStyleClassFromBlock(s);
				}
			}
		}

		/// <summary>
		/// Fills the style class.
		/// </summary>
		/// <param name="s">The style block.</param>
		private void FillStyleClassFromBlock(string s)
		{
			string[] parts = s.Split('{');
			var cleaned = parts[0].Trim();
			var styleNames = cleaned.Split(',').Select(x => x.Trim());

			foreach (var styleName in styleNames)
			{
				StyleClass sc;
				if (Styles.ContainsKey(styleName))
				{
					sc = Styles[styleName];
					Styles.Remove(styleName);
				}
				else
				{
					sc = new StyleClass();
				}

				sc.Position = ++styleCount;

				FillStyleClass(sc, styleName, parts[1]);

				Styles.Add(sc.Name, sc);
			}
		}

		/// <summary>
		/// Fills the style class.
		/// </summary>
		/// <param name="sc">The style class.</param>
		/// <param name="styleName">Name of the style.</param>
		/// <param name="style">The styles.</param>
		private static void FillStyleClass(StyleClass sc, string styleName, string style)
		{
			sc.Name = styleName;

			//string[] atrs = style.Split(';');
			//string[] atrs = CleanUp(style).Split(';');
			string[] atrs = _fillStyleClassRegex.Split(CleanUp(style));

			foreach (string a in atrs)
			{
				var attribute = CssAttribute.FromRule(a);

				if (attribute != null) sc.Attributes.Merge(attribute);
			}
		}

		private static readonly Regex _fillStyleClassRegex = new Regex(@"(;)(?=(?:[^""']|""[^""]*""|'[^']*')*$)", RegexOptions.Multiline | RegexOptions.Compiled);
		private static readonly Regex _cssCommentRegex = new Regex(@"(?:/\*(.|[\r\n])*?\*/)|(?:(?<!url\s*\([^)]*)(?<!:)(?<!'[^']*?//)(?<!""[^""]*?//)//.*)", RegexOptions.Compiled);
		private static readonly Regex _unsupportedAtRuleRegex = new Regex(@"(?:@charset [^;]*;)|(?:@(page|font-face)[^{]*{[^}]*})|@import.+?;", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static string CleanUp(string s)
		{
			string protocolAgnosticMarker = "___PROTOCOL_AGNOSTIC_URL_" + Guid.NewGuid().ToString("N") + "___";
			string httpProtocolMarker = "___HTTP_PROTOCOL_" + Guid.NewGuid().ToString("N") + "___";
			string dataUrlMarker = "___DATA_URL_DOUBLE_SLASH_" + Guid.NewGuid().ToString("N") + "___";
			
			
			string temp = Regex.Replace(s, "(['\"])([^'\"\\\\]*(?:\\\\.[^'\"\\\\]*)*?)//([^'\"]*?)\\1", m => 
				m.Groups[1].Value + m.Groups[2].Value + protocolAgnosticMarker + m.Groups[3].Value + m.Groups[1].Value);
			
			temp = Regex.Replace(temp, @"url\s*\(\s*(['""]?)//([^)]*?)\\1\s*\)", m => 
				"url(" + m.Groups[1].Value + protocolAgnosticMarker + m.Groups[2].Value + m.Groups[1].Value + ")");
			
			temp = Regex.Replace(temp, @"url\s*\(\s*//([^)]*?)\s*\)", m => 
				"url(" + protocolAgnosticMarker + m.Groups[1].Value + ")");
			
			temp = Regex.Replace(temp, "(['\"])([^'\"\\\\]*(?:\\\\.[^'\"\\\\]*)*?)http://([^'\"]*?)\\1", m => 
				m.Groups[1].Value + m.Groups[2].Value + "http:" + httpProtocolMarker + m.Groups[3].Value + m.Groups[1].Value);
			
			temp = Regex.Replace(temp, "(data:[^;]+;base64,)([^)\"']*?)//([^)\"']*)", m => 
				m.Groups[1].Value + m.Groups[2].Value + dataUrlMarker + m.Groups[3].Value);
			
			temp = _cssCommentRegex.Replace(temp, "");
			temp = _unsupportedAtRuleRegex.Replace(temp, "");
			temp = CleanupMediaQueries(temp);
			temp = temp.Replace("\r", "").Replace("\n", "");
            temp = temp.Replace("<!--", "").Replace("-->", "");
			
			temp = temp.Replace(protocolAgnosticMarker, "//");
			temp = temp.Replace(httpProtocolMarker, "//");
			temp = temp.Replace(dataUrlMarker, "//");
			
			temp = Regex.Replace(temp, @"url\s*\(\s*(['""]?)data:([^;]+);base64,([^)'""]*)(['""]?)\s*\)", m => 
				"url(" + m.Groups[1].Value + "data:" + m.Groups[2].Value + ";base64," + m.Groups[3].Value + m.Groups[4].Value + ")");
			
			return temp;
		}

		public static readonly Regex _supportedMediaQueriesRegex = new Regex(@"^(?:\s*(?:only\s+)?(?:screen|projection|all),\s*)*(?:(?:only\s+)?(?:screen|projection|all))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex _mediaQueryRegex = new Regex(@"@media\s*(?<query>[^{]*){(?<styles>(?>[^{}]+|{(?<DEPTH>)|}(?<-DEPTH>))*(?(DEPTH)(?!)))}", RegexOptions.Compiled);

		private static string CleanupMediaQueries(string s)
		{
			return _mediaQueryRegex.Replace(s, m => _supportedMediaQueriesRegex.IsMatch(m.Groups["query"].Value.Trim()) ? m.Groups["styles"].Value.Trim() : string.Empty);
		}

		public static IEnumerable<string> GetUnsupportedMediaQueries(string s)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				yield break;
			}
			foreach (Match match in _mediaQueryRegex.Matches(s))
			{
				if (!_supportedMediaQueriesRegex.IsMatch(match.Value))
				{
					yield return match.Value;
				}
			}
		}
	}
}
