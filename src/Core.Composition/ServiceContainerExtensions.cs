namespace CustomCode.Core.Composition
{
    using LightInject;
    using LightInjectExtensions;
    using Reflection;
#if netstandard20
    using System.Reflection;
#elif netstandard16
    using System.Runtime.Loader
#endif

    /// <summary>
    /// Extension methods for using attribute conventions with the <see cref="ServiceContainer"/> type.
    /// </summary>
    public static class ServiceContainerExtensions
    {
        #region Logic

        /// <summary>
        /// Configure the service <paramref name="container"/> to allow <see cref="ExportAttribute"/>s for registering types.
        /// </summary>
        /// <param name="container"> The <see cref="ServiceContainer"/> to be configured. </param>
        public static void UseAttributeConventions(this ServiceContainer container)
        {
            container.AssemblyScanner = new AttributeAssemblyScanner(
                new CachedTypeExtractor(new AttributeTypeExtractor()),
                new FactoryDelegateBuilder(),
                container.CompositionRootTypeExtractor,
                container.CompositionRootExecutor);
            container.ConstructorDependencySelector = new AttributeConstructorDependencySelector();
            container.ConstructorSelector = new AttributeConstructorSelector(container.CanGetInstance);
        }

        /// <summary>
        /// Configure the service <paramref name="container"/> to allow <see cref="IocVisibleAssemblyAttribute"/>s for
        /// registering assemblies.
        /// </summary>
        /// <param name="container"> The <see cref="ServiceContainer"/> to be configured. </param>
        /// <param name="codeBase">
        /// Per default this extension method will search for all assemblies in the same folder (and subfolders) as the
        /// application's entry assembly. If you don't want that behavior, you can specify another root folder instead.
        /// </param>
        public static void RegisterIocVisibleAssemblies(this ServiceContainer container, string codeBase = null)
        {
            var assemblyDiscoverer = new AssemblyDiscoverer();
            foreach (var assemblyPath in assemblyDiscoverer.DiscoverIocVisibleAssemblies(codeBase))
            {
#if netstandard20
                container.RegisterAssembly(Assembly.LoadFrom(assemblyPath));
#elif netstandard16
                container.RegisterAssembly(AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath));
#endif
            }
        }

        #endregion
    }
}