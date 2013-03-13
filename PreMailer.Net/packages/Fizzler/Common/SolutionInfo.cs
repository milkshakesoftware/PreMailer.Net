#region Copyright and License
// 
// Fizzler - CSS Selector Engine for Microsoft .NET Framework
// Copyright (c) 2009 Atif Aziz, Colin Ramsay. All rights reserved.
// 
// This library is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the Free 
// Software Foundation; either version 3 of the License, or (at your option) 
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more 
// details.
// 
// You should have received a copy of the GNU Lesser General Public License 
// along with this library; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
// 
#endregion

using System.Reflection;
using CLSCompliantAttribute = System.CLSCompliantAttribute;
using ComVisible = System.Runtime.InteropServices.ComVisibleAttribute;

//
// General description
//

[assembly: AssemblyProduct("Fizzler")]
[assembly: AssemblyCompany("http://fizzler.googlecode.com")]
[assembly: AssemblyCopyright("Copyright (c) 2009 Atif Aziz, Colin Ramsay. All rights reserved.")]
[assembly: AssemblyCulture("")]

//
// Version information
//

[assembly: AssemblyVersion("0.1.15619.0")]
[assembly: AssemblyInformationalVersion("1.015619.0")]

//
// Configuration (test, debug, release)
//

#if TEST
    #if !DEBUG
    #warning Test builds should be compiled using the DEBUG configuration.
    #endif
    [assembly: AssemblyConfiguration("Test")]
#elif DEBUG
    [assembly: AssemblyConfiguration("Debug")]
#else
    [assembly: AssemblyConfiguration("Release")]
#endif

//
// COM visibility and CLS compliance
//

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
