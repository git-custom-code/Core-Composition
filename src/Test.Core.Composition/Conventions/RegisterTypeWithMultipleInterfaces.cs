namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If the type implements multiple interfaces and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd multiple times (once for each interface with the interface as service contract),
    /// if -and only if- all interface names differ from the type name.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeWithMultipleInterfaces : ServiceContainerTestCase
    {
        public interface IFoo
        { }

        public interface IBar
        { }

        [Export]
        public sealed class FooBar : IFoo, IBar
        { }
        
        [Fact(DisplayName = "Register type with multiple interfaces with first interface")]
        public void RegisterTypeWithMultipleInterfacesWithFirstSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IFoo>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<FooBar>();
                });
        }

        [Fact(DisplayName = "Register type with multiple interfaces with second interface")]
        public void RegisterTypeWithMultipleInterfacesWithSecondSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IBar>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<FooBar>();
                });
        }

        [Fact(DisplayName = "Register type with multiple interfaces not by implementation")]
        public void RegisterTypeWithMultipleInterfacesNotByImplementationViolated()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<FooBar>())
            .ThenThrow<InvalidOperationException>();
        }
    }
}