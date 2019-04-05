namespace CustomCode.Core.Composition
{
    using System;

    /// <summary>
    /// Use this attribute for constructor parameters to import a named type from the <see cref="LightInject.ServiceContainer"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is used in combination with the
    /// <see cref="ServiceContainerExtensions.UseAttributeConventions(LightInject.ServiceContainer)"/>
    /// extension method.
    /// </remarks>
    /// <example>
    /// 
    /// - If multiple types implement the same interface , you can use the import attribute to get a specific
    ///   implementation by service name
    /// 
    /// [Export(ServiceName = "Bar1")]
    /// public sealed class Bar1 : IBar
    /// { }
    /// 
    /// [Export(ServiceName = "Bar2")]
    /// public sealed class Bar2 : IBar
    /// { }
    /// 
    /// [Export]
    /// public sealed class Foo
    /// {
    ///   public Foo([Import("Bar2")] IBar bar)
    ///   { }
    /// }
    /// 
    /// </example>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ImportAttribute : Attribute
    {
        #region Dependencies

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="serviceName"> The name of the service to be imported. </param>
        public ImportAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the name of the service to be imported.
        /// </summary>
        public string ServiceName { get; }

        #endregion
    }
}