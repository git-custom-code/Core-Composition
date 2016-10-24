namespace LightInject.AttributeConventions.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A <see cref="IConstructorSelector"/> implementation that allows the usage of
    /// <see cref="ImportAttribute"/>s in combination with constructor parameters.
    /// </summary>
    public sealed class AttributeConstructorSelector : MostResolvableConstructorSelector
    {
        #region Dependencies

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="canGetInstance">
        /// A delegate that determines if a service type can be resolved.
        /// </param>
        public AttributeConstructorSelector(Func<Type, string, bool> canGetInstance)
            : base(canGetInstance)
        { }

        #endregion

        #region Logic

        /// <summary>
        /// Gets the service name based on the given <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter"> The <see cref="ParameterInfo"/> for which to get the service name. </param>
        /// <returns>
        /// The name of the service for the given <paramref name="parameter"/>.
        /// </returns>
        protected override string GetServiceName(ParameterInfo parameter)
        {
            var importAttribute = (ImportAttribute)parameter
                .GetCustomAttributes(typeof(ImportAttribute), true)
                .FirstOrDefault();

            return importAttribute == null ? base.GetServiceName(parameter) : importAttribute.ServiceName;
        }

        #endregion
    }
}