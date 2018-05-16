namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If you have multiple implementations of the same interface but want to register a decorator
    /// only for one specific implementation you can use the <see cref="DecoratedByAttribute"/> in combination
    /// with the <see cref="ExportAttribute"/>.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterDecoratorPerImplementation : ServiceContainerTestCase
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
        public void RegisterDecoratorPerImplementationSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer =>
                {
                    var foo1 = iocContainer.GetInstance<IFoo>("Foo1");
                    var foo2 = iocContainer.GetInstance<IFoo>("Foo2");
                    return (foo1, foo2);
                })
            .Then(result =>
                {
                    var (foo1, foo2) = result;

                    foo1.ShouldNot().BeNull();
                    foo1.Should().BeOfType<Foo1>();

                    foo2.ShouldNot().BeNull();
                    foo2.Should().BeOfType<FooDecorator>();
                    ((FooDecorator)foo2).DecoratedFoo.Should().BeOfType<Foo2>();
                });
        }
    }
}