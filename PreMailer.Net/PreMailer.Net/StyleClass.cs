using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CsQuery.ExtensionMethods;

namespace PreMailer.Net {
    public class StyleClass {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleClass"/> class.
        /// </summary>
        public StyleClass() {
            Attributes = new SortedList<string, string>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public SortedList<string, string> Attributes { get; set; }

        /// <summary>
        /// Merges the specified style class, with this instance. Styles on this instance are not overriden by duplicates in the specified styleClass.
        /// </summary>
        /// <param name="styleClass">The style class.</param>
        /// <param name="canOverwrite">if set to <c>true</c> [can overwrite].</param>
        public void Merge(StyleClass styleClass, bool canOverwrite) {
            ExpandShorthand();
            styleClass.ExpandShorthand();

            foreach (var item in styleClass.Attributes) {
                if (!Attributes.ContainsKey(item.Key))
                    Attributes.Add(item.Key, item.Value);
                else if (canOverwrite)
                    Attributes[item.Key] = item.Value;
            }

            CollapseToShorthand();
            styleClass.CollapseToShorthand();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            foreach (var item in Attributes)
                sb.AppendFormat("{0}: {1};", item.Key, item.Value);

            return sb.ToString();
        }

        public void ExpandShorthand() {
            ExpandShorthandBorder();
            ExpandShorthandDimensions();
            ExpandShorthandFont();
            ExpandShorthandBackground();
            ExpandShorthandListStyle();
        }

        public void CollapseToShorthand() {
            CollapseBorderToShorthand();
            CollapseDimensionsToShorthand();
            CollapseFontToShorthand();
            CollapseBackgroundToShorthand();
            CollapseListStyleToShorthand();
        }

        private readonly string[] _backgroundProperties = { "background-color", "background-image", "background-repeat", "background-position", "background-attachment" };

        private void ExpandShorthandBackground() {
            //if (!Attributes.ContainsKey("background"))
            //    return;

            //string value = Attributes["background"];

            //if (value.Trim().Equals("inherits", StringComparison.OrdinalIgnoreCase))
            //    _backgroundProperties.ForEach(prop => SplitDeclaration("background", prop, "inherit"));

            //SplitDeclaration("background", "background-image", CssRegex.URI_RX + CssRegex.RE_GRADIENT);
            //SplitDeclaration("background", "background-attachment", CssRegex.RE_SCROLL_FIXED);
            //SplitDeclaration("background", "background-repeat", CssRegex.RE_REPEAT);
            //SplitDeclaration("background", "background-color", CssRegex.RE_COLOUR);
            //SplitDeclaration("background", "background-position", CssRegex.RE_BACKGROUND_POSITION);

            //Attributes.Remove("background");
        }

        private void CollapseBackgroundToShorthand() {
        }

        private void ExpandShorthandBorder() {
            //var attrs = new[] { "border", "border-left", "border-right", "border-top", "border-bottom" };

            //foreach (var attr in attrs) {
            //    if (!Attributes.ContainsKey(attr))
            //        continue;

            //    string value = Attributes[attr];

            //    SplitDeclaration(attr, attr + "-width", CssRegex.RE_BORDER_UNITS);
            //    SplitDeclaration(attr, attr + "-color", CssRegex.RE_COLOUR);
            //    SplitDeclaration(attr, attr + "-style", CssRegex.RE_BORDER_STYLE);

            //    Attributes.Remove(attr);
            //}
        }

        private void CollapseBorderToShorthand() {
        }

        private static readonly Dictionary<string, string> _dimensionsMap = new Dictionary<string, string> {
                { "margin", "margin-{0}" },
                { "padding", "padding-{0}" },
                { "border-color", "border-{0}-color" },
                { "border-style", "border-{0}-style" },
                { "border-width", "border-{0}-width" }
            };

        private void ExpandShorthandDimensions() {
            foreach (var map in _dimensionsMap) {
                if (!Attributes.ContainsKey(map.Key))
                    continue;

                var value = Attributes[map.Key];

                value = Regex.Replace(value, CssRegex.RE_COLOUR, m => Regex.Replace(m.Value, @"[\s]+", ""));

                var matches = Regex.Split(value.Trim(), @"[\s]+");

                string top = null, right = null, bottom = null, left = null;

                switch (matches.Length) {
                    case 1:
                        top = right = bottom = left = matches[0];
                        break;
                    case 2:
                        top = bottom = matches[0];
                        right = left = matches[1];
                        break;
                    case 3:
                        top = matches[0];
                        right = left = matches[1];
                        bottom = matches[2];
                        break;
                    case 4:
                        top = matches[0];
                        right = matches[1];
                        bottom = matches[2];
                        left = matches[3];
                        break;
                }

                SplitDeclaration(map.Key, String.Format(map.Value, "top"), top);
                SplitDeclaration(map.Key, String.Format(map.Value, "right"), right);
                SplitDeclaration(map.Key, String.Format(map.Value, "bottom"), bottom);
                SplitDeclaration(map.Key, String.Format(map.Value, "left"), left);

                Attributes.Remove(map.Key);
            }
        }

        private void CollapseDimensionsToShorthand() {
            foreach (var map in _dimensionsMap) {
                var dimensions = new Dimensions();
                dimensions.SetValues(Attributes, map.Value);

                if (!dimensions.HasValues)
                    continue;

                Attributes[map.Key] = dimensions.ToShorthand();
                Dimensions.Directions.ForEach(dir => Attributes.Remove(String.Format(map.Value, dir)));
            }
        }

        private void ExpandShorthandFont() {
        }

        private void CollapseFontToShorthand() {
        }

        private readonly string[] _listStyleProperties = { "list-style-type", "list-style-position", "list-style-image" };

        private void ExpandShorthandListStyle() {
        }

        private void CollapseListStyleToShorthand() {
        }

        private void SplitDeclaration(string source, string dest, string value) {
            if (String.IsNullOrWhiteSpace(value))
                return;

            if (Attributes.ContainsKey(dest))
                if (Attributes.IndexOfKey(dest) > Attributes.IndexOfKey(source))
                    return;
                else
                    Attributes[dest] = "";

            Attributes[dest] = value;
        }

        private class Dimensions {
            public static readonly string[] Directions = { "top", "right", "bottom", "left" };

            public string Top { get; set; }
            public string Right { get; set; }
            public string Bottom { get; set; }
            public string Left { get; set; }

            public bool HasValues {
                get {
                    return !String.IsNullOrWhiteSpace(Top)
                        || !String.IsNullOrWhiteSpace(Right)
                        || !String.IsNullOrWhiteSpace(Bottom)
                        || !String.IsNullOrWhiteSpace(Left);
                }
            }

            public void SetValues(IDictionary<string, string> values, string keyFormat) {
                Directions.ForEach(dir => SetValue(dir, values.ContainsKey(String.Format(keyFormat, dir)) ? values[String.Format(keyFormat, dir)].Trim().ToLowerInvariant() : null));
            }

            public void SetValue(string key, string value) {
                if (String.IsNullOrEmpty(value))
                    return;

                value = value.Trim();

                switch (key) {
                    case "top":
                        Top = value;
                        break;
                    case "right":
                        Right = value;
                        break;
                    case "bottom":
                        Bottom = value;
                        break;
                    case "left":
                        Left = value;
                        break;
                }
            }

            public string ToShorthand() {
                if (Left == Right)
                    if (Top == Bottom)
                        if (Top == Left) // all sides equal
                            return Top;
                        else // top and bottom are equal, left and right are equal
                            return String.Concat(Top, " ", Left); // top and bottom are equal, left and right are equal
                    else // only left and right are equa
                        return String.Concat(Top, " ", Left, " ", Bottom);
                
                // no sides are equal
                return String.Concat(Top, " ", Right, " ", Bottom, " ", Left);
            }
        }
    }
}