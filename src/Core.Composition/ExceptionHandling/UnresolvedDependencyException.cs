namespace CustomCode.Core.Composition.ExceptionHandling
{
    using Core.ExceptionHandling;
    using LightInject;
    using System;

    /// <summary>
    /// Exception that is thrown when a <see cref="ServiceContainer"/> couldn't create at least
    /// one registered service instance because of an unresolved dependency.
    /// </summary>
    public sealed class UnresolvedDependencyException : TechnicalException
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="UnresolvedDependencyException"/> type.
        /// </summary>
        /// <param name="dependencyNamespace"> The unresolved dependency's namespace. </param>
        /// <param name="dependencyType"> The unresolved dependency's type name. </param>
        /// <param name="serviceNamespace"> The namespace of the service type, that has an unresolved dependency. </param>
        /// <param name="serviceType"> The name of the service type, that has an unresolved dependency. </param>
        /// <param name="exception"> The original (lightinject) exception that has caused this exception. </param>
        public UnresolvedDependencyException(
            string dependencyNamespace,
            string dependencyType,
            string serviceNamespace,
            string serviceType,
            InvalidOperationException exception)
            : base(exception, $"Service \"{serviceNamespace}.{serviceType}\" has unresolved dependency \"{dependencyNamespace}.{dependencyType}\"", "ErrorUnresolvedDependency")
        {
            DependencyNamespace = dependencyNamespace;
            DependencyType = dependencyType;
            ServiceNamespace = serviceNamespace;
            ServiceType = serviceType;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the unresolved dependency's namespace.
        /// </summary>
        public string DependencyNamespace { get; }

        /// <summary>
        /// Gets the unresolved dependency's type name.
        /// </summary>
        public string DependencyType { get; }

        /// <summary>
        /// Gets the namespace of the service type, that has an unresolved dependency.
        /// </summary>
        public string ServiceNamespace { get; }

        /// <summary>
        /// Gets the name of the service type, that has an unresolved dependency.
        /// </summary>
        public string ServiceType { get; }

        #endregion

        #region Logic

        /// <summary>
        /// Convert exception data to an object array that can be used via <see cref="string.Format(string, object[])"/>
        /// for localization purposes.
        /// </summary>
        /// <returns> The exception's format items for localization or null. </returns>
        public override object[]? GetFormatItems()
        {
            return new[] { DependencyNamespace, DependencyType, ServiceNamespace, ServiceType };
        }

        /// <summary>
        /// Convert this instance to a human readable string representation.
        /// </summary>
        /// <returns> A human readable string representation of this instance. </returns>
        public override string ToString()
        {
            return Message;
        }

        #endregion
    }
}