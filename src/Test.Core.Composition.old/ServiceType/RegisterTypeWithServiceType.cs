namespace CustomCode.Core.Composition.Test
{
    using LightInject;
    using System;
    using Xunit;

    /// <summary>
    /// If you can't or don't want to use the <see cref="ExportAttribute"/>'s default conventions, you can
    /// use the <see cref="ExportAttribute.ServiceType"/> property to specify the type of the service contract
    /// explicitely. You can even use multiple attributes, if you need to specify multiple service contracts.
    /// </summary>
    public sealed class RegisterTypeWithServiceType
    {
        public interface IFoo
        { }

        public interface IBar
        { }

        [Export(typeof(IBar))]
        public sealed class Foo : IFoo, IBar
        { }

        [Fact(DisplayName = "Register type with explicite service type")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeWithServiceTypeSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithServiceType).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<IBar>();

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.Throws<InvalidOperationException>(() => iocContainer.GetInstance<IFoo>());
            Assert.Throws<InvalidOperationException>(() => iocContainer.GetInstance<Foo>());
        }
    }
}