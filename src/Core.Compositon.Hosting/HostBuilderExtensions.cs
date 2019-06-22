namespace CustomCode.Core.Composition.Hosting
{
    using LightInject;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extension methods for the <see cref="IHostBuilder"/> type.
    /// </summary>
    public static class HostBuilderExtensions
    {
        #region Logic

        /// <summary>
        /// Configure the extended <see cref="IHostBuilder"/> to use <see cref="global::LightInject.ServiceContainer"/>
        /// as inversion of control framework for dependency injection as well as attribute conventions.
        /// </summary>
        /// <param name="builder"> The extended <see cref="IHostBuilder"/> instance. </param>
        /// <param name="codeBase">
        /// Per default this extension method will search for all assemblies in the same folder (and subfolders) as the
        /// application's entry assembly. If you don't want that behavior, you can specify another root folder instead.
        /// </param>
        /// <returns> The configured <see cref="IHostBuilder"/> instance (for fluent syntax). </returns>
        public static IHostBuilder UseAttributeConventions(this IHostBuilder builder, string? codeBase = null)
        {
            builder = builder.UseServiceProviderFactory(new LightInjectServiceProviderFactory());
            builder = builder.ConfigureContainer<global::LightInject.ServiceContainer>((context, container) =>
            {
                container.UseAttributeConventions();
                container.RegisterIocVisibleAssemblies(codeBase);
                container.RegisterSelf();
            });
            return builder;
        }

        #endregion
    }
}