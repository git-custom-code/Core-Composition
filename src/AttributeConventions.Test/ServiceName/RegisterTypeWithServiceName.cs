namespace LightInject.AttributeConventions.Test
{
    using Xunit;

    /// <summary>
    /// Register two or more implementations of the same interface with a service name to be able to
    /// get a specific instance via <see cref="IServiceFactory.GetInstance{TService}(string)"/>.
    /// </summary>
    public sealed class RegisterTypeWithServiceName
    {
        public interface IFoo
        { }

        [Export(ServiceName = "Foo1")]
        public sealed class Foo1 : IFoo
        { }

        [Export(ServiceName = "Foo2")]
        public sealed class Foo2 : IFoo
        { }

        [Fact(DisplayName = "Register type with service name")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeWithServiceNameSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithServiceName).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo1 = iocContainer.GetInstance<IFoo>("Foo1");
            var foo2 = iocContainer.GetInstance<IFoo>("Foo2");

            // Then
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.IsType<Foo1>(foo1);
            Assert.IsType<Foo2>(foo2);
        }
    }
}