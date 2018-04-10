namespace CustomCode.Core.Composition
{
    using System;

    /// <summary>
    /// Use this attribute on the assembly level to register all types decorated with an
    /// <see cref="ExportAttribute"/> at the <see cref="LightInject.ServiceContainer"/>.
    /// If you don't want to use attribute conventions you can use the <see cref="LightInject.CompositionRootTypeAttribute"/> instead.
    /// </summary>
    /// <remarks>
    /// This attribute is used in combination with the
    /// <see cref="ServiceContainerExtensions.RegisterIocVisibleAssemblies(LightInject.ServiceContainer, string)"/>
    /// extension method.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class IocVisibleAssemblyAttribute : Attribute
    { }
}