namespace CustomCode.Core.Composition.Test
{
    using LightInject;
    using System;
    using Xunit;

    /// <summary>
    /// If the type implements a single interface and is registerd with a plain <see cref="ExportAttribute"/>
    /// it is registerd once using the interface as service contract (no matter if the interface name differs
    /// from the type name or not).
    /// </summary>
    public sealed class RegisterTypeWithSingleInterface
    {
        public interface IBar
        { }

        [Export]
        public sealed class Foo : IBar
        { }

        [Fact(DisplayName = "Register type with single interface")]
        [Trait("Category", "IntegrationTest")]
        public void RegisterTypeWithSingleInterfaceSucccess()
        {
            // Given
            var rootDir = typeof(RegisterTypeWithSingleInterface).Assembly.Location;
            var iocContainer = new ServiceContainer();
            iocContainer.UseAttributeConventions();
            iocContainer.RegisterIocVisibleAssemblies(rootDir);

            // When
            var foo = iocContainer.GetInstance<IBar>();

            // Then
            Assert.NotNull(foo);
            Assert.IsType<Foo>(foo);
            Assert.Throws<InvalidOperationException>(() => iocContainer.GetInstance<Foo>());
        }
    }
}