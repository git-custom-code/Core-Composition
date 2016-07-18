namespace LightInject.AttributeConventions.Test
{
    using System;
    using Xunit;

    /// <summary>
    /// Use a <see cref="FactoryConstructorAttribute"/> to register a function factory for a specific type
    /// that can be used to pass all constructor parameters via one of the <see cref="ServiceContainer"/>'s
    /// factory method overloads.
    /// </summary>
    public sealed class RegisterTypeFactory
    {
        public interface IFoo
        {
            int Id { get; }
        }

        [Export]
        public sealed class Foo : IFoo
        {
            [FactoryConstructor]
            public Foo(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        [Fact(DisplayName = "Register type factory")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeFactorySucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeFactory).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<int, IFoo>(42);

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.Equal(42, foo.Id);
        }

        [Fact(DisplayName = "Inject type factory")]
        [Trait("Category", "IntegrationTest")]
        public void InjectTypeFactorySucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeFactory).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var fooFactory = iocContainer.GetInstance<Func<int, IFoo>>();
            var foo = fooFactory(42);

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.Equal(42, foo.Id);
        }
    }
}