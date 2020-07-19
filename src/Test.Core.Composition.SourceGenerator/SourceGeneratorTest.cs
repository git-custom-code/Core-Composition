namespace CustomCode.Core.Composition.SourceGenerator.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Base class for automated tests that want to execute <see cref="ISourceGenerator"/> instances.
    /// </summary>
    public abstract class SourceGeneratorTest
    {
        #region Logic

        /// <summary>
        /// Create a <see cref="Compilation"/> from the given <paramref name="source"/> code.
        /// </summary>
        /// <param name="source"> The source code that should be compiled. </param>
        /// <returns> The compiled <paramref name="source"/> code. </returns>
        protected Compilation CreateCompilation(string source)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var netStandard = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
            var systemRuntime = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
            var coreComposition = MetadataReference.CreateFromFile(typeof(ExportAttribute).Assembly.Location);

            var options = new CSharpCompilationOptions(
               OutputKind.DynamicallyLinkedLibrary,
               optimizationLevel: OptimizationLevel.Debug);

            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(),
                references: new MetadataReference[] { mscorlib, netStandard, systemRuntime, coreComposition },
                options: options);

            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));
            compilation = compilation.AddSyntaxTrees(syntaxTree);

            return compilation;
        }

        /// <summary>
        /// Execute the given <paramref name="sourceGenerator"/> (under test) on the given <paramref name="compilation"/>.
        /// </summary>
        /// <param name="compilation">
        /// The compiled source code that should be passed to the <paramref name="sourceGenerator"/>.
        /// </param>
        /// <param name="sourceGenerator"> The <see cref="ISourceGenerator"/> under test. </param>
        /// <returns>
        /// The compiled source code modified by the <paramref name="sourceGenerator"/> as well as any diagnostics.
        /// </returns>
        protected (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) ExecuteSourceGenerator(
            Compilation compilation, ISourceGenerator sourceGenerator)
        {
            var generatorDriver = new CSharpGeneratorDriver(
                parseOptions: compilation.SyntaxTrees.First().Options,
                generators: ImmutableArray.Create(sourceGenerator),
                additionalTexts: ImmutableArray<AdditionalText>.Empty);

            generatorDriver.RunFullGeneration(compilation, out var outputCompilation, out var diagnostics);
            return (outputCompilation, diagnostics);
        }

        #endregion
    }
}