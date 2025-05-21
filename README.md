# PreMailer.Net ![.NET Core build](https://github.com/milkshakesoftware/PreMailer.Net/workflows/.NET%20Core%20build/badge.svg) [![Nuget count](https://img.shields.io/nuget/v/PreMailer.Net.svg?style=flat-square)](https://www.nuget.org/packages/PreMailer.Net/)

C# Library for moving CSS to inline style attributes, to gain maximum E-mail client compatibility.

## Usage

### Static method on `PreMailer` class
```csharp
string htmlSource = File.ReadAllText(@"C:\Workspace\testmail.html");

var result = PreMailer.MoveCssInline(htmlSource);

result.Html 		// Resultant HTML, with CSS in-lined.
result.Warnings 	// string[] of any warnings that occurred during processing.
```

### Set up `PreMailer` instance
```csharp
string htmlSource = File.ReadAllText(@"C:\Workspace\testmail.html");

var pm = new PreMailer(htmlSource);
// Optional to add analytics tags
pm.AddAnalyticsTags(source, medium, campaign, content, domain = null);

var result = pm.MoveCssInline(
    removeStyleElements: false,
    ignoreElements: "#ignore",
    preserveMediaQueries: true
);

result.Html       // Resultant HTML, with CSS in-lined.
result.Warnings   // List<string> of any warnings that occurred during processing.
```

### Options
The following options can be passed to the `PreMailer.MoveCssInline` method to configure its behavior:

- `baseUri(Uri = null)` - Base URL to apply to `link` elements with `href` values ending with `.css`.
- `removeStyleElements(bool = false)` - Removes elements that were used to source CSS (currently, only `style` is supported).
- `ignoreElements(string = null)` - CSS selector of element(s) _not_ to inline. Useful for mobile styles (see below).
- `css(string = null)` - A string containing a style-sheet for inlining.
- `stripIdAndClassAttributes(bool = false)` - True to strip ID and class attributes.
- `removeComments(bool = false)` - True to remove comments, false to leave them intact.
- `customFormatter(IMarkupFormatter = null)` - Custom formatter to use for the HTML output.
- `preserveMediaQueries(bool = false)` - If true and removeStyleElements is true, it will preserve media queries in the style node while removing other CSS, instead of removing the entire style node.
- `useEmailFormatter(bool = false)` - If true, empty HTML tags will be preserved as full tags instead of being converted to self-closing tags, and HTML entities like &copy; will be preserved.

### External style sheets
Sometimes it's handy to reference external style sheets with a `<link href="..." />` element. PreMailer will download and use external style sheets as long as the value of `href` ends with `.css`.

Both absolute and relative URLs are supported. If the URL is relative, you must specify the `baseUri` parameter in either the constructor, or when calling the static `MoveCssInline` method.

`<link />` elements that match the `ignoreElements` selector won't be downloaded.

### Media queries
If you want to [apply mobile styles to your e-mail](http://help.campaignmonitor.com/topic.aspx?t=164), you should put your
mobile specific styles in its own `style` block that targets the appropriate devices using media queries.

But since you cannot know by the time of sending an e-mail whether or not it will be viewed on a mobile device, the `style`
block that targets mobile devices should not be inlined!

To ignore a `style` block, you need to specify an ignore selector when calling the `MoveCssInline` method, like this:

```csharp
var result = PreMailer.MoveCssInline(input, false, ignoreElements: "#ignore");
```

Alternatively, you can use the `preserveMediaQueries` parameter to keep your media queries while removing other styles:

```csharp
var result = PreMailer.MoveCssInline(input, removeStyleElements: true, preserveMediaQueries: true);
```

And your mobile specific `style` block should have an ID of `ignore`:

```html
<style type="text/css" id="ignore">.target { width: 1337px; }</style>
```

### Premailer specific CSS becomes HTML attributes
Premailer looks for the use of CSS attributes prefixed with `-premailer` and will proxy the value through to the DOM element as an attribute.

For example

```css
table {
    -premailer-cellspacing: 5;
    -premailer-width: 500;
}
```

will make a `table` element render as

```html
<table cellspacing="5" width="500">
```
### Analytics Tags
The `AddAnalyticsTags` method can be used to add Google Analytics tracking parameters to links in your HTML:

```csharp
var pm = new PreMailer(htmlSource);
pm.AddAnalyticsTags(
    source: "newsletter",       // utm_source parameter
    medium: "email",            // utm_medium parameter
    campaign: "summer_sale",    // utm_campaign parameter
    content: "logo_link",       // utm_content parameter
    domain: "example.com"       // Optional: only add tags to links matching this domain
);
```

This will append `?utm_source=newsletter&utm_medium=email&utm_campaign=summer_sale&utm_content=logo_link` to all links, or to links matching the specified domain if provided.



### Custom DOM Processing
The `Document` property provides access to the underlying `IHtmlDocument` object, allowing you to perform custom DOM manipulation before inlining CSS:

```csharp
using(var pm = new PreMailer(html)){
  var document = pm.Document;

  // Use AngleSharp to process document before moving css inline...
  
  var result = pm.MoveCssInline();
}
```

This is useful for advanced scenarios where you need to modify the HTML structure before applying CSS.

### Notes

- Pseudo classes/elements which not supported by external dependencies, or doesn't make sense in email, will be ignored and logged to the `InlineResult.Warnings` collection.

## Installation
**NuGet**: [PreMailer.Net](http://nuget.org/List/Packages/PreMailer.Net)

## Contributors

* [martinnormark](https://github.com/martinnormark)
* [robcthegeek](https://github.com/robcthegeek)

[Among others](https://github.com/milkshakesoftware/PreMailer.Net/graphs/contributors)

## License

PreMailer.Net is available under the MIT license. See the [LICENSE](https://github.com/milkshakesoftware/PreMailer.Net/blob/master/LICENSE) file for more info.
