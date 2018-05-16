namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type implements no interfaces and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd once using the type as service contract.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeWithoutInterface : ServiceContainerTestCase
    {
        [Export]
        public sealed class Foo
        { }

        [Fact(DisplayName = "Register type without intereface")]
        public void RegisterTypeWithoutInterfaceSuccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<Foo>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                });
        }
    }
}