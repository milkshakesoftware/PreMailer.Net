using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PreMailer.Net {
    public class CssParser {
        private readonly List<string> _styleSheets;
        private SortedList<string, StyleClass> _scc;

        public SortedList<string, StyleClass> Styles {
            get { return _scc; }
            set { _scc = value; }
        }

        public CssParser() {
            _styleSheets = new List<string>();
            _scc = new SortedList<string, StyleClass>();
        }

        public void AddStyleSheet(string styleSheetContent) {
            _styleSheets.Add(styleSheetContent);
            ProcessStyleSheet(styleSheetContent);
        }

        public string GetStyleSheet(int index) {
            return _styleSheets[index];
        }

        public StyleClass ParseStyleClass(string className, string style) {
            var sc = new StyleClass { Name = className };

            FillStyleClass(sc, className, style);

            return sc;
        }

        private void ProcessStyleSheet(string styleSheetContent) {
            string content = CleanUp(styleSheetContent);
            string[] parts = content.Split('}');

            foreach (string s in parts) {
                if (CleanUp(s).IndexOf('{') > -1) {
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
					var cleaned = CleanUp(parts[0]).Trim();
					var styleNames = cleaned.Split(',').Select(x => x.Trim());

					foreach (var styleName in styleNames)
					{
						StyleClass sc;
						if (_scc.ContainsKey(styleName))
						{
							sc = _scc[styleName];
							_scc.Remove(styleName);
						}
						else
						{
							sc = new StyleClass();
						}

						FillStyleClass(sc, styleName, parts[1]);

						_scc.Add(sc.Name, sc);
					}
				}

        /// <summary>
        /// Fills the style class.
        /// </summary>
        /// <param name="sc">The style class.</param>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="style">The styles.</param>
        private void FillStyleClass(StyleClass sc, string styleName, string style) {
            sc.Name = styleName;

            string[] atrs = CleanUp(style).Split(';');

            foreach (string a in atrs) {
                if (!a.Contains(":"))
                    continue;

                string key = a.Split(':')[0].Trim();

                if (sc.Attributes.ContainsKey(key))
                    sc.Attributes.Remove(key);

                sc.Attributes.Add(key, a.Split(':')[1].Trim().ToLower());
            }
        }

        private string CleanUp(string s)
        {
            string temp = s;
            const string reg = "(/\\*(.|[\r\n])*?\\*/)|(//.*)";

            temp = Regex.Replace(temp, reg, "", RegexOptions.ExplicitCapture);
            temp = temp.Replace("\r", "").Replace("\n", "");

            return temp;
        }
    }
}