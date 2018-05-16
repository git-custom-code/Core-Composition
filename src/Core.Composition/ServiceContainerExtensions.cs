namespace CustomCode.Core.Composition
{
    using CustomCode.Core.Composition.ExceptionHandling;
    using LightInject;
    using LightInjectExtensions;
    using Reflection;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
#if NETSTANDARD1_6
    using System.Runtime.Loader;
#endif

    /// <summary>
    /// Extension methods for using attribute conventions with the <see cref="ServiceContainer"/> type.
    /// </summary>
    public static class ServiceContainerExtensions
    {
        #region Logic

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
#if NETSTANDARD2_0
                container.RegisterAssembly(Assembly.LoadFrom(assemblyPath));
#elif NETSTANDARD1_6
                container.RegisterAssembly(AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath));
#endif
            }
        }

        /// <summary>
        /// Register the <paramref name="container"/> as <see cref="IServiceFactory"/> singleton.
        /// </summary>
        /// <param name="container"> The <see cref="ServiceContainer"/> to be registered. </param>
        public static void RegisterSelf(this ServiceContainer container)
        {
            container.RegisterInstance<IServiceFactory>(container);
        }

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
        /// Verify that the container can resolve all registered dependencies by trying to create
        /// an instance of each registered service an collecting all unresolved dependency exceptions.
        /// </summary>
        /// <param name="container"> The container that should perform the dependency check. </param>
        /// <exception cref="UnresolvedDependenciesException">
        /// Thrown when one or more dependencies could not be resolved by the <paramref name="container"/>.
        /// </exception>
        /// <remarks>
        /// Note that this method is not intended to be used in release builds and therefore is marked
        /// with a conditional debug attribute.
        /// </remarks>
        [Conditional("DEBUG")]
        public static void VerifyDependencies(this ServiceContainer container)
        {
            var unresolvedDependencies = new List<UnresolvedDependencyException>();
            foreach (var service in container.AvailableServices)
            {
                try
                {
                    if (service.Lifetime is PerScopeLifetime)
                    {
                        using (var scope = container.BeginScope())
                        {
                            container.GetInstance(service.ServiceType, service.ServiceName);
                        }
                    }
                    else if (service.FactoryExpression != null)
                    {
                        var arguments = GetFactoryDefaultValues(service);
                        container.GetInstance(service.ServiceType, service.ServiceName, arguments);
                    }
                    else
                    {
                        container.GetInstance(service.ServiceType, service.ServiceName);
                    }
                }
                catch (Exception e)
                {
                    var unresolvedDependencyException = e.AsUnresolvedDependencyException();
                    if (unresolvedDependencyException != null)
                    {
                        unresolvedDependencies.Add(unresolvedDependencyException);
                    }
                }
            }

            if (unresolvedDependencies.Count > 0)
            {
                throw new UnresolvedDependenciesException(unresolvedDependencies);
            }
        }

        /// <summary>
        /// Get the default values for a registered <paramref name="service"/> factory.
        /// </summary>
        /// <param name="service"> The service whose default values should be retrieved. </param>
        /// <returns> The <paramref name="service"/>'s default values. </returns>
        private static object[] GetFactoryDefaultValues(ServiceRegistration service)
        {
            var type = service.ImplementingType.GetTypeInfo();
            foreach (var constructor in type.GetConstructors())
            {
                if (constructor.GetCustomAttribute(typeof(FactoryParametersAttribute)) is FactoryParametersAttribute attribute)
                {
                    var parameters = constructor.GetParameters();
                    var parameterNameLut = new HashSet<string>();
                    if (attribute.ParameterNames == null)
                    {
                        foreach(var parameter in parameters)
                        {
                            parameterNameLut.Add(parameter.Name.ToLowerInvariant());
                        }
                    }
                    else
                    {
                        foreach (var parameter in attribute.ParameterNames)
                        {
                            parameterNameLut.Add(parameter.ToLowerInvariant());
                        }
                    }

                    var arguments = new object[parameterNameLut.Count];
                    var index = 0;
                    foreach(var parameter in parameters)
                    {
                        if (parameterNameLut.Contains(parameter.Name.ToLowerInvariant()))
                        {
                            if (parameter.ParameterType.GetTypeInfo().IsValueType)
                            {
                                arguments[index] = Activator.CreateInstance(parameter.ParameterType);
                            }
                            else
                            {
                                arguments[index] = null;
                            }
                            ++index;
                        }
                    }

                    return arguments;
                }
            }

            return null;
        }

        #endregion
    }
}