namespace CustomCode.Analyzer.Core.Composition
{
    using Extensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;

    /// <summary>
    /// A roslyn <see cref="DiagnosticAnalyzer"/> that checks if an assembly that contains at least one
    /// type with an export attribute has an ioc visible assembly attribute defined.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class IocVisibleAssemblyAnalyzer : DiagnosticAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="IocVisibleAssemblyAnalyzer"/> type.
        /// </summary>
        public IocVisibleAssemblyAnalyzer()
        {
            Rule = new DiagnosticDescriptor(
                id: "CCA0001",
                title: "Missing IocVisibleAssembly attribute",
                messageFormat: "Type \"{0}\" is marked for export but no IocVisibleAssembly attribute was defined in assembly \"{1}\"",
                category: "Core.Composition",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "A type was marked for export but the containing assembly has no IocVisibleAssembly attribute");
            SupportedDiagnostics = ImmutableArray.Create(Rule);
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the diagnostics that this analyzer is capable of producing.
        /// </summary>
        private DiagnosticDescriptor Rule { get; }

        /// <summary>
        /// Gets a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        #endregion

        #region Logic

        /// <summary>
        /// Check if any export attributes are defined and if so if the assembly contains
        /// an ioc visible assembly attribute.
        /// </summary>
        /// <param name="context"> The analysis context that contains the attribute <see cref="SyntaxNode"/>. </param>
        private void AnalyzeExportAttributes(SyntaxNodeAnalysisContext context)
        {
            var attributeNode = (AttributeSyntax)context.Node;
            var classNode = attributeNode.GetParent<ClassDeclarationSyntax>();
            if (classNode == null)
            {
                return;
            }

            var symbol = context.SemanticModel.GetTypeInfo(attributeNode.Name);
            if (symbol.Type?.Name == "ExportAttribute" &&
                symbol.Type.GetNamespace() == Constants.CompositionNamespace)
            {
                if (!HasIocVisibleAssemblyAttribute(context))
                {
                    var classSymbol = context.SemanticModel.GetDeclaredSymbol(classNode);
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        attributeNode.GetLocation(),
                        classSymbol.Name, context.Compilation.AssemblyName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// Check if the analyzed assembly has an ioc visible assembly attribute defined.
        /// </summary>
        /// <param name="context"> The analysis context that contains the syntax tree(s) of the analyzed assembly. </param>
        /// <returns> True if an ioc visible assembly attribute was found, false otherwise. </returns>
        private bool HasIocVisibleAssemblyAttribute(SyntaxNodeAnalysisContext context)
        {
            if (context.Compilation == null)
            {
                return false;
            }

            var attributes = context.Compilation?.Assembly?.GetAttributes();
            if (attributes?.Length > 0)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.Name == "IocVisibleAssemblyAttribute" &&
                        attribute.AttributeClass.GetNamespace() == Constants.CompositionNamespace)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context"> The context used to register analysis actions. </param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeExportAttributes, new[] { SyntaxKind.Attribute });
        }

        #endregion
    }
}