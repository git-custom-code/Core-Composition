namespace CustomCode.Core.Composition
{
    /// <summary>
    /// Used in combination with the <see cref="ExportAttribute"/> to define the lifetime of a registerd service.
    /// </summary>
    public enum Lifetime : byte
    {
        /// <summary> A new instance is created by the <see cref="LightInject.ServiceContainer"/> per request. </summary>
        Transient = 0,
        /// <summary> A new instance is created by the <see cref="LightInject.ServiceContainer"/> only once. </summary>
        Singleton = 1,
        /// <summary> A new instance is created by the <see cref="LightInject.ServiceContainer"/> per scope. </summary>
        Scoped = 2
    }
}