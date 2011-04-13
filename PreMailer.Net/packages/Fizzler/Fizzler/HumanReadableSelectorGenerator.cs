namespace Fizzler
{
    using System;

    /// <summary>
    /// An <see cref="ISelectorGenerator"/> implementation that generates
    /// human-readable description of the selector.
    /// </summary>
    public class HumanReadableSelectorGenerator : ISelectorGenerator
    {
        private int _chainCount;
        private string _text;

        /// <summary>
        /// Initializes the text.
        /// </summary>
        public virtual void OnInit()
        {
            Text = null;
        }

        /// <summary>
        /// Gets the generated human-readable description text.
        /// </summary>
        public string Text
        {
            get { return _text; }
            private set { _text = value; }
        }

        /// <summary>
        /// Generates human-readable for a selector in a group.
        /// </summary>
        public virtual void OnSelector()
        {
            if (string.IsNullOrEmpty(Text))
                Text = "Take all";
            else
                Text += " and select them. Combined with previous, take all";
        }

        /// <summary>
        /// Concludes the text.
        /// </summary>
        public virtual void OnClose()
        {
            Text = Text.Trim();
            Text += " and select them.";
        }

        /// <summary>
        /// Adds to the generated human-readable text.
        /// </summary>
        protected void Add(string selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            Text += selector;
        }

        /// <summary>
        /// Generates human-readable text of this type selector.
        /// </summary>
        public void Type(NamespacePrefix prefix, string type)
        {
            Add(string.Format(" <{0}> elements", type));
        }

        /// <summary>
        /// Generates human-readable text of this universal selector.
        /// </summary>
        public void Universal(NamespacePrefix prefix)
        {
            Add(" elements");
        }

        /// <summary>
        /// Generates human-readable text of this ID selector.
        /// </summary>
        public void Id(string id)
        {
            Add(string.Format(" with an ID of '{0}'", id));
        }

        /// <summary>
        /// Generates human-readable text of this class selector.
        /// </summary>
        void ISelectorGenerator.Class(string clazz)
        {
            Add(string.Format(" with a class of '{0}'", clazz));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributeExists(NamespacePrefix prefix, string name)
        {
            Add(string.Format(" which have attribute {0} defined", name));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributeExact(NamespacePrefix prefix, string name, string value)
        {
            Add(string.Format(" which have attribute {0} with a value of '{1}'", name, value));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributeIncludes(NamespacePrefix prefix, string name, string value)
        {
            Add(string.Format(" which have attribute {0} that includes the word '{1}'", name, value));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributeDashMatch(NamespacePrefix prefix, string name, string value)
        {
            Add(string.Format(" which have attribute {0} with a hyphen separated value matching '{1}'", name, value));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributePrefixMatch(NamespacePrefix prefix, string name, string value)
        {
            Add(string.Format(" which have attribute {0} whose value begins with '{1}'", name, value));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributeSuffixMatch(NamespacePrefix prefix, string name, string value)
        {
            Add(string.Format(" which have attribute {0} whose value ends with '{1}'", name, value));
        }

        /// <summary>
        /// Generates human-readable text of this attribute selector.
        /// </summary>
        public void AttributeSubstring(NamespacePrefix prefix, string name, string value)
        {
            Add(string.Format(" which have attribute {0} whose value contains '{1}'", name, value));
        }

        /// <summary>
        /// Generates human-readable text of this pseudo-class selector.
        /// </summary>
        public void FirstChild()
        {
            Add(" which are the first child of their parent");
        }

        /// <summary>
        /// Generates human-readable text of this pseudo-class selector.
        /// </summary>
        public void LastChild()
        {
            Add(" which are the last child of their parent");
        }

        /// <summary>
        /// Generates human-readable text of this pseudo-class selector.
        /// </summary>
        public void NthChild(int a, int b)
        {
            Add(string.Format(" where the element has {0}n+{1}-1 sibling before it", a, b));
        }

        /// <summary>
        /// Generates human-readable text of this pseudo-class selector.
        /// </summary>
        public void OnlyChild()
        {
            Add(" where the element is the only child");
        }

        /// <summary>
        /// Generates human-readable text of this pseudo-class selector.
        /// </summary>
        public void Empty()
        {
            Add(" where the element is empty");
        }

        /// <summary>
        /// Generates human-readable text of this combinator.
        /// </summary>
        public void Child()
        {
            Add(", then take their immediate children which are");
        }

        /// <summary>
        /// Generates human-readable text of this combinator.
        /// </summary>
        public void Descendant()
        {
            if (_chainCount > 0)
            {
                Add(". With those, take only their descendants which are");
            }
            else
            {
                Add(", then take their descendants which are");
                _chainCount++;
            }
        }

        /// <summary>
        /// Generates human-readable text of this combinator.
        /// </summary>
        public void Adjacent()
        {
            Add(", then take their immediate siblings which are");
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
        /// which separates two sequences of simple selectors. The elements represented
        /// by the two sequences share the same parent in the document tree and the
        /// element represented by the first sequence precedes (not necessarily
        /// immediately) the element represented by the second one.
        /// </summary>
        public void GeneralSibling()
        {
            Add(", then take their siblings which are");
        }

        /// <summary>
        /// Generates human-readable text of this combinator.
        /// </summary>
        public void NthLastChild(int a, int b)
        {
            Add(string.Format(" where the element has {0}n+{1}-1 sibling after it", a, b));
        }
    }
}