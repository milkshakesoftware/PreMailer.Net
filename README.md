PreMailer.Net
==========

C# Library for moving CSS to inline style attributes, to gain maximum E-mail client compatibility.

Usage
---------

    string htmlSource = File.ReadAllText(@"C:\Workspace\testmail.html");
    
    string premailedOutput = PreMailer.MoveCssInline(htmlSource, false);

Installation
----------
**NuGet**: [PreMailer.Net](http://nuget.org/List/Packages/PreMailer.Net)



Credits
-------

* [The Dynamic Programmer](http://blog.dynamicprogrammer.com/2008/01/20/CSSParserClassInNET.aspx) - For his CSS parser
* [Fizzler](http://code.google.com/p/fizzler/)