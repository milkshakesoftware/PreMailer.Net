using System.Collections.Generic;
using System.Text;

namespace PreMailer.Net
{
	public class InlineResult
	{
		public StringBuilder StringBuilder { get; protected set; }
		public string Html => StringBuilder.ToString();

		public List<string> Warnings { get; protected set; }
		// TODO: Add plain-text output.
		// TODO: Store processing Errors.

		public InlineResult(StringBuilder stringBuilder, List<string> warnings = null)
		{
			StringBuilder = stringBuilder;
			Warnings = warnings ?? new List<string>();
		}
	}
}