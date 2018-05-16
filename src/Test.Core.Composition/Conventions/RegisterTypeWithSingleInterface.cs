namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type implements a single interface and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd once using the interface as service contract (no matter if the interface name differs
    /// from the type name or not).
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeWithSingleInterface : ServiceContainerTestCase
    {
        public interface IBar
        { }

        [Export]
        public sealed class Foo : IBar
        { }

        [Fact(DisplayName = "Register type with single interface")]
        public void RegisterTypeWithSingleInterfaceSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IBar>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                });
        }

        [Fact(DisplayName = "Register type with single interface not by implementation")]
        public void RegisterTypeWithSingleInterfaceNotByImplementationViolated()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<Foo>())
            .ThenThrow<InvalidOperationException>();
        }
    }
}