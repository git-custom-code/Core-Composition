namespace CustomCode.Core.Composition.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Discovers a collection of <see cref="Assembly"/>s, that are visible for inversion of control, i.e. assemblies that
    /// are marked with either the <see cref="IocVisibleAssemblyAttribute"/> or the <see cref="LightInject.CompositionRootTypeAttribute"/>.
    /// </summary>
    public sealed class AssemblyDiscoverer
    {
        #region Logic

        /// <summary>
        /// Searches for all assemblies in the current application directory for those that contain
        /// either the <see cref="IocVisibleAssemblyAttribute"/> or the <see cref="LightInject.CompositionRootTypeAttribute"/>
        /// and returns a list with the corresponding assembly paths.
        /// </summary>
        /// <returns> A list with all discovered assembly paths. </returns>
        /// <remarks>
        /// In order to evaluate the custom attribute data the assemblies are (reflection only) loaded
        /// in a seperate application domain that is unloaded afterwards.
        /// </remarks>
        public IEnumerable<string> DiscoverIocVisibleAssemblies(string? codeBase = null)
        {
            if (codeBase == null)
            {
                codeBase = Assembly.GetEntryAssembly().CodeBase;
            }

            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var applicationRootDir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(applicationRootDir))
            {
                throw new Exception("Unable to resolve the application root directory.");
            }

            var result = GetIocVisibleAssemblyPaths(applicationRootDir);
            return result;
        }

        /// <summary>
        /// Discover the paths of all assemblies beneath the <paramref name="applicationRootDir"/> that contain
        /// an assembly level attribute either of type <see cref="IocVisibleAssemblyAttribute"/> or
        /// <see cref="LightInject.CompositionRootTypeAttribute"/>.
        /// </summary>
        /// <param name="applicationRootDir"> The root directory that should be searched. </param>
        /// <returns> A collection with all found assembly paths. </returns>
        public IEnumerable<string> GetIocVisibleAssemblyPaths(string applicationRootDir)
        {
            var assemblyPaths = new List<string>();
            var directory = new DirectoryInfo(applicationRootDir);
            
            var libraries = Directory.GetFiles(applicationRootDir, "*.dll", SearchOption.AllDirectories)
                .Select(f => new SlimPortableExecutable(Path.GetFullPath(f)))
                .Where(f => f.IsIocVisibleAssembly())
                .ToList();
            var exes = Directory.GetFiles(applicationRootDir, "*.exe", SearchOption.AllDirectories)
                .Select(f => new SlimPortableExecutable(Path.GetFullPath(f)))
                .Where(f => f.IsIocVisibleAssembly())
                .ToList();

            foreach (var assembly in libraries.Union(exes))
            {
                assemblyPaths.Add(assembly.Path);
            }

            return assemblyPaths;
        }

        #endregion
    }
}