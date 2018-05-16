namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If your decorator implements multiple interfaces or you just want to be more specific you can
    /// use the <see cref="DecoratorAttribute.DecoratedServiceType"/> to explicitely specify the decorated type.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterDecorator : ServiceContainerTestCase
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
        public void RegisterDecoratorSucccess()
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