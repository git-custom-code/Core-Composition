namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If two or more implementations of the same interface are register with different service names, you can
    /// use the <see cref="ImportAttribute"/> to get a specific instance by name.
    /// </summary>
    [IntegrationTest]
    public sealed class ImportTypeByServiceName : ServiceContainerTestCase
    {
        public interface IFoo
        { }

        [Export(ServiceName = "Foo1")]
        public sealed class Foo1 : IFoo
        { }

        [Export(ServiceName = "Foo2")]
        public sealed class Foo2 : IFoo
        { }

        [Export]
        public sealed class Bar
        {
            public Bar([Import("Foo1")] IFoo foo)
            {
                Foo = foo;
            }

            public IFoo Foo { get; }
        }

        [Fact(DisplayName = "Import type by service name")]
        public void ImportTypeByServiceNameSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<Bar>())
            .Then(bar =>
                {
                    bar.ShouldNot().BeNull();
                    bar.Foo.ShouldNot().BeNull();
                    bar.Foo.Should().BeOfType<Foo1>();
                });
        }
    }
}