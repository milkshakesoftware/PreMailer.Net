// No usings needed

namespace PreMailerDotNet.Parsing
{
	public enum SelectorTypes
	{
		/// <summary>
		/// Default, used when no selector is specified.
		/// </summary>
		InlineStyle = 0,

		/// <summary>
		/// CSS selector: #priceInput
		/// </summary>
		Id = 1,

		/// <summary>
		/// CSS selector: .blue-bg
		/// </summary>
		ClassName = 2,

		/// <summary>
		/// CSS selector: a[href=#delete]
		/// </summary>
		Attribute = 3,

		/// <summary>
		/// CSS selector: h2:first-child
		/// </summary>
		PseudoClass = 4,

		/// <summary>
		/// CSS selector: p::first-line
		/// </summary>
		PseudoElement = 5,

		/// <summary>
		/// CSS selector: div
		/// </summary>
		Element = 6
	}
}