#region Disclaimer

/*********************************************************************************

    This is a slightly modified copy of Bernhard Richter's light inject
    dependency injection repository, since at the time of creating this
    code, said repo was a) not signed and b) had no support for the new
    .Net core generic host.
    Once those two features are implemented by the author, this copy
    can be safely removed.

******************************************************************************
    The MIT License (MIT)
    Copyright (c) 2018 bernhard.richter@gmail.com
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    LightInject.Microsoft.DependencyInjection version 2.2.0
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/

#endregion

namespace CustomCode.Core.Composition.Hosting.LightInject
{
    using global::LightInject;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extends the <see cref="IServiceContainer"/> interface.
    /// </summary>
    public static class ServiceContainerExtensions
    {
        #region Logic

        /// <summary>
        /// Creates an <see cref="IServiceProvider"/> based on the given <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/>.</param>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> that contains information about the services to be registered.</param>
        /// <returns>A configured <see cref="IServiceProvider"/>.</returns>
        public static IServiceProvider CreateServiceProvider(this IServiceContainer container, IServiceCollection serviceCollection)
        {
            if (container.ScopeManagerProvider.GetScopeManager(container).CurrentScope != null)
            {
                throw new InvalidOperationException("CreateServiceProvider can only be called once per IServiceContainer instance.");
            }

            var rootScope = container.BeginScope();
            rootScope.Completed += (a, s) => container.Dispose();
            container.Register<IServiceProvider>(factory => new LightInjectServiceProvider(container), new PerRootScopeLifetime(rootScope));
            container.Register<IServiceScopeFactory>(factory => new LightInjectServiceScopeFactory(container), new PerRootScopeLifetime(rootScope));
            RegisterServices(container, rootScope, serviceCollection);
            return new LightInjectServiceScope(rootScope).ServiceProvider;
        }

        private static void RegisterServices(IServiceContainer container, Scope rootScope, IServiceCollection serviceCollection)
        {
            var registrations = serviceCollection.Select(d => CreateServiceRegistration(d, rootScope)).ToList();

            for (var i = 0; i < registrations.Count; ++i)
            {
                var registration = registrations[i];
                registration.ServiceName = i.ToString("D8", CultureInfo.InvariantCulture.NumberFormat);
                container.Register(registration);
            }
        }

        private static ServiceRegistration CreateServiceRegistration(ServiceDescriptor serviceDescriptor, Scope rootScope)
        {
            if (serviceDescriptor.ImplementationFactory != null)
            {
                return CreateServiceRegistrationForFactoryDelegate(serviceDescriptor, rootScope);
            }

            if (serviceDescriptor.ImplementationInstance != null)
            {
                return CreateServiceRegistrationForInstance(serviceDescriptor, rootScope);
            }

            return CreateServiceRegistrationServiceType(serviceDescriptor, rootScope);
        }

        private static ServiceRegistration CreateServiceRegistrationServiceType(ServiceDescriptor serviceDescriptor, Scope rootScope)
        {
            var registration = CreateBasicServiceRegistration(serviceDescriptor, rootScope);
            registration.ImplementingType = serviceDescriptor.ImplementationType;
            return registration;
        }

        private static ServiceRegistration CreateServiceRegistrationForInstance(ServiceDescriptor serviceDescriptor, Scope rootScope)
        {
            var registration = CreateBasicServiceRegistration(serviceDescriptor, rootScope);
            registration.Value = serviceDescriptor.ImplementationInstance;
            return registration;
        }

        private static ServiceRegistration CreateServiceRegistrationForFactoryDelegate(ServiceDescriptor serviceDescriptor, Scope rootScope)
        {
            var registration = CreateBasicServiceRegistration(serviceDescriptor, rootScope);
            registration.FactoryExpression = CreateFactoryDelegate(serviceDescriptor);
            return registration;
        }

        private static ServiceRegistration CreateBasicServiceRegistration(ServiceDescriptor serviceDescriptor, Scope rootScope)
        {
            var registration = new ServiceRegistration
            {
                Lifetime = ResolveLifetime(serviceDescriptor, rootScope),
                ServiceType = serviceDescriptor.ServiceType,
                ServiceName = Guid.NewGuid().ToString(),
            };
            return registration;
        }

        private static ILifetime? ResolveLifetime(ServiceDescriptor serviceDescriptor, Scope rootScope)
        {
            if (serviceDescriptor.ImplementationInstance != null)
            {
                return null;
            }

            ILifetime? lifetime = null;

            switch (serviceDescriptor.Lifetime)
            {
                case ServiceLifetime.Scoped:
                    lifetime = new PerScopeLifetime();
                    break;
                case ServiceLifetime.Singleton:
                    lifetime = new PerRootScopeLifetime(rootScope);
                    break;
                case ServiceLifetime.Transient:
                    lifetime = new PerRequestLifeTime();
                    break;
            }

            return lifetime;
        }

        private static Delegate CreateFactoryDelegate(ServiceDescriptor serviceDescriptor)
        {
            var openGenericMethod = typeof(ServiceContainerExtensions).GetTypeInfo().GetDeclaredMethod("CreateTypedFactoryDelegate");
            var closedGenericMethod = openGenericMethod.MakeGenericMethod(serviceDescriptor.ServiceType);
            return (Delegate)closedGenericMethod.Invoke(null, new object[] { serviceDescriptor });
        }

        private static Func<IServiceFactory, T> CreateTypedFactoryDelegate<T>(ServiceDescriptor serviceDescriptor)
        {
            return serviceFactory => (T)serviceDescriptor.ImplementationFactory(serviceFactory.GetInstance<IServiceProvider>());
        }

        #endregion
    }
}