namespace CustomCode.Core.Composition.ExceptionHandling
{
    using Core.ExceptionHandling;
    using LightInject;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Exception that is thrown when a <see cref="ServiceContainer"/> couldn't create at least
    /// one registered service instance because of an unresolved dependency.
    /// </summary>
    public sealed class UnresolvedDependenciesException : TechnicalAggregateException
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="UnresolvedDependencyException"/> type.
        /// </summary>
        /// <param name="unresolvedDependencies"> The service container's unresolved dependencies. </param>
        public UnresolvedDependenciesException(IEnumerable<UnresolvedDependencyException> unresolvedDependencies)
            : base(unresolvedDependencies, $"Service container has {unresolvedDependencies.Count()} unresolved dependencies", "ErrorUnresolvedDependencies")
        { }

        #endregion

        #region Logic

        /// <summary>
        /// Convert exception data to an object array that can be used via <see cref="string.Format(string, object[])"/>
        /// for localization purposes.
        /// </summary>
        /// <returns> The exception's format items for localization or null. </returns>
        public override object[] GetFormatItems()
        {
            return new[] { InnerExceptions };
        }

        /// <summary>
        /// Convert this instance to a human readable string representation.
        /// </summary>
        /// <returns> A human readable string representation of this instance. </returns>
        public override string ToString()
        {
            if (InnerExceptions?.Count() == 1)
            {
                return "Service container has 1 unresolved dependency";
            }

            return $"Service container has {InnerExceptions?.Count() ?? 0} unresolved dependency";
        }

        #endregion
    }
}