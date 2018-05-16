namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type is registerd with <see cref="Lifetime.Scoped"/>
    /// - the same instance is returned every time <see cref="IServiceFactory.GetInstance{TService}"/>
    ///   is called inside of the same <see cref="Scope"/>.
    /// - a new instance is returned every time <see cref="IServiceFactory.GetInstance{TService}"/>
    ///   is called in a different <see cref="Scope"/>.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeAsScoped : ServiceContainerTestCase
    {
        [Export(Lifetime.Scoped)]
        public sealed class Foo
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact(DisplayName = "Register type as scoped")]
        public void RegisterTypeAsScopedSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer =>
                {
                    Foo foo, sameScopeFoo, otherScopeFoo;
                    using (var scope1 = iocContainer.BeginScope())
                    {
                        foo = iocContainer.GetInstance<Foo>();
                        sameScopeFoo = iocContainer.GetInstance<Foo>();
                    }
                    using (var scope2 = iocContainer.BeginScope())
                    {
                        otherScopeFoo = iocContainer.GetInstance<Foo>();
                    }
                    return (foo, sameScopeFoo, otherScopeFoo);
                })
            .Then(result =>
                {
                    var (foo, sameScopeFoo, otherScopeFoo) = result;

                    foo.ShouldNot().BeNull();
                    sameScopeFoo.ShouldNot().BeNull();
                    otherScopeFoo.ShouldNot().BeNull();

                    foo.Should().BeOfType<Foo>();
                    sameScopeFoo.Should().BeOfType<Foo>();
                    otherScopeFoo.Should().BeOfType<Foo>();

                    foo.Id.Should().Be(sameScopeFoo.Id);
                    foo.Id.ShouldNot().Be(otherScopeFoo.Id);
                });
        }
    }
}