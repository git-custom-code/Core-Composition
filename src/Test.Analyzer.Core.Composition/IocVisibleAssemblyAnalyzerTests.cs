namespace CustomCode.Analyzer.Core.Composition.Tests
{
    using CustomCode.Core.Composition;
    using System.Linq;
    using Test.BehaviorDrivenDevelopment;
    using Test.BehaviorDrivenDevelopment.Analyzer;
    using Xunit;

    /// <summary>
    /// Automated tests for the <see cref="IocVisibleAssemblyAnalyzer"/> type.
    /// </summary>
    [UnitTest]
    [Category("Analyzer")]
    public sealed class IocVisibleAssemblyAnalyzerTests : AnalyzerTestCase
    {
        [Fact(DisplayName = "Analyze type with export- but without ioc visible assembly attribute")]
        public void AnalyzeTypeWithExportAttributeButWithoutIocVisibleAssemblyAttribute()
        {
            Given<IocVisibleAssemblyAnalyzer>()
            .UsingAssembly<IocVisibleAssemblyAttribute>()
            .With(@"
                using CustomCode.Core.Composition;

                [Export]
                public sealed class MyClass
                {
                }", "MyClass.cs")
            .WhenAnalyzed()
            .Then(diagnostics =>
                {
                    diagnostics.Should().HaveCountOf(1);

                    var id = diagnostics.First().Id;
                    id.Should().Be("CC001");
                    var severity = diagnostics.First().Severity;
                    severity.Should().Be(Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
                    var title = diagnostics.First().Descriptor.Title.ToString();
                    title.Should().Be("Missing IocVisibleAssembly attribute");
                    var category = diagnostics.First().Descriptor.Category;
                    category.Should().Be("Core.Composition");
                    var message = diagnostics.First().GetMessage();
                    message.Should().Be("Type \"MyClass\" is marked for export but no IocVisibleAssembly attribute was defined in assembly \"TestProject\"");
                });
        }

        [Fact(DisplayName = "Analyze type with export- and ioc visible assembly attribute")]
        public void AnalyzeTypeWithExportAttributeAndIocVisibleAssemblyAttribute()
        {
            Given<IocVisibleAssemblyAnalyzer>()
            .UsingAssembly<IocVisibleAssemblyAttribute>()
            .With(@"
                using CustomCode.Core.Composition;

                [assembly: IocVisibleAssembly]

                [Export]
                public sealed class MyClass
                {
                }", "MyClass.cs")
            .WhenAnalyzed()
            .Then(diagnostics =>
                {
                    diagnostics.Should().BeEmpty();
                });
        }
    }
}