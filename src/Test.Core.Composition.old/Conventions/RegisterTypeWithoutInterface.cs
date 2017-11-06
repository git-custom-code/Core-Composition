namespace CustomCode.Core.Composition.Test
{
    using LightInject;
    using Xunit;

    /// <summary>
    /// If the type implements no interfaces and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd once using the type as service contract.
    /// </summary>
    public sealed class RegisterTypeWithoutInterface
    {
        [Export]
        public sealed class Foo
        { }

        [Fact(DisplayName = "Register type without intereface")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeWithoutInterfaceSuccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithoutInterface).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<Foo>();

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
        }
    }
}