using System.Collections.Generic;
using System.Text;

namespace PreMailer.Net {
    public class StyleClass {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleClass"/> class.
        /// </summary>
        public StyleClass() {
            Attributes = new HashSet<CssAttribute>(new CssAttributeComparer());
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
        public HashSet<CssAttribute> Attributes { get; set; }

        /// <summary>
        /// Merges the specified style class, with this instance. Styles on this instance are not overriden by duplicates in the specified styleClass.
        /// </summary>
        /// <param name="styleClass">The style class.</param>
        /// <param name="canOverwrite">if set to <c>true</c> [can overwrite].</param>
        public void Merge(StyleClass styleClass, bool canOverwrite) {
            foreach (var item in styleClass.Attributes) {
                if (!Attributes.Contains(item)) Attributes.Add(item);
                else if (canOverwrite) {
                    Attributes.Remove(item);
                    Attributes.Add(item);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return string.Join(";", Attributes);
        }
    }
}