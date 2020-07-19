namespace CustomCode.Core.Composition.SourceGenerator.Tests
{
    using Microsoft.CodeAnalysis;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Automated tests for the <see cref="CompositionRootGenerator"/> type.
    /// </summary>
    public sealed class CompositionRootGeneratorTests : SourceGeneratorTest
    {
        [Fact]
        public void GenerateCompositionRoot()
        {
            // Given
            var generator = new CompositionRootGenerator();

            var source = @"
                namespace Test.Namespace
                {
                    using CustomCode.Core.Composition;

                    public interface IFoo
                    { }

                    public interface IBar
                    { }

                    [Export(typeof(IFoo))]
                    public sealed class Foo : IFoo, IBar
                    { }
                }";

            var compilation = CreateCompilation(source);

            // When
            var generated = ExecuteSourceGenerator(compilation, generator);

            // Then
            Assert.Equal(0, compilation.GetDiagnostics().Count(d => d.Severity == DiagnosticSeverity.Error));
            var diagnostics = generated.compilation.GetDiagnostics();
            Assert.Equal(0, diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error));
        }
    }
}