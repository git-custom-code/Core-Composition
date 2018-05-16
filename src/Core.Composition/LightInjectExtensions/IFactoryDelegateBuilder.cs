namespace CustomCode.Core.Composition.LightInjectExtensions
{
    using LightInject;
    using System;
    using System.Reflection;

    /// <summary>
    /// Interface for types that can create factory delegates for usage by LightInject's <see cref="ServiceContainer"/>.
    /// </summary>
    public interface IFactoryDelegateBuilder
    {
        /// <summary>
        /// Create a new dynamically compiled factory delegate for a given <paramref name="type"/>,
        /// if- and only if- one of the type's constructors is marked with a <see cref="FactoryParametersAttribute"/>.
        /// </summary>
        /// <param name="type"> The type that should be created via a factory. </param>
        /// <returns> A delegate that can create a new instance of the specified <paramref name="type"/>. </returns>
        Delegate CreateFactoryFor(TypeInfo type);
    }
}