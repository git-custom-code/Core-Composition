namespace CustomCode.Core.Composition
{
    using System;

    /// <summary>
    /// Use this attribute on the class level to register a type as decorator at the <see cref="ServiceContainer"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is used in combination with the
    /// <see cref="ServiceContainerExtensions.UseAttributeConventions(ServiceContainer)"/>
    /// extension method.
    /// </remarks>
    /// <example>
    /// 
    /// - Use a plain attribute if your decorator follows the conventional decorator pattern:
    /// 
    /// [Decorator]
    /// public sealed class FooDecorator : IFoo
    /// {
    ///    public FooDecorator(IFoo foo)
    ///    {
    ///      ...
    ///    }
    /// }
    /// 
    /// - Specify the decorated type explicitely:
    /// 
    /// [Decorator(typeof(IFoo))]
    /// public sealed class FooDecorator : IFoo, IBar
    /// {
    ///    public FooDecorator(IFoo foo)
    ///    {
    ///      ...
    ///    }
    /// }
    /// 
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DecoratorAttribute : Attribute
    {
        #region Dependencies

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="decoratedServiceType"> The service type that should be decorated. </param>
        public DecoratorAttribute(Type decoratedServiceType = null)
        {
            DecoratedServiceType = decoratedServiceType;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the service type that should be decorated.
        /// </summary>
        public Type DecoratedServiceType { get; }

        #endregion
    }
}