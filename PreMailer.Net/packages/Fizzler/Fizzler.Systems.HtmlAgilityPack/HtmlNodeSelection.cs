namespace Fizzler.Systems.HtmlAgilityPack
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::HtmlAgilityPack;

    #endregion

    /// <summary>
    /// Selector API for <see cref="HtmlNode"/>.
    /// </summary>
    /// <remarks>
    /// For more information, see <a href="http://www.w3.org/TR/selectors-api/">Selectors API</a>.
    /// </remarks>
    public static class HtmlNodeSelection
    {
        private static readonly HtmlNodeOps _ops = new HtmlNodeOps();

        /// <summary>
        /// Similar to <see cref="QuerySelectorAll(HtmlNode,string)" /> 
        /// except it returns only the first element matching the supplied 
        /// selector strings.
        /// </summary>
        public static HtmlNode QuerySelector(this HtmlNode node, string selector)
        {
            return node.QuerySelectorAll(selector).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all element nodes from descendants of the starting 
        /// element node that match any selector within the supplied 
        /// selector strings. 
        /// </summary>
        public static IEnumerable<HtmlNode> QuerySelectorAll(this HtmlNode node, string selector)
        {
            return QuerySelectorAll(node, selector, null);
        }

        /// <summary>
        /// Retrieves all element nodes from descendants of the starting 
        /// element node that match any selector within the supplied 
        /// selector strings. An additional parameter specifies a 
        /// particular compiler to use for parsing and compiling the 
        /// selector.
        /// </summary>
        /// <remarks>
        /// The <paramref name="compiler"/> can be <c>null</c>, in which
        /// case a default compiler is used. If the selector is to be used
        /// often, it is recommended to use a caching compiler such as the
        /// one supplied by <see cref="CreateCachingCompiler()"/>.
        /// </remarks>
        public static IEnumerable<HtmlNode> QuerySelectorAll(this HtmlNode node, string selector, Func<string, Func<HtmlNode, IEnumerable<HtmlNode>>> compiler)
        {
            return (compiler ?? Compile)(selector)(node);
        }

        /// <summary>
        /// Parses and compiles CSS selector text into run-time function.
        /// </summary>
        /// <remarks>
        /// Use this method to compile and reuse frequently used CSS selectors
        /// without parsing them each time.
        /// </remarks>
        public static Func<HtmlNode, IEnumerable<HtmlNode>> Compile(string selector)
        {
            var compiled = Parser.Parse(selector, new SelectorGenerator<HtmlNode>(_ops)).Selector;
            return node => compiled(Enumerable.Repeat(node, 1));
        }

        //
        // Caching
        //

        [ThreadStatic] 
        private static readonly Func<string, Func<HtmlNode, IEnumerable<HtmlNode>>> _defaultCachingCompiler = CreateCachingCompiler();

        /// <summary>
        /// Compiles a selector. If the selector has been previously 
        /// compiled then this method returns it rather than parsing
        /// and compiling the selector on each invocation.
        /// </summary>
        /// <remarks>
        /// The cache is per-thread and therefore thread-safe without
        /// lock contention.
        /// </remarks>
        public static Func<HtmlNode, IEnumerable<HtmlNode>> CachableCompile(string selector)
        {
            return _defaultCachingCompiler(selector);
        }

        /// <summary>
        /// Creates a caching selector compiler that uses a default
        /// cache strategy when the selector text is regarded as being
        /// orginally case-insensitive.
        /// </summary>
        public static Func<string, Func<HtmlNode, IEnumerable<HtmlNode>>> CreateCachingCompiler()
        {
            return CreateCachingCompiler(null);
        }

        /// <summary>
        /// Creates a caching selector compiler where the compiled selectors
        /// are maintained in a user-supplied <see cref="IDictionary{TKey,TValue}"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// If <paramref name="cache"/> is <c>null</c> then this method uses a
        /// the <see cref="Dictionary{TKey,TValue}"/> implementation with an 
        /// ordinally case-insensitive selectors text comparer.
        /// </remarks>
        public static Func<string, Func<HtmlNode, IEnumerable<HtmlNode>>> CreateCachingCompiler(IDictionary<string, Func<HtmlNode, IEnumerable<HtmlNode>>> cache)
        {
            return SelectorsCachingCompiler.Create(Compile, cache);
        }
    }
}
