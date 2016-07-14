namespace LightInject.AttributeConventions.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;

    /// <summary>
    /// Discover a collection of <see cref="Assembly"/>s that are visible for inversion of control, i.e. assemblies that
    /// are marked with either the <see cref="IocVisibleAssemblyAttribute"/> or the <see cref="CompositionRootTypeAttribute"/>.
    /// </summary>
    /// <remarks>
    /// This class creates a new instance of a <see cref="AssemblyWithAttributeLoader"/> class internally, which does
    /// the actual ReflectionOnly loading of the <see cref="Assembly"/> into the new <see cref="AppDomain"/>.
    /// (see <see cref="http://www.codeproject.com/Articles/42312/Loading-Assemblies-in-Separate-Directories-Into-a"/>
    /// for more details)
    /// </remarks>
    public sealed class AssemblyDiscoverer
    {
        #region Logic

        /// <summary>
        /// Creates a new <see cref="AppDomain"/> based on the <paramref name="parentDomain"/>'s <see cref="Evidence"/>
        /// and <see cref="AppDomainSetup"/>
        /// </summary>
        /// <param name="parentDomain">The parent <see cref="AppDomain"/>. </param>
        /// <returns> The newly created <see cref="AppDomain"/>. </returns>
        private AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            var evidence = new Evidence(parentDomain.Evidence);
            var setup = parentDomain.SetupInformation;
            var domain = AppDomain.CreateDomain(typeof(IocVisibleAssemblyAttribute).Name, evidence, setup);

            return domain;
        }

        /// <summary>
        /// Searches for all assemblies in the current application directory for those that contain
        /// either the <see cref="IocVisibleAssemblyAttribute"/> or the <see cref="CompositionRootTypeAttribute"/>
        /// and returns a list with the corresponding assembly paths.
        /// </summary>
        /// <returns> A list with all discovered assembly paths. </returns>
        /// <remarks>
        /// In order to evaluate the custom attribute data the assemblies are (reflection only) loaded
        /// in a seperate application domain that is unloaded afterwards.
        /// </remarks>
        public IEnumerable<string> DiscoverIocVisibleAssemblies(string codeBase = null)
        {
            AppDomain childDomain = null;

            try
            {
                childDomain = BuildChildDomain(AppDomain.CurrentDomain);
                var loaderType = typeof(AssemblyWithAttributeLoader);
                var loader = (AssemblyWithAttributeLoader)childDomain
                    .CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName)
                    .Unwrap();

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

                var result = loader.GetIocVisibleAssemblyPaths(applicationRootDir);
                return result;
            }
            finally
            {
                if (childDomain != null)
                {
                    AppDomain.Unload(childDomain);
                }
            }
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Use an instance of this type to discover all paths of ioc visible assemblies under a specific root directory.
        /// A seperate application domain is used to discover the assemblies in order to prevent any unwanted
        /// loading of assemblies.
        /// </summary>
        /// <remarks>
        /// This type inherits from <see cref="MarshalByRefObject"/> to allow the CLR to marshall
        /// this object by reference across <see cref="AppDomain"/> boundaries.
        /// </remarks>
        private sealed class AssemblyWithAttributeLoader : MarshalByRefObject
        {
            #region Logic

            /// <summary>
            /// Discover the paths of all assemblies beneath the <paramref name="applicationRootDir"/> that contain
            /// an assembly level attribute either of type <see cref="IocVisibleAssemblyAttribute"/> or
            /// <see cref="CompositionRootTypeAttribute"/>.
            /// </summary>
            /// <param name="applicationRootDir"> The root directory that should be searched. </param>
            /// <returns> A collection with all found assembly paths. </returns>
            public IEnumerable<string> GetIocVisibleAssemblyPaths(string applicationRootDir)
            {
                var assemblyPaths = new List<string>();

                var directory = new DirectoryInfo(applicationRootDir);
                ResolveEventHandler resolveEventHandler = (s, e) => OnReflectionOnlyResolve(e, directory);

                try
                {
                    AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;
                    var libraries = Directory.GetFiles(applicationRootDir, "*.dll", SearchOption.AllDirectories)
                        .Select(LoadAssemblyForReflection)
                        .Where(assembly => assembly != null)
                        .ToList();
                    var exes = Directory.GetFiles(applicationRootDir, "*.exe", SearchOption.AllDirectories)
                        .Select(LoadAssemblyForReflection)
                        .Where(assembly => assembly != null)
                        .ToList();

                    foreach (var assembly in libraries.Union(exes))
                    {
                        if (assembly.GetCustomAttributesData().Any(IsVisibleForIoc))
                        {
                            assemblyPaths.Add(assembly.Location);
                        }
                    }
                }
                finally
                {
                    AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;
                }

                return assemblyPaths;
            }

            /// <summary>
            /// Check if the specified assembly is a managed (.net) assembly or not.
            /// </summary>
            /// <param name="assemblyPath"> The full path to the assembly that should be checked. </param>
            /// <returns> True if the specified assembly is a managed (.net) assembly, false otherwise. </returns>
            private bool IsManagedAssembly(string assemblyPath)
            {
                using (var fileStream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    if (fileStream.Length < 64)
                    {
                        return false;
                    }

                    //PE Header starts @ 0x3C (60). Its a 4 byte header.
                    fileStream.Position = 0x3C;
                    var peHeaderPointer = binaryReader.ReadUInt32();
                    if (peHeaderPointer == 0)
                    {
                        peHeaderPointer = 0x80;
                    }

                    // Ensure there is at least enough room for the following structures:
                    //     24 byte PE Signature & Header
                    //     28 byte Standard Fields         (24 bytes for PE32+)
                    //     68 byte NT Fields               (88 bytes for PE32+)
                    // >= 128 byte Data Dictionary Table
                    if (peHeaderPointer > fileStream.Length - 256)
                    {
                        return false;
                    }

                    // Check the PE signature.  Should equal 'PE\0\0'.
                    fileStream.Position = peHeaderPointer;
                    var peHeaderSignature = binaryReader.ReadUInt32();
                    if (peHeaderSignature != 0x00004550)
                    {
                        return false;
                    }

                    // skip over the PEHeader fields
                    fileStream.Position += 20;
                    const ushort pe32 = 0x10b;
                    const ushort pe32Plus = 0x20b;

                    // Read PE magic number from Standard Fields to determine format.
                    var peFormat = binaryReader.ReadUInt16();
                    if (peFormat != pe32 && peFormat != pe32Plus)
                    {
                        return false;
                    }

                    // Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
                    // When this is non-zero then the file contains CLI data otherwise not.
                    var dataDictionaryStart = (ushort)(peHeaderPointer + (peFormat == pe32 ? 232 : 248));
                    fileStream.Position = dataDictionaryStart;

                    var cliHeaderRva = binaryReader.ReadUInt32();
                    if (cliHeaderRva == 0)
                    {
                        return false;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Query if an <see cref="Assembly"/> contains a custom attribute either of type
            /// <see cref="IocVisibleAssemblyAttribute"/> or <see cref="CompositionRootTypeAttribute"/>.
            /// </summary>
            /// <param name="attributeData"> The attribute(s) that are defined by the assembly. </param>
            /// <returns>
            /// True if the <paramref name="attributeData"/> is either of type <see cref="IocVisibleAssemblyAttribute"/>
            /// or <see cref="CompositionRootTypeAttribute"/>, false otherwise.
            /// </returns>
            private bool IsVisibleForIoc(CustomAttributeData attributeData)
            {
                if (attributeData != null &&
                    attributeData.Constructor != null &&
                    attributeData.Constructor.DeclaringType != null)
                {
                    return nameof(IocVisibleAssemblyAttribute).Equals(attributeData.Constructor.DeclaringType.Name, StringComparison.OrdinalIgnoreCase) ||
                        nameof(CompositionRootTypeAttribute).Equals(attributeData.Constructor.DeclaringType.Name, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }

            /// <summary>
            /// (Reflection only) Load of a single <see cref="Assembly"/> defined by the <paramref name="assemblyPath"/>.
            /// </summary>
            /// <param name="assemblyPath"> The path of the <see cref="Assembly"/> to load for reflection. </param>
            /// <returns> The (reflection only) loaded assembly or null if no such assembly was found. </returns>
            private Assembly LoadAssemblyForReflection(string assemblyPath)
            {
                try
                {
                    if (IsManagedAssembly(assemblyPath))
                    {
                        return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    }

                    return null;
                }
                catch (BadImageFormatException)
                {
                    return null;
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            }

            /// <summary>
            /// Event that is raised when an <see cref="Assembly"/> needs to be resolved.
            /// </summary>
            /// <param name="args"> The arguments of the event. </param>
            /// <param name="directory"> The current assembly directory. </param>
            /// <returns> The (reflection only) loaded assembly. </returns>
            private Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
            {
                var loadedAssembly = AppDomain.CurrentDomain
                    .ReflectionOnlyGetAssemblies()
                    .FirstOrDefault(asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));

                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                var assemblyName = new AssemblyName(args.Name);
                string dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");

                if (File.Exists(dependentAssemblyFilename))
                {
                    return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename);
                }

                return Assembly.ReflectionOnlyLoad(args.Name);
            }

            #endregion
        }

        #endregion
    }
}