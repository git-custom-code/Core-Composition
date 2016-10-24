namespace LightInject.AttributeConventions.Test
{
    using Xunit;

    /// <summary>
    /// If two or more implementations of the same interface are register with different service names, you can
    /// use the <see cref="ImportAttribute"/> to get a specific instance by name.
    /// </summary>
    public sealed class ImportTypeByServiceName
    {
        public interface IFoo
        { }

        [Export(ServiceName = "Foo1")]
        public sealed class Foo1 : IFoo
        { }

        [Export(ServiceName = "Foo2")]
        public sealed class Foo2 : IFoo
        { }

        [Export]
        public sealed class Bar
        {
            public Bar([Import("Foo1")] IFoo foo)
            {
                Foo = foo;
            }

            public IFoo Foo { get; }
        }

        [Fact(DisplayName = "Import type by service name")]
        [Trait("Category", "IntegrationTest")]
        public void ImportTypeByServiceNameSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithServiceName).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var bar = iocContainer.GetInstance<Bar>();

            // Then
            Assert.NotNull(bar);
            Assert.NotNull(bar.Foo);
            Assert.IsType<Foo1>(bar.Foo);
        }
    }
}