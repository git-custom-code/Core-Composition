namespace LightInject.AttributeConventions.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A <see cref="IConstructorDependencySelector"/> implementation that allows the usage of
    /// <see cref="ImportAttribute"/>s in combination with constructor parameters.
    /// </summary>
    public sealed class AttributeConstructorDependencySelector : ConstructorDependencySelector
    {
        #region Logic

        /// <summary>
        /// Selects the constructor dependencies for the given <paramref name="constructor"/>.
        /// </summary>
        /// <param name="constructor">
        /// The <see cref="ConstructionInfo"/> for which to select the constructor dependencies.
        /// </param>
        /// <returns>
        /// A list of <see cref="ConstructorDependency"/> instances that represents the constructor
        /// dependencies for the given <paramref name="constructor"/>.
        /// </returns>
        public override IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor)
        {
            var dependencies = base.Execute(constructor).ToArray();
            foreach (var dependency in dependencies)
            {
                var importAttribute = (ImportAttribute)dependency.Parameter
                    .GetCustomAttributes(typeof(ImportAttribute), true)
                    .FirstOrDefault();
                if (importAttribute != null)
                {
                    dependency.ServiceName = importAttribute.ServiceName;
                }
            }

            return dependencies;
        }

        #endregion
    }
}