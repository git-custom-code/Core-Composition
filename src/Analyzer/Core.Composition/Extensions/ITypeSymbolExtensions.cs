namespace CustomCode.Analyzer.Core.Composition.Extensions
{
    using Microsoft.CodeAnalysis;
    using System.Text;

    /// <summary>
    /// Extension methods for the <see cref="ITypeSymbol"/> type.
    /// </summary>
    public static class ITypeSymbolExtensions
    {
        #region Logic

        /// <summary>
        /// Get the symbol's full namespace.
        /// </summary>
        /// <param name="symbol"> The symbol whose namespace should be returned. </param>
        /// <returns> The symbol's full namespace or null. </returns>
        public static string? GetNamespace(this ITypeSymbol symbol)
        {
            if (symbol == null)
            {
                return null;
            }

            var @namespace = new StringBuilder();
            var current = symbol.ContainingNamespace;
            if (current.IsGlobalNamespace)
            {
                return null;
            }

            while (current != null && current.IsGlobalNamespace == false)
            {
                @namespace.Insert(0, $"{current.Name}.");
                current = current.ContainingNamespace;
            }

            return @namespace.ToString(0, @namespace.Length - 1);
        }

        #endregion
    }
}