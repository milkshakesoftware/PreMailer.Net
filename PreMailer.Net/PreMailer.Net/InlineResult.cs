using System.Collections.Generic;

namespace PreMailer.Net
{
	public class InlineResult
	{
		public string Html { get; protected set; }
		public List<string> Warnings { get; protected set; }
		// TODO: Add plain-text output.
		// TODO: Store processing Errors.

		public InlineResult(string html, List<string> warnings = null)
		{
			Html = html;
			Warnings = warnings ?? new List<string>();
		}
	}
}