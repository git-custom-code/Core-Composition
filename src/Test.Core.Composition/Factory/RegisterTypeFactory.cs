namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// Use a <see cref="FactoryConstructorAttribute"/> to register a function factory for a specific type
    /// that can be used to pass all constructor parameters via one of the <see cref="ServiceContainer"/>'s
    /// factory method overloads.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeFactory : ServiceContainerTestCase
    {
        public interface IFoo
        {
            int Id { get; }
        }

        [Export]
        public sealed class Foo : IFoo
        {
            [FactoryConstructor]
            public Foo(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        [Fact(DisplayName = "Register type factory")]
        public void RegisterTypeFactorySucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<int, IFoo>(42))
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                    foo.Id.Should().Be(42);
                });
        }

        [Fact(DisplayName = "Inject type factory")]
        public void InjectTypeFactorySucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer =>
                {
                    var fooFactory = iocContainer.GetInstance<Func<int, IFoo>>();
                    return fooFactory(42);
                })
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                    foo.Id.Should().Be(42);
                });
        }
    }
}