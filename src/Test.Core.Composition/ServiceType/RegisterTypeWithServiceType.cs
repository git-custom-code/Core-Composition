namespace CustomCode.Core.Composition.Tests
{
    using LightInject;
    using System;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// If you can't or don't want to use the <see cref="ExportAttribute"/>'s default conventions, you can
    /// use the <see cref="ExportAttribute.ServiceType"/> property to specify the type of the service contract
    /// explicitely. You can even use multiple attributes, if you need to specify multiple service contracts.
    /// </summary>
    [IntegrationTest]
    public sealed class RegisterTypeWithServiceType : ServiceContainerTestCase
    {
        public interface IFoo
        { }

        public interface IBar
        { }

        [Export(typeof(IBar))]
        public sealed class Foo : IFoo, IBar
        { }

        [Fact(DisplayName = "Register type with explicite service type")]
        public void RegisterTypeWithServiceTypeSucccess()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IBar>())
            .Then(foo =>
                {
                    foo.ShouldNot().BeNull();
                    foo.Should().BeOfType<Foo>();
                });
        }

        [Fact(DisplayName = "Register type with explicite service type but not other interface")]
        public void RegisterTypeWithServiceTypeButNotWithOtherViolated()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<IFoo>())
            .ThenThrow<InvalidOperationException>();
        }

        [Fact(DisplayName = "Register type with explicite service type but not implentation")]
        public void RegisterTypeWithServiceTypeNotImplementationViolated()
        {
            Given(() => NewServiceContainer())
            .When(iocContainer => iocContainer.GetInstance<Foo>())
            .ThenThrow<InvalidOperationException>();
        }
    }
}