namespace CustomCode.Core.Composition.SourceGenerator
{
    /// <summary>
    /// Data transfer object that contains the type metadata of detected services by the <see cref="ExportedServiceWalker"/>.
    /// </summary>
    public sealed class ExportedService
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="ExportedService"/> type.
        /// </summary>
        /// <param name="serviceName"> The (type-)name of the exported service. </param>
        /// <param name="serviceNamespace"> The namespace of the exported service. </param>
        /// <param name="implementationName"> The (type-)name of the exported service's implementation. </param>
        /// <param name="implementationNamespace"> The namespace of the exported service's implementation. </param>
        /// <param name="lifetime"> The service's lifetime (null for Lifetime.Transient). </param>
        /// <param name="serviceId"> The service's optional id. </param>
        public ExportedService(
            string serviceName,
            string serviceNamespace,
            string implementationName,
            string implementationNamespace,
            string? lifetime,
            string? serviceId)
        {
            ServiceName = serviceName;
            ServiceNamespace = serviceNamespace;
            ImplementationName = implementationName;
            ImplementationNamespace = implementationNamespace;
            Lifetime = lifetime;
            ServiceId = serviceId;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the (type-)name of the exported service's implementation.
        /// </summary>
        public string ImplementationName { get; }

        /// <summary>
        /// Gets the namespace of the exported service's implementation.
        /// </summary>
        public string ImplementationNamespace { get; }

        /// <summary>
        /// Gets the service's lifetime (null for Lifetime.Transient).
        /// </summary>
        public string? Lifetime { get; }

        /// <summary>
        /// Gets the service's optional id.
        /// </summary>
        public string? ServiceId { get; }

        /// <summary>
        /// Gets the (type-)name of the exported service.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the namespace of the exported service.
        /// </summary>
        public string ServiceNamespace { get; }

        #endregion
    }
}
