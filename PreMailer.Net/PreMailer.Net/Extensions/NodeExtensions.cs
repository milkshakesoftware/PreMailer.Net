#nullable enable

using AngleSharp.Dom;

namespace PreMailer.Net.Extensions
{
	public static class NodeExtensions
	{
		/// <summary>
		/// Get the text data from the first text node child of this node.
		/// This avoids the serialization overhead of <see cref="IElement.InnerHtml" /> and <see cref="INode.TextContent" />.
		/// Useful for getting CSS from a STYLE element.
		/// </summary>
		/// <param name="node">The node from where to start the search for a <see cref="IText" />.</param>
		/// <returns>The contents of the first text node, or null if none were found.</returns>
		public static string? GetFirstTextNodeData(this INode node)
		{
			return node.FindChild<IText>()?.Data;
		}
	}
}
