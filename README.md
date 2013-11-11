# PreMailer.Net [![Build Status](http://ci.milkshakehq.com/app/rest/builds/buildType:\(id:bt4\)/statusIcon)](http://ci.milkshakehq.com/project.html?projectId=project3&tab=projectOverview&guest=1)

C# Library for moving CSS to inline style attributes, to gain maximum E-mail client compatibility.

## Usage

    string htmlSource = File.ReadAllText(@"C:\Workspace\testmail.html");
    
    PreMailer pm = new PreMailer();
    
    string premailedOutput = pm.MoveCssInline(htmlSource, false);

### Media queries
If you want to [apply mobile styles to your e-mail](http://help.campaignmonitor.com/topic.aspx?t=164), you should put your
mobile specific styles in its own `style` block that targets the appropriate devices using media queries.

But since you cannot know by the time of sending an e-mail wether or not it will be viewed on a mobile device, the `style` 
block that targets mobile devices should not be inlined!

To ignore a `style` block, you need to specify an ignore selector when calling the `MoveCssInline` method, like this:

    string premailedOutput = pm.MoveCssInline(input, false, ignoreElements: "#ignore");
    
And your mobile specific `style` block should have an ID of `ignore`:
    
    <style type=\"text/css\" id=\"ignore\">.target { width: 1337px; }</style>

## Installation
**NuGet**: [PreMailer.Net](http://nuget.org/List/Packages/PreMailer.Net)

## Credits

* [The Dynamic Programmer](http://blog.dynamicprogrammer.com/2008/01/20/CSSParserClassInNET.aspx) - For his CSS parser

## License

PreMailer.Net is available under the MIT license. See the [LICENSE](https://github.com/milkshakesoftware/PreMailer.Net/blob/master/LICENSE) file for more info.
