namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type is registerd with <see cref="Lifetime.Singleton"/> the same instance is returned
    /// every time <see cref="IServiceFactory.GetInstance{TService}"/> is called.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeAsSingelton : ServiceContainerTestCase
    {
        [Export(Lifetime.Singleton)]
        public sealed class Foo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact(DisplayName = "Register type as singleton")]
        public void RegisterTypeAsSingeltonSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer =>
                {
                    var foo = iocContainer.GetInstance<Foo>();
                    var otherFoo = iocContainer.GetInstance<Foo>();
                    return (foo, otherFoo);
                })
            .Then(result =>
                {
                    var (foo, otherFoo) = result;

                    foo.ShouldNot().BeNull();
                    otherFoo.ShouldNot().BeNull();

                    foo.Should().BeOfType<Foo>();
                    otherFoo.Should().BeOfType<Foo>();

                    foo.Id.Should().Be(otherFoo.Id);
                });
        }
    }
}