namespace CustomCode.Core.Composition.SourceGenerator.Tests
{
    using Microsoft.CodeAnalysis;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Automated tests for the <see cref="IocVisibleAssemblyGenerator"/> type.
    /// </summary>
    public sealed class IocVisibleAssemblyGeneratorTests : SourceGeneratorTest
    {
        [Fact]
        public void GenerateIocVisibleAssemblyAttribute()
        {
            // Given
            var generator = new IocVisibleAssemblyGenerator();

            var source = @"
                namespace Test.Namespace
                {
                    using CustomCode.Core.Composition;

                    public interface IFoo
                    { }

                    [Export]
                    public sealed class Foo : IFoo
                    { }
                }";

            var compilation = CreateCompilation(source);

            // When
            var generated = ExecuteSourceGenerator(compilation, generator);

            // Then
            Assert.Equal(0, compilation.GetDiagnostics().Count(d => d.Severity == DiagnosticSeverity.Error));
            Assert.Equal(0, generated.compilation.GetDiagnostics().Count(d => d.Severity == DiagnosticSeverity.Error));
        }
    }
}