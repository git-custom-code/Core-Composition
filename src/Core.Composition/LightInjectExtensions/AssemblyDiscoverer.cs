namespace CustomCode.Core.Composition.LightInjectExtensions
{
    using LightInject;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    
    /// <summary>
    /// Discovers a collection of <see cref="Assembly"/>s, that are visible for inversion of control, i.e. assemblies that
    /// are marked with either the <see cref="IocVisibleAssemblyAttribute"/> or the <see cref="CompositionRootTypeAttribute"/>.
    /// </summary>
    /// <remarks>
    /// This class creates a new instance of a <see cref="AssemblyWithAttributeLoader"/> class internally, which does
    /// the actual ReflectionOnly loading of the <see cref="Assembly"/> into the new <see cref="AppDomain"/>.
    /// (see http://www.codeproject.com/Articles/42312/Loading-Assemblies-in-Separate-Directories-Into-a/ for more details)
    /// </remarks>
    public sealed class AssemblyDiscoverer
    {
        #region Logic
        

        #endregion
    }
}