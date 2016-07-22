namespace LightInject.AttributeConventions.Test
{
    using System;
    using Xunit;

    /// <summary>
    /// Use a <see cref="FactoryConstructorAttribute"/> to register a function factory for a specific type
    /// that can be used to pass a specified number of constructor parameters via one of the
    /// <see cref="ServiceContainer"/>'s factory method overloads.
    /// Note that the constructor parameters are specified by (comma-seperated) index.
    /// Note also that parameters that are not specified are resolved using the <see cref="ServiceContainer"/> itself.
    /// </summary>
    public sealed class RegisterTypePartialFactory
    {
        public interface IBar
        { }

        [Export]
        public sealed class Bar : IBar
        { }

        public interface IFoo
        {
            int Id { get; }

            IBar Bar { get; }
        }

        [Export]
        public sealed class Foo : IFoo
        {
            [FactoryConstructor(0)]
            public Foo(int id, IBar bar)
            {
                Id = id;
                Bar = bar;
            }

            public int Id { get; }

            public IBar Bar { get; }
        }

        [Fact(DisplayName = "Register type with partial factory")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypePartialFactorySucccess()
        {

            // Given
            var rootDir = typeof(RegisterTypePartialFactory).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<int, IFoo>(42);

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.NotNull(foo.Bar);
            Assert.IsType<Bar>(foo.Bar);
            Assert.Equal(42, foo.Id);
        }

        [Fact(DisplayName = "Inject type with partial factory")]
        [Trait("Category", "IntegrationTest")]
        public void InjectTypeFactorySucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypePartialFactory).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var fooFactory = iocContainer.GetInstance<Func<int, IFoo>>();
            var foo = fooFactory(42);

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.NotNull(foo.Bar);
            Assert.IsType<Bar>(foo.Bar);
            Assert.Equal(42, foo.Id);
        }
    }
}