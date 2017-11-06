namespace CustomCode.Core.Composition.Test
{
    using LightInject;
    using System;
    using Xunit;

    /// <summary>
    /// If the type is registerd with <see cref="Lifetime.Singleton"/> the same instance is returned
    /// every time <see cref="IServiceFactory.GetInstance{TService}"/> is called.
    /// </summary>
    public sealed class RegisterTypeAsSingelton
    {
        [Export(Lifetime.Singleton)]
        public sealed class Foo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact(DisplayName = "Register type as singleton")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeAsSingeltonSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeAsSingelton).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo1 = iocContainer.GetInstance<Foo>();
            var foo2 = iocContainer.GetInstance<Foo>();

            // Then
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.IsType<Foo>(foo1);
            Assert.IsType<Foo>(foo2);
            Assert.Equal(foo1.Id, foo2.Id);
        }
    }
}