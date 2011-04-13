PreMailer.Net
==========

C# Library for moving CSS to inline style attributes, to gain maximum E-mail client compatibility.

Usage
---------

    string htmlSource = File.ReadAllText(@"C:\Workspace\testmail.html");
    
    PreMailer pm = new PreMailer();
    
    string premailedOutput = pm.MoveCssInline(htmlSource, false);