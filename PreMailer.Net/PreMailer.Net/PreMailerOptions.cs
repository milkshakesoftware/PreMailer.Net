#nullable enable
using AngleSharp;

namespace PreMailer.Net;

public class PreMailerOptions
{
	public bool RemoveStyleElements { get; set; }
	public string? IgnoreElements { get; set; } = null;
	public string? Css { get; set; }
	public bool StripIdAndClassAttributes { get; set; }
	public bool RemoveComments { get; set; }
	public IMarkupFormatter? CustomFormatter { get; set; } = null;
	public bool PreserveMediaQueries { get; set; }
	public bool ConvertLinkedCssWebExceptionsToWarnings { get; set; }
}