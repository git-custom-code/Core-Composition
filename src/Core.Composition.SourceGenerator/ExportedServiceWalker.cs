namespace CustomCode.Core.Composition.SourceGenerator
{
    using Extensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implementation of a <see cref="CSharpSyntaxWalker"/> that will detect all types that
    /// are marked with an "Export" attribute.
    /// </summary>
    public sealed class ExportedServiceWalker : CSharpSyntaxWalker
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="ExportedServiceWalker"/> type.
        /// </summary>
        /// <param name="compilation">
        /// The roslyn <see cref="Microsoft.CodeAnalysis.Compilation"/> of the analyzed c# assembly.
        /// </param>
        /// <param name="syntaxTree">
        /// The roslyn <see cref="Microsoft.CodeAnalysis.SyntaxTree"/> representation of the analyzed c# code.
        /// </param>
        public ExportedServiceWalker(Compilation compilation, SyntaxTree syntaxTree)
        {
            Compilation = compilation;
            SyntaxTree = syntaxTree;
        }

        /// <summary>
        /// Gets the roslyn <see cref="Microsoft.CodeAnalysis.Compilation"/> of the analyzed c# assembly.
        /// </summary>
        private Compilation Compilation { get; }

        /// <summary>
        /// Gets the roslyn <see cref="Microsoft.CodeAnalysis.SyntaxTree"/> representation of the analyzed c# code.
        /// </summary>
        private SyntaxTree SyntaxTree { get; }

        #endregion

        #region Data

        /// <summary>
        /// Gets a collection of detected exported services.
        /// </summary>
        public IList<ExportedService> DetectedServices { get; private set; } = new List<ExportedService>();

        #endregion

        #region Logic

        /// <inheritdoc />
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            try
            {
                var attributes = node.AttributeLists.SelectMany(l => l.Attributes);
                if (attributes.Any(a => a.Name.ToString().StartsWith(Constants.ExportAttribute, StringComparison.OrdinalIgnoreCase)))
                {
                    var semanticModel = Compilation.GetSemanticModel(SyntaxTree);
                    foreach (var exportAttribute in attributes.Where(a => a.Name.ToString() == Constants.ExportAttribute))
                    {
                        var exportAttributeSymbol = semanticModel.GetSymbolInfo(exportAttribute);
                        if (exportAttributeSymbol.Symbol?.ContainingNamespace?.ToString() == Constants.CoreCompositionNamespace)
                        {
                            var lifetime = exportAttribute.ParseLifetime();
                            var serviceName = exportAttribute.ParseServiceName();
                            var typeSymbol = semanticModel.GetDeclaredSymbol(node);
                            if (typeSymbol == null)
                            {
                                // ToDo
                                return;
                            }

                            var interfaces = typeSymbol.AllInterfaces;
                            if (interfaces.Any())
                            {
                                var serviceType = exportAttribute.ParseServiceType();
                                if (serviceType == null)
                                {
                                    foreach (var @interface in interfaces)
                                    {
                                        DetectedServices.Add(new ExportedService(
                                            @interface.Name,
                                            @interface.ContainingNamespace.ToString(),
                                            typeSymbol.Name,
                                            typeSymbol.ContainingNamespace.ToString(),
                                            lifetime,
                                            serviceName));
                                    }
                                }
                                else
                                {
                                    var @interface = interfaces.Single(i => i.Name == serviceType);
                                    DetectedServices.Add(new ExportedService(
                                        @interface.Name,
                                        @interface.ContainingNamespace.ToString(),
                                        typeSymbol.Name,
                                        typeSymbol.ContainingNamespace.ToString(),
                                        lifetime,
                                        serviceName));
                                }
                            }
                            else
                            {
                                DetectedServices.Add(new ExportedService(
                                    typeSymbol.Name,
                                    typeSymbol.ContainingNamespace.ToString(),
                                    typeSymbol.Name,
                                    typeSymbol.ContainingNamespace.ToString(),
                                    lifetime,
                                    serviceName));
                            }

                            return;
                        }
                    }
                }
            }
            finally
            {
                base.VisitClassDeclaration(node);
            }
        }

        #endregion
    }
}
