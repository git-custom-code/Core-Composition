namespace LightInject.AttributeConventions
{
    using System;

    /// <summary>
    /// Use this attribute on the assembly level to register all types decorated with an
    /// <see cref="ExportAttribute"/> at the <see cref="ServiceContainer"/>.
    /// If you don't want to use attribute conventions you can use the <see cref="CompositionRootTypeAttribute"/> instead.
    /// </summary>
    /// <remarks>
    /// This attribute is used in combination with the
    /// <see cref="ServiceContainerExtensions.RegisterIocVisibleAssemblies(ServiceContainer, string)"/>
    /// extension method.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class IocVisibleAssemblyAttribute : Attribute
    { }
}