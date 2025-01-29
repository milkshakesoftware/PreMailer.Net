using System.Collections.Generic;

namespace PreMailer.Net.Sources
{
	/// <summary>
	/// Arbitrary source of CSS code/definitions.
	/// </summary>
	public interface ICssSource
	{
		IEnumerable<string> GetCss();
	}
}