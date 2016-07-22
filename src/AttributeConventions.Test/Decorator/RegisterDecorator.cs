namespace LightInject.AttributeConventions.Test
{
    using Xunit;

    /// <summary>
    /// If your decorator implements multiple interfaces or you just want to be more specific you can
    /// use the <see cref="DecoratorAttribute.DecoratedServiceType"/> to explicitely specify the decorated type.
    /// </summary>
    public sealed class RegisterDecorator
    {
        public interface IFoo
        { }

        public interface IBar
        { }

        [Export]
        public sealed class Foo : IFoo
        { }

        [Decorator(typeof(IFoo))]
        public sealed class FooDecorator : IFoo, IBar
        {
            public FooDecorator(IFoo foo)
            {
                DecoratedFoo = foo;
            }

            public IFoo DecoratedFoo { get; }
        }


        [Fact(DisplayName = "Register decorator")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterDecoratorSucccess()
        {
            // Given
            var rootDir = typeof(RegisterDecorator).Assembly.Location;
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