namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// Use a <see cref="FactoryConstructorAttribute"/> to register a function factory for a specific type
    /// that can be used to pass a specified number of constructor parameters via one of the
    /// <see cref="ServiceContainer"/>'s factory method overloads.
    /// Note that the constructor parameters are specified by (comma-seperated) index.
    /// Note also that parameters that are not specified are resolved using the <see cref="ServiceContainer"/> itself.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypePartialFactory : ServiceContainerTestCase
    {
        public interface IBar
        { }

        [Export]
        public sealed class Bar : IBar
        { }

        public interface IFoo
        {
            int Id { get; }

            IBar Bar { get; }
        }

        [Export]
        public sealed class Foo : IFoo
        {
            [FactoryConstructor(0)]
            public Foo(int id, IBar bar)
            {
                Id = id;
                Bar = bar;
            }

            public int Id { get; }

            public IBar Bar { get; }
        }

        [Fact(DisplayName = "Register type with partial factory")]
        public void RegisterTypePartialFactorySucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<int, IFoo>(42))
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                    foo.Bar.ShouldNot().BeNull();
                    foo.Bar.Should().BeOfType<Bar>();
                    foo.Id.Should().Be(42);
                });
        }

        [Fact(DisplayName = "Inject type with partial factory")]
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
                    foo.Bar.ShouldNot().BeNull();
                    foo.Bar.Should().BeOfType<Bar>();
                    foo.Id.Should().Be(42);
                });
        }
    }
}