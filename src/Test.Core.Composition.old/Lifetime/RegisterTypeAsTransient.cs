namespace CustomCode.Core.Composition.Test
{
    using LightInject;
    using System;
    using Xunit;

    /// <summary>
    /// If the type is registerd with <see cref="Lifetime.Transient"/> a new instance is created
    /// every time <see cref="IServiceFactory.GetInstance{TService}"/> is called.
    /// </summary>
    public sealed class RegisterTypeAsTransient
    {
        [Export(Lifetime.Transient)]
        public sealed class Foo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact(DisplayName = "Register type as transient")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeAsTransientSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeAsTransient).Assembly.Location;
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
            Assert.NotEqual(foo1.Id, foo2.Id);
        }
    }
}