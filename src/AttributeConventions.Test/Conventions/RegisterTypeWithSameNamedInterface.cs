namespace LightInject.AttributeConventions.Test
{
    using System;
    using Xunit;

    /// <summary>
    /// If the type implements multiple interfaces and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd only once, if -and only if- one interface has the same name as the type (ignoring a leading "I")
    /// using the interface as service contract.
    /// </summary>
    public sealed class RegisterTypeWithSameNamedInterface
    {
        public interface IBar
        { }

        public interface IFoo
        { }

        [Export]
        public sealed class Foo : IFoo, IBar
        { }

        [Fact(DisplayName = "Register type with same-named interface")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeWithSameNamedInterfaceSuccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithSameNamedInterface).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<IFoo>();

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.Throws<InvalidOperationException>(() => iocContainer.GetInstance<IBar>());
            Assert.Throws<InvalidOperationException>(() => iocContainer.GetInstance<Foo>());
        }
    }
}