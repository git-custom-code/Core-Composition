namespace CustomCode.Core.Composition
{
    using System;

    /// <summary>
    /// An attribute that can be used once per type on a single constructor.
    /// The decorated constructor will be used to register a function factory for the type at 
    /// the <see cref="LightInject.ServiceContainer"/>.
    /// </summary>
    /// <example>
    /// 
    /// Register a function factory that allows to specify all constructor parameters externally:
    /// 
    /// [Export]
    /// public sealed class Foo : IFoo
    /// {
    ///     [FactoryParameters]
    ///     public Foo(int id)
    ///     {
    ///         ...
    ///     }
    /// }
    /// 
    /// Register a function factory that allows to specify the first parameter (id, names or case insensitive) externally,
    /// the second parameter (IBar) is still resolved using the service container
    /// 
    /// [Export]
    /// public sealed class Foo : IFoo
    /// {
    ///     [FactoryParameters(nameof(Id))]
    ///     public Foo(int id, IBar bar)
    ///     {
    ///         Id = id;
    ///         ...
    ///     }
    ///
    ///     public int Id { get; }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class FactoryParametersAttribute : Attribute
    {
        #region Dependencies

        /// <summary>
        /// Create a new instance of the <see cref="FactoryParametersAttribute"/> type.
        /// </summary>
        /// <param name="parameterNames">
        /// The names of the constructor parameters that should be used as parameters for a function factory.
        /// </param>
        public FactoryParametersAttribute(params string[] parameterNames)
        {
            if (parameterNames?.Length > 0)
            {
                ParameterNames = parameterNames;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the names of the constructor parameters that should be used as parameters
        /// for a function factory.
        /// </summary>
        public string[]? ParameterNames { get; }

        #endregion
    }
}