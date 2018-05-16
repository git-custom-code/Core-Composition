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
    public sealed class Test1 : ServiceContainerTestCase
    {
        public interface IBar
        { }

        public interface IFoo
        { }

        public interface IGenericFoo<T>
        { }

        //[Export]
        public sealed class Foo : IFoo
        {
            public Foo(IBar bar)
            { }
        }

        [Export(typeof(IGenericFoo<>))]
        public sealed class GenericFoo<T> : IGenericFoo<T>
        {
            public GenericFoo(IBar bar)
            { }
        }

        [Fact(DisplayName = "Register type with single interface not by implementation")]
        public void Test()
        {
            try
            {
                var container = NewServiceContainer();
                container.VerifyAllDependencies();
            }
            catch(Exception e)
            {

            }
        }
    }
}