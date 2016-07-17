namespace LightInject.AttributeConventions.Test
{
    using System;
    using Xunit;

    /// <summary>
    /// If the type implements multiple interfaces and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd multiple times (once for each interface with the interface as service contract),
    /// if -and only if- all interface names differ from the type name.
    /// </summary>
    public sealed class RegisterTypeWithMultipleInterfaces
    {
        public interface IFoo
        { }

        public interface IBar
        { }

        [Export]
        public sealed class FooBar : IFoo, IBar
        { }

        [Fact(DisplayName = "Register type with multiple interfaces")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeWithMultipleInterfacesSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithMultipleInterfaces).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo1 = iocContainer.GetInstance<IFoo>();
            var foo2 = iocContainer.GetInstance<IBar>();

            // Then
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.IsType<FooBar>(foo1);
            Assert.IsType<FooBar>(foo2);
            Assert.Throws<InvalidOperationException>(() => iocContainer.GetInstance<FooBar>());
        }
    }
}