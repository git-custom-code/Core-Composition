namespace CustomCode.Core.Composition.SourceGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
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
                if (attributes.Any(a => a.Name.ToString() == "Export"))
                {
                    var semanticModel = Compilation.GetSemanticModel(SyntaxTree);
                    foreach (var exportAttribute in attributes.Where(a => a.Name.ToString() == "Export"))
                    {
                        var exportAttributeSymbol = semanticModel.GetSymbolInfo(exportAttribute);
                        if (exportAttributeSymbol.Symbol?.ContainingNamespace?.ToString() == "CustomCode.Core.Composition")
                        {
                            var lifetime = ParseLifetime(exportAttribute);
                            var typeSymbol = semanticModel.GetDeclaredSymbol(node);
                            if (typeSymbol == null)
                            {
                                // ToDo
                                return;
                            }

                            var interfaces = typeSymbol.AllInterfaces;
                            if (interfaces.Any())
                            {
                                var serviceType = ParseServiceType(exportAttribute);
                                if (serviceType == null)
                                {
                                    foreach (var @interface in interfaces)
                                    {
                                        DetectedServices.Add(new ExportedService(
                                            @interface.Name,
                                            @interface.ContainingNamespace.ToString(),
                                            typeSymbol.Name,
                                            typeSymbol.ContainingNamespace.ToString(),
                                            lifetime));
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
                                        lifetime));
                                }
                            }
                            else
                            {
                                DetectedServices.Add(new ExportedService(
                                    typeSymbol.Name,
                                    typeSymbol.ContainingNamespace.ToString(),
                                    typeSymbol.Name,
                                    typeSymbol.ContainingNamespace.ToString(),
                                    lifetime));
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

        /// <summary>
        /// Parse the "lifetime" parameter of the given <paramref name="exportAttribute"/>.
        /// </summary>
        /// <param name="exportAttribute"> The attribute whose "lifetime" parameter should be parsed. </param>
        /// <returns> The value of the given <paramref name="exportAttribute"/>'s "lifetime" parameter. </returns>
        private string? ParseLifetime(AttributeSyntax exportAttribute)
        {
            var arguments = exportAttribute.ArgumentList?.Arguments;
            var lifetimeValue = string.Empty;
            if (arguments?.Count == 1)
            {
                var name = arguments?[0].NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                var value = arguments?[0].Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                if ("lifetime".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    value.StartsWith("Lifetime.", StringComparison.OrdinalIgnoreCase))
                {
                    lifetimeValue = value;
                }
            }
            else if (arguments?.Count == 2)
            {
                var name = arguments?[0].NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                var value = arguments?[0].Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                if ("lifetime".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    value.StartsWith("Lifetime.", StringComparison.OrdinalIgnoreCase))
                {
                    lifetimeValue = value;
                }
                else
                {
                    name = arguments?[1].NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                    value = arguments?[1].Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                    if ("lifetime".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        value.StartsWith("Lifetime.", StringComparison.OrdinalIgnoreCase))
                    {
                        lifetimeValue = value;
                    }
                }
            }

            if (lifetimeValue.EndsWith(".Singleton", StringComparison.OrdinalIgnoreCase))
            {
                return "PerContainerLifetime";
            }
            if (lifetimeValue.EndsWith(".Scoped", StringComparison.OrdinalIgnoreCase))
            {
                return "PerScopeLifetime";
            }
            return null;
        }

        /// <summary>
        /// Parse the "serviceType" parameter of the given <paramref name="exportAttribute"/>.
        /// </summary>
        /// <param name="exportAttribute"> The attribute whose "serviceType" parameter should be parsed. </param>
        /// <returns> The value of the given <paramref name="exportAttribute"/>'s "serviceType" parameter. </returns>
        private string? ParseServiceType(AttributeSyntax exportAttribute)
        {
            var arguments = exportAttribute.ArgumentList?.Arguments;
            var serviceTypeValue = (string?)null;
            if (arguments?.Count == 1)
            {
                var name = arguments?[0].NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                var value = arguments?[0].Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                if ("serviceType".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    value.StartsWith("typeof(", StringComparison.OrdinalIgnoreCase))
                {
                    serviceTypeValue = value;
                }
            }
            else if (arguments?.Count == 2)
            {
                var name = arguments?[0].NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                var value = arguments?[0].Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                if ("serviceType".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    value.StartsWith("typeof(", StringComparison.OrdinalIgnoreCase))
                {
                    serviceTypeValue = value;
                }
                else
                {
                    name = arguments?[1].NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                    value = arguments?[1].Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                    if ("serviceType".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        value.StartsWith("typeof(", StringComparison.OrdinalIgnoreCase))
                    {
                        serviceTypeValue = value;
                    }
                }
            }

            if (serviceTypeValue != null && serviceTypeValue.EndsWith(")", StringComparison.OrdinalIgnoreCase))
            {
                return serviceTypeValue.Substring("typeof(".Length, serviceTypeValue.Length - "typeof(".Length - ")".Length);
            }
            return null;
        }

        #endregion
    }
}