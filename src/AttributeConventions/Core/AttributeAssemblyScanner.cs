namespace LightInject.AttributeConventions.Core
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a class that is capable of scanning an assembly and register services into an
    /// <see cref="IServiceContainer"/> instance supporting <see cref="ExportAttribute"/>s.
    /// </summary>
    public sealed class AttributeAssemblyScanner : IAssemblyScanner
    {
        #region Dependencies

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="typeExtractor">
        /// The <see cref="ITypeExtractor"/> that is responsible for extracting concrete types from the assembly being scanned.
        /// </param>
        /// <param name="compositionRootTypeExtractor">
        /// The <see cref="ITypeExtractor"/> that is responsible for extracting <see cref="ICompositionRoot"/> implementations
        /// from the assembly being scanned.
        /// </param>
        /// <param name="compositionRootExecutor">
        /// The <see cref="ICompositionRootExecutor"/> that is responsible for creating and executing an
        /// <see cref="ICompositionRoot"/>.
        /// </param>
        public AttributeAssemblyScanner(
            ITypeExtractor typeExtractor,
            ITypeExtractor compositionRootTypeExtractor,
            ICompositionRootExecutor compositionRootExecutor)
        {
            Contract.Requires(typeExtractor != null);
            Contract.Requires(compositionRootTypeExtractor != null);
            Contract.Requires(compositionRootExecutor != null);

            TypeExtractor = typeExtractor;
            CompositionRootTypeExtractor = compositionRootTypeExtractor;
            CompositionRootExecutor = compositionRootExecutor;
        }

        /// <summary>
        /// Gets the <see cref="ITypeExtractor"/> that is responsible for extracting concrete types from
        /// the assembly being scanned.
        /// </summary>
        private ITypeExtractor TypeExtractor { get; }

        /// <summary>
        /// Gets the <see cref="ITypeExtractor"/> that is responsible for extracting <see cref="ICompositionRoot"/>
        /// implementations from the assembly being scanned.
        /// </summary>
        private ITypeExtractor CompositionRootTypeExtractor { get; }

        /// <summary>
        /// Gets the <see cref="ICompositionRootExecutor"/> that is responsible for creating and executing an
        /// <see cref="ICompositionRoot"/>.
        /// </summary>
        private ICompositionRootExecutor CompositionRootExecutor { get; }

        #endregion

        #region Data

        /// <summary>
        /// Gets or sets the currently scanned assembly.
        /// </summary>
        private Assembly CurrentAssembly { get; set; }

        /// <summary>
        /// Gets a regular expression that checks for non-generic type names.
        /// </summary>
        private Regex Regex { get; } = new Regex("((?:[a-z][a-z]+))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Logic

        /// <summary>
        /// Scans the target <paramref name="assembly"/> and executes composition roots found within the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly"> The <see cref="Assembly"/> to scan. </param>
        /// <param name="serviceRegistry"> The target <see cref="IServiceRegistry"/> instance. </param>
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
        {
            var compositionRootTypes = CompositionRootTypeExtractor.Execute(assembly);
            if (compositionRootTypes.Length > 0 && !Equals(CurrentAssembly, assembly))
            {
                CurrentAssembly = assembly;
                foreach (var compositionRoot in compositionRootTypes)
                {
                    CompositionRootExecutor.Execute(compositionRoot);
                }
            }
        }

        /// <summary>
        /// Scans the target <paramref name="assembly"/> and registers all services found within the assembly.
        /// </summary>
        /// <param name="assembly"> The <see cref="Assembly"/> to scan. </param>
        /// <param name="serviceRegistry"> The target <see cref="IServiceRegistry"/> instance. </param>
        /// <param name="lifetime">
        /// The <see cref="ILifetime"/> factory that controls the lifetime of the registered service.
        /// </param>
        /// <param name="shouldRegister">
        /// A function delegate that determines if a service implementation should be registered.
        /// </param>
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister)
        {
            var types = TypeExtractor.Execute(assembly);
            foreach (var type in types)
            {
                RegisterInteral(type, serviceRegistry, lifetime, shouldRegister);
            }
        }

        /// <summary>
        /// Register a new service at the <see cref="ServiceContainer"/>.
        /// </summary>
        /// <param name="implementingType"> The type that should be registerd at the container. </param>
        /// <param name="serviceRegistry"> The <see cref="ServiceContainer"/> that should register the <paramref name="implementingType"/>. </param>
        /// <param name="lifetimeFactory"> The default lifetime for the service. </param>
        /// <param name="shouldRegister"> A delegate that specifies if the service should be registered. </param>
        private void RegisterInteral(Type implementingType, IServiceRegistry serviceRegistry, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            var info = implementingType.GetTypeInfo();
            var exports = info.GetCustomAttributes<ExportAttribute>();
            foreach (var export in exports)
            {
                var serviceTypes = FindServiceTypes(info, export) ?? new[] { implementingType };
                foreach (var serviceType in serviceTypes)
                {
                    if (shouldRegister(serviceType, implementingType))
                    {
                        var service = new ServiceRegistration();
                        service.ServiceName = export.ServiceName ?? GetServiceName(serviceType, implementingType);
                        service.Lifetime = GetLifetime(export) ?? lifetimeFactory();
                        service.ServiceType = serviceType;
                        service.ImplementingType = implementingType;

                        serviceRegistry.Register(service);
                    }
                }
            }
        }

        /// <summary>
        /// Find the service type that is used to register a service at the container.
        /// </summary>
        /// <param name="info"> The <see cref="TypeInfo"/> for the service to register. </param>
        /// <param name="export"> The <see cref="ExportAttribute"/> data for the service to register. </param>
        /// <returns> The service types that are used to register the service at the container. </returns>
        private Type[] FindServiceTypes(TypeInfo info, ExportAttribute export)
        {
            // 1) use type defined by export
            if (export.ServiceType != null)
            {
                return new[] { export.ServiceType };
            }

            // 2) use interface if only 1 interface is present
            var interfaces = info.ImplementedInterfaces.ToArray();
            if (interfaces.Length == 1)
            {
                return new[] { interfaces[0] };
            }
            // 3) use interface that equals type name precceded by an "I" if present
            else if (interfaces.Length > 1)
            {
                var defaultInterfaceName = $"I{info.Name}";
                foreach (var @interface in interfaces)
                {
                    if (@interface.Name == info.Name || @interface.Name == defaultInterfaceName)
                    {
                        return new[] { @interface };
                    }
                }

                // 4) use all interfaces instead
                return interfaces;
            }

            // 5) use the implementation type as default (see above)
            return null;
        }

        /// <summary>
        /// Get the lifetime that is used to register a service at the container.
        /// </summary>
        /// <param name="export"> The <see cref="ExportAttribute"/> data for the service to register. </param>
        /// <returns> The lifetime that is used to register the service at the container. </returns>
        private ILifetime GetLifetime(ExportAttribute export)
        {
            if (export.Lifetime == Lifetime.Transient)
            {
                return null;
            }
            else if (export.Lifetime == Lifetime.Singleton)
            {
                return new PerContainerLifetime();
            }
            else if (export.Lifetime == Lifetime.Scoped)
            {
                return new PerScopeLifetime();
            }

            return null;
        }

        /// <summary>
        /// Get the service name that is used to register a service at the container.
        /// </summary>
        /// <param name="serviceType"> The service type to be registered. </param>
        /// <param name="implementingType"> The implementation type to be registered. </param>
        /// <returns> The service name that is used to register a service at the container. </returns>
        private string GetServiceName(Type serviceType, Type implementingType)
        {
            var implementingTypeName = implementingType.Name;
            var serviceTypeName = serviceType.Name;
            if (implementingType.GetTypeInfo().IsGenericTypeDefinition)
            {
                implementingTypeName = Regex.Match(implementingTypeName).Groups[1].Value;
                serviceTypeName = Regex.Match(serviceTypeName).Groups[1].Value;
            }

            if (serviceTypeName.Substring(1) == implementingTypeName)
            {
                implementingTypeName = string.Empty;
            }

            return implementingTypeName;
        }

        #endregion
    }
}