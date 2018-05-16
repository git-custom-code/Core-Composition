namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type implements multiple interfaces and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd only once, if -and only if- one interface has the same name as the type (ignoring a leading "I")
    /// using the interface as service contract.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeWithSameNamedInterface : ServiceContainerTestCase
    {
        public interface IBar
        { }

        public interface IFoo
        { }

        [Export]
        public sealed class Foo : IFoo, IBar
        { }

        [Fact(DisplayName = "Register type with same-named interface")]
        public void RegisterTypeWithSameNamedInterfaceSuccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer =>iocContainer.GetInstance<IFoo>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                });
        }

        [Fact(DisplayName = "Register type with same-named interface not by other interface")]
        public void RegisterTypeWithSameNamedInterfaceButNotByOtherViolated()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IBar>())
            .ThenThrow<InvalidOperationException>();
        }

        [Fact(DisplayName = "Register type with same-named interface not by implementation")]
        public void RegisterTypeWithSameNamedInterfaceNotByImplementationViolated()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<Foo>())
            .ThenThrow<InvalidOperationException>();
        }
    }
}