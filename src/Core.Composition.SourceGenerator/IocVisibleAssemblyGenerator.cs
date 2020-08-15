namespace CustomCode.Core.Composition.SourceGenerator
{
    using Extensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Text;

    /// <summary>
    /// Implementation of an <see cref="ISourceGenerator"/> that is used to generate the [assembly: IocVisibleAssembly]
    /// assembly level attribute (but only if "Core.Composition" assembly is referenced).
    /// </summary>
    /// <example>
    /// This SourceGenerator will generate the following code:
    /// <![CDATA[
    /// using CustomCode.Core.Composition;
    ///
    /// [assembly: IocVisibleAssembly]
    /// ]]>
    /// </example>
    [Generator]
    public sealed class IocVisibleAssemblyGenerator : ISourceGenerator
    {
        #region Logic

        /// <inheritdoc />
        public void Execute(SourceGeneratorContext context)
        {
            try
            {
                if (context.Compilation.IsCoreCompositonReferenced())
                {
                    var code = new StringBuilder();
                    code.AppendLine("using CustomCode.Core.Composition;");
                    code.AppendLine();
                    code.AppendLine("[assembly: IocVisibleAssembly]");

                    context.AddSource(
                        Guid.NewGuid().ToString(),
                        SourceText.From(code.ToString(), Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "CCSG001",
                        title: "Can't create IocVisibleAssembly attribute",
                        messageFormat: "IocVisibleAssemblyGenerator: {0}",
                        category: "Core.Composition.SourceGenerator",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        description: "There was an unexpected exception creating the IocVisibleAssembly attribute"),
                    Location.None,
                    e);
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <inheritdoc />
        public void Initialize(InitializationContext context)
        {
            // No initialization required for this generator
        }

        #endregion
    }
}
