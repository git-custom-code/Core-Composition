namespace CustomCode.Core.Composition.LightInjectExtensions
{
    using LightInject;
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A LightInject <see cref="ITypeExtractor"/> that extracts all types decorated with an <see cref="ExportAttribute"/>
    /// or <see cref="DecoratorAttribute"/> from a given <see cref="Assembly"/>.
    /// </summary>
    public sealed class AttributeTypeExtractor : ITypeExtractor
    {
        #region Data

        /// <summary>
        /// The type of the <see cref="ExportAttribute"/>.
        /// </summary>
        private readonly Type ExportType = typeof(ExportAttribute);

        /// <summary>
        /// The type of the <see cref="DecoratorAttribute"/>.
        /// </summary>
        private readonly Type DecoratorType = typeof(DecoratorAttribute);

        #endregion

        #region Logic

        /// <summary>
        /// Extract all types decorated with an <see cref="ExportAttribute"/> or <see cref="DecoratorAttribute"/>
        /// from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly"> The <see cref="Assembly"/> for which to extract types. </param>
        /// <returns> A collection of types found in the given <paramref name="assembly"/>. </returns>
        public Type[] Execute(Assembly assembly)
        {
            return assembly
                .DefinedTypes
                .Where(t => t.GetTypeInfo().CustomAttributes.Any(a =>
                    a.AttributeType == ExportType ||
                    a.AttributeType == DecoratorType))
                .ToArray();
        }

        #endregion
    }
}