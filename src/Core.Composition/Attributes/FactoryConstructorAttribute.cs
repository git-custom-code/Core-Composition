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
    ///     [FactoryConstructor]
    ///     public Foo(int id)
    ///     {
    ///         ...
    ///     }
    /// }
    /// 
    /// Register a function factory that allows to specify the first parameter (id) externally, the
    /// second parameter (IBar) is resolved using the service container
    /// 
    /// [Export]
    /// public sealed class Foo : IFoo
    /// {
    ///     [FactoryConstructor(0)]
    ///     public Foo(int id, IBar bar)
    ///     {
    ///         ...
    ///     }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class FactoryConstructorAttribute : Attribute
    {
        #region Dependencies

        /// <summary>
        /// Default ctor.
        /// </summary>
        public FactoryConstructorAttribute()
        { }

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="argumentIndices">
        /// Indices that specify which constructor arguments should be used as arguments for a function factory.
        /// </param>
        public FactoryConstructorAttribute(params int[] argumentIndices)
        {
            ArgumentIndices = argumentIndices;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets a collection that specifies which constructor arguments (specified by index)
        /// should be used as arguments for a function factory.
        /// </summary>
        public int[] ArgumentIndices { get; }

        #endregion
    }
}