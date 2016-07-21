namespace LightInject.AttributeConventions.Test
{
    using Xunit;

    /// <summary>
    /// If you follow the conventional pattern for implementing a decorator (i.e. "subclass" the original
    /// type and pass a single type instance to the decorator's constructor) you can use a plain
    /// <see cref="DecoratorAttribute"/> at the class level to register a decorator at the
    /// <see cref="ServiceContainer"/>.
    /// </summary>
    public sealed class RegisterDecoratorUsingConventions
    {
        public interface IFoo
        { }

        [Export]
        public sealed class Foo : IFoo
        { }
        
        [Decorator]
        public sealed class FooDecorator : IFoo
        {
            public FooDecorator(IFoo foo)
            {
                DecoratedFoo = foo;
            }

            public IFoo DecoratedFoo { get; }
        }


        [Fact(DisplayName = "Register decorator using conventions")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterDecoratorUsingConventionsSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeFactory).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<IFoo>();

            // Then
            Assert.NotNull(foo);
            Assert.IsType<FooDecorator>(foo);
            Assert.IsType<Foo>(((FooDecorator)foo).DecoratedFoo);
        }
    }
}