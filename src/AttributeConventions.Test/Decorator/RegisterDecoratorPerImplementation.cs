namespace LightInject.AttributeConventions.Test
{
    using Xunit;

    /// <summary>
    /// If you have multiple implementations of the same interface but want to register a decorator
    /// only for one specific implementation you can use the <see cref="DecoratedByAttribute"/> in combination
    /// with the <see cref="ExportAttribute"/>.
    /// </summary>
    public sealed class RegisterDecoratorPerImplementation
    {
        public interface IFoo
        { }

        [Export(ServiceName = "Foo1")]
        public sealed class Foo1 : IFoo
        { }

        [Export(ServiceName = "Foo2")]
        [DecoratedBy(typeof(FooDecorator))]
        public sealed class Foo2 : IFoo
        { }

        public sealed class FooDecorator : IFoo
        {
            public FooDecorator(IFoo foo)
            {
                DecoratedFoo = foo;
            }

            public IFoo DecoratedFoo { get; }
        }


        [Fact(DisplayName = "Register decorator per implementation")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterDecoratorPerImplementationSucccess()
        {
            // Given
            var rootDir = typeof(RegisterDecoratorPerImplementation).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo1 = iocContainer.GetInstance<IFoo>("Foo1");
            var foo2 = iocContainer.GetInstance<IFoo>("Foo2");

            // Then
            Assert.NotNull(foo1);
            Assert.IsType<Foo1>(foo1);

            Assert.NotNull(foo2);
            Assert.IsType<FooDecorator>(foo2);
            Assert.IsType<Foo2>(((FooDecorator)foo2).DecoratedFoo);
        }
    }
}