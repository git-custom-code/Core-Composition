using System.Reflection;

[assembly: AssemblyProduct("CustomCode.Core.Composition")]

[assembly: AssemblyCompany("CustomCode")]
[assembly: AssemblyCopyright("Copyright © 2016 - 2017")]
[assembly: AssemblyTrademark("CustomCode")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif