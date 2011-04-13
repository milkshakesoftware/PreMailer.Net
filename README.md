PreMailer.Net
==========

C# Library for moving CSS to inline style attributes, to gain maximum E-mail client compatibility.

Usage
---------

    string htmlSource = File.ReadAllText(@"C:\Workspace\testmail.html");
    
    PreMailer pm = new PreMailer();
    
    string premailedOutput = pm.MoveCssInline(htmlSource, false);

Credits
-------

* [The Dynamic Programmer](http://blog.dynamicprogrammer.com/2008/01/20/CSSParserClassInNET.aspx)
* [Fizzler](http://code.google.com/p/fizzler/)