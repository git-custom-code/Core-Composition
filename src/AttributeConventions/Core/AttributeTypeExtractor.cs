namespace LightInject.AttributeConventions.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A LightInject <see cref="ITypeExtractor"/> that extracts all types decorated with an <see cref="ExportAttribute"/>
    /// from a given <see cref="Assembly"/>.
    /// </summary>
    public sealed class AttributeTypeExtractor : ITypeExtractor
    {
        #region Data

        /// <summary>
        /// The type of the <see cref="ExportAttribute"/>.
        /// </summary>
        private readonly Type ExportType = typeof(ExportAttribute);

        #endregion

        #region Logic

        /// <summary>
        /// Extract all types decorated with an <see cref="ExportAttribute"/> from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly"> The <see cref="Assembly"/> for which to extract types. </param>
        /// <returns> A collection of types found in the given <paramref name="assembly"/>. </returns>
        public Type[] Execute(Assembly assembly)
        {
            return assembly
                .DefinedTypes
                .Where(t => t.GetTypeInfo().CustomAttributes.Any(a => a.AttributeType == ExportType))
                .ToArray();
        }

        #endregion
    }
}