using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyProduct("PreMailer.Net")]
[assembly: AssemblyCompany("Milkshake Software")]
[assembly: AssemblyCopyright("Copyright © Milkshake Software 2013")]

[assembly: AssemblyVersion("1.1.3.0")]
[assembly: AssemblyFileVersion("1.1.3.0")]

#if DEBUG 
[assembly: AssemblyConfiguration("Debug")] 
#else
[assembly: AssemblyConfiguration("Release")]
#endif