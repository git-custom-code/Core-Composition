namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// Register two or more implementations of the same interface with a service name to be able to
    /// get a specific instance via <see cref="IServiceFactory.GetInstance{TService}(string)"/>.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeWithServiceName : ServiceContainerTestCase
    {
        public interface IFoo
        { }

        [Export(ServiceName = "Foo1")]
        public sealed class Foo1 : IFoo
        { }

        [Export(ServiceName = "Foo2")]
        public sealed class Foo2 : IFoo
        { }

        [Fact(DisplayName = "Register type with service name")]
        public void RegisterTypeWithServiceNameSucccess()
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
                    foo2.ShouldNot().BeNull();

                    foo1.Should().BeOfType<Foo1>();
                    foo2.Should().BeOfType<Foo2>();
                });
        }
    }
}