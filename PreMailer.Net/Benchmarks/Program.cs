using AngleSharp;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

public static class Program
{
	public static void Main()
	{
		// Some local environments may run into issues with Windows Defender or 
		// SentinelOne (and others) when running a benchmark. This ensures we
		// keep our toolchain within our process and stops the above apps from blocking
		// our benchmark process, but can slow the execution time.
		var avSafeConfig = DefaultConfig.Instance
			.AddJob(
				Job.ShortRun
					.WithToolchain(InProcessNoEmitToolchain.Instance)
					.WithIterationCount(100)
			);

		BenchmarkRunner.Run<Realistic>(avSafeConfig);
	}
}

[MemoryDiagnoser]
public class Realistic
{
	[Benchmark]
	public void AngleSharpBaseline()
	{
		new HtmlParser().ParseDocument(RawHtml).ToHtml();
	}

	[Benchmark]
	public void MoveCssInline()
	{
		PreMailer.Net.PreMailer.MoveCssInline(RawHtml);
	}

	[Benchmark]
	public void MoveCssInline_AllFlags()
	{
		PreMailer.Net.PreMailer.MoveCssInline(
			RawHtml,
			removeStyleElements: true,
			ignoreElements: ".container table:not(:first-child)",
			css: "table td { color: #123 } table.body-wrap { width: 10%;}",
			stripIdAndClassAttributes: true,
			removeComments: true,
			preserveMediaQueries: true);
	}

	private static readonly string RawHtml = @"
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta name=""viewport"" content=""width=device-width, initial-scale=1, maximum-scale=1"">

<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
<title>PreMailer Benchmark</title>

</head>
 
<body bgcolor=""#123"">

<style>
/* ------------------------------------- 
		GLOBAL 
------------------------------------- */
* { 
	margin:0;
	padding:0;
}
* { font-family: Arial, sans-serif; font-size:15px; }

img { 
	max-width: 100%; 
}
.collapse {
	margin:0;
	padding:0;
}
body {
	-webkit-font-smoothing:antialiased; 
	-webkit-text-size-adjust:none; 
	width: 100%!important; 
	height: 100%;
}


/* ------------------------------------- 
		ELEMENTS 
------------------------------------- */
a { color: #456;}

/* ------------------------------------- 
		HEADER 
------------------------------------- */
table.head-wrap { width: 100%;}
.header .content{
  border-left:1px solid black;
  border-right:1px solid black;
  border-top:1px solid black;
}

/* ------------------------------------- 
		BODY 
------------------------------------- */
table.body-wrap { width: 100%;}
.body .content{
  border-left:1px solid black;
  border-right:1px solid black;
  border-bottom:1px solid black;
}

/* ------------------------------------- 
		FOOTER 
------------------------------------- */
table.footer-wrap { width: 100%;}


/* ------------------------------------- 
		TYPOGRAPHY 
------------------------------------- */
h1,h2,h3,h4,h5,h6 {
font-family:  Arial, sans-serif; line-height: 1.1; margin-bottom:15px; color:#000;
}

h1 { font-weight:200; font-size: 28px;}
h2 { font-weight:200; font-size: 20px;}
h3 { font-weight:500; font-size: 16px;}

.collapse { margin:0!important;}

p, ul { 
	margin-bottom: 10px; 
	font-weight: normal; 
	font-size:14px; 
	line-height:1.6;
}

ul li {
	margin-left:5px;
	list-style-position: inside;
}


/* --------------------------------------------------- 
		RESPONSIVENESS
		Nuke it from orbit. It's the only way to be sure. 
------------------------------------------------------ */

/* Set a max-width, and make it display as block so it will automatically stretch to that width, but will also shrink down on a phone */
.container {
	display:block!important;
	max-width:600px!important;
	margin:0 auto!important; /* makes it centered */
	clear:both!important;
}

/* This should also be a block element, so that it will fill 100% of the .container */
.content {
	padding:15px;
	max-width:600px;
	margin:0 auto;
	display:block; 
}

/* Make sure tables in the content area are 100% wide */
.content table { width: 100%; }


/* Odds and ends */
.column {
	width: 300px;
	float:left;
}
.column tr td { padding: 15px; }
.column-wrap { 
	padding:0!important; 
	margin:0 auto; 
	max-width:600px!important;
}
.column table { width:100%;}
.social .column {
	width: 280px;
	min-width: 279px;
	float:left;
}

/* Be sure to place a .clear element after each set of columns, just to be safe */
.clear { display: block; clear: both; }

</style>

  <table><tr><td height=""10"">&nbsp;</td></tr></table>
<!-- HEADER -->
<table class=""head-wrap"" cellspacing=""0"">
	<tr>
		<td></td>
		<td class=""header container"">
				
				<div class=""content"">
				<table>
					<tr>
						<td align=""center""><img style=""max-width:200px;"" width=""200"" src=""https://dummy-url.invalid"" alt="""" /></td>
					</tr>
				</table>
				</div>
				
		</td>
		<td></td>
	</tr>
</table><!-- /HEADER -->


<!-- BODY -->
<table class=""body-wrap"" cellspacing=""0"">
	<tr>
		<td></td>
		<td class=""body container"">

			<div class=""content"">
			<table>
				<tr>
					<td>
Hello!<br />
<br />
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
<br />
<br />
<div>
	Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
</div>
<br />
<br />
<br />
<br/><br/>
					</td>
				</tr>
			</table>
			</div><!-- /content --></td>
		<td></td>
	</tr>
</table><!-- /BODY -->


</body>
</html>";
}