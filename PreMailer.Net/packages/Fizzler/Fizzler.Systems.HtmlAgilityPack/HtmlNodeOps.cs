#region Copyright and License
// 
// Fizzler - CSS Selector Engine for Microsoft .NET Framework
// Copyright (c) 2009 Atif Aziz, Colin Ramsay. All rights reserved.
// 
// This library is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the Free 
// Software Foundation; either version 3 of the License, or (at your option) 
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more 
// details.
// 
// You should have received a copy of the GNU Lesser General Public License 
// along with this library; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
// 
#endregion

namespace Fizzler.Systems.HtmlAgilityPack
{
    #region Imports

    using System;
    using System.Linq;
    using global::HtmlAgilityPack;

    #endregion

    /// <summary>
    /// An <see cref="IElementOps{TElement}"/> implementation for <see cref="HtmlNode"/>
    /// from <a href="http://www.codeplex.com/htmlagilitypack">HtmlAgilityPack</a>.
    /// </summary>
    public class HtmlNodeOps : IElementOps<HtmlNode>
    {
        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#type-selectors">type selector</a>,
        /// which represents an instance of the element type in the document tree. 
        /// </summary>
        public virtual Selector<HtmlNode> Type(NamespacePrefix prefix, string type)
        {
            return prefix.IsSpecific
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => nodes.Elements().Where(n => n.Name == type));
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#universal-selector">universal selector</a>,
        /// any single element in the document tree in any namespace 
        /// (including those without a namespace) if no default namespace 
        /// has been specified for selectors. 
        /// </summary>
        public virtual Selector<HtmlNode> Universal(NamespacePrefix prefix)
        {
            return prefix.IsSpecific
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => nodes.Elements());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#Id-selectors">ID selector</a>,
        /// which represents an element instance that has an identifier that 
        /// matches the identifier in the ID selector.
        /// </summary>
        public virtual Selector<HtmlNode> Id(string id)
        {
            return nodes => nodes.Elements().Where(n => n.Id == id);
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#class-html">class selector</a>,
        /// which is an alternative <see cref="IElementOps{TElement}.AttributeIncludes"/> when 
        /// representing the <c>class</c> attribute. 
        /// </summary>
        public virtual Selector<HtmlNode> Class(string clazz)
        {
            return nodes => nodes.Elements().Where(n => n.GetAttributeValue("class", string.Empty)
                                                         .Split(' ')
                                                         .Contains(clazz));
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the given attribute <paramref name="name"/>
        /// whatever the values of the attribute.
        /// </summary>
        public virtual Selector<HtmlNode> AttributeExists(NamespacePrefix prefix, string name)
        {
            return prefix.IsSpecific
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => nodes.Elements().Where(n => n.Attributes[name] != null));
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the given attribute <paramref name="name"/>
        /// and whose value is exactly <paramref name="value"/>.
        /// </summary>
        public virtual Selector<HtmlNode> AttributeExact(NamespacePrefix prefix, string name, string value)
        {
            return prefix.IsSpecific
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => from n in nodes.Elements()
                             let a = n.Attributes[name]
                             where a != null && a.Value == value
                             select n);
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the given attribute <paramref name="name"/>
        /// and whose value is a whitespace-separated list of words, one of 
        /// which is exactly <paramref name="value"/>.
        /// </summary>
        public virtual Selector<HtmlNode> AttributeIncludes(NamespacePrefix prefix, string name, string value)
        {
            return prefix.IsSpecific
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => from n in nodes.Elements()
                             let a = n.Attributes[name]
                             where a != null && a.Value.Split(' ').Contains(value)
                             select n);
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the given attribute <paramref name="name"/>,
        /// its value either being exactly <paramref name="value"/> or beginning 
        /// with <paramref name="value"/> immediately followed by "-" (U+002D).
        /// </summary>
        public virtual Selector<HtmlNode> AttributeDashMatch(NamespacePrefix prefix, string name, string value)
        {
            return prefix.IsSpecific || string.IsNullOrEmpty(value)
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => from n in nodes.Elements()                            
                             let a = n.Attributes[name]
                             where a != null && a.Value.Split('-').Contains(value)
                             select n);
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the attribute <paramref name="name"/> 
        /// whose value begins with the prefix <paramref name="value"/>.
        /// </summary>
        public Selector<HtmlNode> AttributePrefixMatch(NamespacePrefix prefix, string name, string value)
        {
            return prefix.IsSpecific || string.IsNullOrEmpty(value) 
                 ? (Selector<HtmlNode>) (nodes => Enumerable.Empty<HtmlNode>()) 
                 : (nodes => from n in nodes.Elements()
                             let a = n.Attributes[name]
                             where a != null && a.Value.StartsWith(value)
                             select n);
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the attribute <paramref name="name"/> 
        /// whose value ends with the suffix <paramref name="value"/>.
        /// </summary>
        public Selector<HtmlNode> AttributeSuffixMatch(NamespacePrefix prefix, string name, string value)
        {
            return prefix.IsSpecific || string.IsNullOrEmpty(value)
                 ? (Selector<HtmlNode>)(nodes => Enumerable.Empty<HtmlNode>())
                 : (nodes => from n in nodes.Elements()
                             let a = n.Attributes[name]
                             where a != null && a.Value.EndsWith(value)
                             select n);
        }

        /// <summary>
        /// Generates an <a href="http://www.w3.org/TR/css3-selectors/#attribute-selectors">attribute selector</a>
        /// that represents an element with the attribute <paramref name="name"/> 
        /// whose value contains at least one instance of the substring <paramref name="value"/>.
        /// </summary>
        public Selector<HtmlNode> AttributeSubstring(NamespacePrefix prefix, string name, string value)
        {
            return prefix.IsSpecific || string.IsNullOrEmpty(value)
                 ? (Selector<HtmlNode>)(nodes => Enumerable.Empty<HtmlNode>())
                 : (nodes => from n in nodes.Elements()
                             let a = n.Attributes[name]
                             where a != null && a.Value.Contains(value)
                             select n);
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
        /// which represents an element that is the first child of some other element.
        /// </summary>
        public virtual Selector<HtmlNode> FirstChild()
        {
            return nodes => nodes.Where(n => !n.ElementsBeforeSelf().Any());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
        /// which represents an element that is the last child of some other element.
        /// </summary>
        public virtual Selector<HtmlNode> LastChild()
        {
            return nodes => nodes.Where(n => n.ParentNode.NodeType != HtmlNodeType.Document 
                                          && !n.ElementsAfterSelf().Any());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
        /// which represents an element that is the N-th child of some other element.
        /// </summary>
        public virtual Selector<HtmlNode> NthChild(int a, int b)
        {
            if (a != 1)
                throw new NotSupportedException("The nth-child(an+b) selector where a is not 1 is not supported.");

            return nodes => from n in nodes
                            let elements = n.ParentNode.Elements().Take(b).ToArray()
                            where elements.Length == b && elements.Last().Equals(n)
                            select n;
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
        /// which represents an element that has a parent element and whose parent 
        /// element has no other element children.
        /// </summary>
        public virtual Selector<HtmlNode> OnlyChild()
        {
            return nodes => nodes.Where(n => n.ParentNode.NodeType != HtmlNodeType.Document
                                          && !n.ElementsAfterSelf().Concat(n.ElementsBeforeSelf()).Any());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
        /// which represents an element that has no children at all.
        /// </summary>
        public virtual Selector<HtmlNode> Empty()
        {
            return nodes => nodes.Elements().Where(n => n.ChildNodes.Count == 0);
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
        /// which represents a childhood relationship between two elements.
        /// </summary>
        public virtual Selector<HtmlNode> Child()
        {
            return nodes => nodes.SelectMany(n => n.Elements());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
        /// which represents a relationship between two elements where one element is an 
        /// arbitrary descendant of some ancestor element.
        /// </summary>
        public virtual Selector<HtmlNode> Descendant()
        {
            return nodes => nodes.SelectMany(n => n.Descendants().Elements());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
        /// which represents elements that share the same parent in the document tree and 
        /// where the first element immediately precedes the second element.
        /// </summary>
        public virtual Selector<HtmlNode> Adjacent()
        {
            return nodes => nodes.SelectMany(n => n.ElementsAfterSelf().Take(1));
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#combinators">combinator</a>,
        /// which separates two sequences of simple selectors. The elements represented
        /// by the two sequences share the same parent in the document tree and the
        /// element represented by the first sequence precedes (not necessarily
        /// immediately) the element represented by the second one.
        /// </summary>
        public virtual Selector<HtmlNode> GeneralSibling()
        {
            return nodes => nodes.SelectMany(n => n.ElementsAfterSelf());
        }

        /// <summary>
        /// Generates a <a href="http://www.w3.org/TR/css3-selectors/#pseudo-classes">pseudo-class selector</a>,
        /// which represents an element that is the N-th child from bottom up of some other element.
        /// </summary>
        public Selector<HtmlNode> NthLastChild(int a, int b)
        {
            if (a != 1)
                throw new NotSupportedException("The nth-last-child(an+b) selector where a is not 1 is not supported.");

            return nodes => from n in nodes
                            let elements = n.ParentNode.Elements().Skip(Math.Max(0, n.ParentNode.Elements().Count() - b)).Take(b).ToArray()
                            where elements.Length == b && elements.First().Equals(n)
                            select n;
        }
    }
}
