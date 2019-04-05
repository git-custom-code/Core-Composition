namespace CustomCode.Core.Composition
{
    using System;

    /// <summary>
    /// Use this attribute on the class level to register a type at the <see cref="LightInject.ServiceContainer"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is used in combination with the
    /// <see cref="ServiceContainerExtensions.UseAttributeConventions(LightInject.ServiceContainer)"/>
    /// extension method.
    /// </remarks>
    /// <example>
    /// 
    /// - If the type implements no interface it is registered by the type itself:
    /// 
    /// [Export]
    /// public sealed class Foo
    /// { }
    /// 
    /// - If the type implements a single interface it is registered by the interface:
    /// 
    /// [Export]
    /// public sealed class Foo : IFoo
    /// { }
    /// 
    /// - If the type implements multiple interfaces register it once per interface:
    ///
    /// [Export]
    /// public sealed class Foo : IFoo, IBar
    /// { }
    /// 
    /// - If the type implements multiple interfaces you can register the type only once
    ///   by specifying the interface explicitely:
    /// 
    /// [Export(typeof(IFoo))]
    /// public sealed class Foo : IFoo, IBar
    /// { }
    /// 
    /// - Per default types are registerd as transient. You can change that by specifing the
    ///   lifetime as singelton or scoped:
    /// 
    /// [Export(Lifetime.Singelton)]
    /// public sealed class Foo : IFoo, IBar
    /// { }
    /// 
    /// - And of course you can combine specifying a single interface with a custom lifetime:
    /// 
    /// [Export(typeof(IFoo), Lifetime.Singleton)]
    /// public sealed class Foo : IFoo, IBar
    /// { }
    /// 
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        #region Dependencies

        /// <summary>
        /// Default ctor.
        /// </summary>
        public ExportAttribute()
            : this(null, Lifetime.Transient)
        { }

        /// <summary>
        /// Create a new export attribute.
        /// </summary>
        /// <param name="serviceType"> The service type to register. </param>
        public ExportAttribute(Type? serviceType)
            : this(serviceType, Lifetime.Transient)
        { }

        /// <summary>
        /// Create a new export attribute.
        /// </summary>
        /// <param name="lifetime"> The service's lifetime. </param>
        public ExportAttribute(Lifetime lifetime)
            : this(null, lifetime)
        { }

        /// <summary>
        /// Create a new export attribute.
        /// </summary>
        /// <param name="serviceType"> The service type to register. </param>
        /// <param name="lifetime"> The service's lifetime. </param>
        public ExportAttribute(Type? serviceType = null, Lifetime lifetime = Lifetime.Transient)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the service type to register.
        /// </summary>
        public Type? ServiceType { get; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the service's lifetime.
        /// </summary>
        public Lifetime Lifetime { get; }

        #endregion
    }
}