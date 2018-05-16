namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If you follow the conventional pattern for implementing a decorator (i.e. "subclass" the original
    /// type and pass a single type instance to the decorator's constructor) you can use a plain
    /// <see cref="DecoratorAttribute"/> at the class level to register a decorator at the
    /// <see cref="ServiceContainer"/>.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterDecoratorUsingConventions : ServiceContainerTestCase
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
        public void RegisterDecoratorUsingConventionsSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IFoo>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<FooDecorator>();
                    ((FooDecorator)foo).DecoratedFoo.Should().BeOfType<Foo>();
                });
        }
    }
}