namespace CustomCode.Core.Composition.Test
{
    using LightInject;
    using System;
    using Xunit;

    /// <summary>
    /// If the type is registerd with <see cref="Lifetime.Scoped"/>
    /// - the same instance is returned every time <see cref="IServiceFactory.GetInstance{TService}"/>
    ///   is called inside of the same <see cref="Scope"/>.
    /// - a new instance is returned every time <see cref="IServiceFactory.GetInstance{TService}"/>
    ///   is called in a different <see cref="Scope"/>.
    /// </summary>
    public sealed class RegisterTypeAsScoped
    {
        [Export(Lifetime.Scoped)]
        public sealed class Foo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact(DisplayName = "Register type as scoped")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeAsScopedSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeAsScoped).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);
            Foo foo1, foo2, foo3;

            // When
            using (var scope1 = iocContainer.BeginScope())
            {
                foo1 = iocContainer.GetInstance<Foo>();
                foo2 = iocContainer.GetInstance<Foo>();
            }
            using (var scope2 = iocContainer.BeginScope())
            {
                foo3 = iocContainer.GetInstance<Foo>();
            }

            // Then
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.NotNull(foo3);
            Assert.IsType<Foo>(foo1);
            Assert.IsType<Foo>(foo2);
            Assert.IsType<Foo>(foo3);
            Assert.Equal(foo1.Id, foo2.Id);
            Assert.NotEqual(foo1.Id, foo3.Id);
        }
    }
}