namespace CustomCode.Core.Composition.SourceGenerator.Extensions
{
    using Microsoft.CodeAnalysis;
    using System;

    /// <summary>
    /// Extension methods for the <see cref="Compilation"/> type.
    /// </summary>
    public static class CompilationExtensions
    {
        #region Logic

        /// <summary>
        /// Query if a given <paramref name="compilation"/> has referenced the "Core.Composition" assembly.
        /// </summary>
        /// <param name="compilation"> The roslyn <see cref="Compilation"/> whose references should be checked. </param>
        /// <returns> True if the "Core.Composition" assembly is referenced, false otherwise. </returns>
        public static bool IsCoreCompositonReferenced(this Compilation compilation)
        {
            var references = compilation.References;
            foreach (var reference in references)
            {
                var assembly = compilation.GetAssemblyOrModuleSymbol(reference);
                if (assembly?.Kind == SymbolKind.Assembly &&
                    LanguageNames.CSharp.Equals(assembly?.Language, StringComparison.OrdinalIgnoreCase) &&
                    "Core.Composition".Equals(assembly?.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}