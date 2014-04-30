using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreMailer.Net {
    public class CssAttribute {
        public string Style { get; set; }
        public string Value { get; set; }
        public bool Important { get; set; }

        private CssAttribute() { }

        public static CssAttribute FromRule(string rule) {            
            var parts = rule.Split(':');

            if (parts.Length == 1) return null;

            var value = parts[1].Trim().ToLower();
            var important = false;

            if(value.Contains("!important")) {
                important = true;
                value = value.Replace(" !important", "");
            }

            return new CssAttribute {
                Style = parts[0].Trim().ToLower(),
                Value = value,
                Important = important
            };
        }

        public override string ToString() {
            return string.Format("{0}: {1}{2}", Style, Value, Important ? " !important" : string.Empty);
        }
    }
}
