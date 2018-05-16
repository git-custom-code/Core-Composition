namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type is registerd with <see cref="Lifetime.Transient"/> a new instance is created
    /// every time <see cref="IServiceFactory.GetInstance{TService}"/> is called.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeAsTransient : ServiceContainerTestCase
    {
        [Export(Lifetime.Transient)]
        public sealed class Foo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact(DisplayName = "Register type as transient")]
        public void RegisterTypeAsTransientSucccess()
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

                    foo.Id.ShouldNot().Be(otherFoo.Id);
                });
        }
    }
}