namespace CustomCode.Core.Composition.Tests
{
    using LightInject;

    /// <summary>
    /// Base type for <see cref="ServiceContainer"/> related tests.
    /// </summary>
    public abstract class ServiceContainerTestCase : Test.BehaviorDrivenDevelopment.TestCase
    {
        #region Logic

        /// <summary>
        /// Create a new <see cref="ServiceContainer"/> instance with the
        /// types defined within this assembly registered.
        /// </summary>
        /// <returns> The <see cref="ServiceContainer"/> to be tested. </returns>
        protected ServiceContainer NewServiceContainer()
        {
            var rootDir = GetType().Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);
            return iocContainer;
        }

        #endregion
    }
}