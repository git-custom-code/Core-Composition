namespace CustomCode.Core.Composition
{
    using System;

    /// <summary>
    /// Use this attribute on the class level to register a type-specific decorator at the <see cref="ServiceContainer"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is used in combination with the <see cref="ExportAttribute"/> and the
    /// <see cref="ServiceContainerExtensions.UseAttributeConventions(ServiceContainer)"/>
    /// extension method.
    /// </remarks>
    /// <example>
    /// 
    /// - Use this attribute to register a decorator that is only valid for a specific type:
    /// 
    /// [Export]
    /// [DecoratedBy(typeof(FooDecorator))]
    /// public sealed class Foo : IFoo
    /// { }
    /// 
    /// public sealed class FooDecorator : IFoo
    /// {
    ///    public FooDecorator(IFoo foo)
    ///    {
    ///      ...
    ///    }
    /// }
    /// 
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DecoratedByAttribute : Attribute
    {
        #region Dependencies

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="decoratorType"> The type that should be used as decorator. </param>
        public DecoratedByAttribute(Type decoratorType)
        {
            DecoratorType = decoratorType;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the type that should be used as decorator.
        /// </summary>
        public Type DecoratorType { get; }

        #endregion
    }
}