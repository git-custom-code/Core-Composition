namespace CustomCode.Analyzer.Core.Composition.Extensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Extension methods for the <see cref="SyntaxNode"/> type.
    /// </summary>
    public static class SyntaxNodeExtensions
    {
        #region Logic

        /// <summary>
        /// Find the closest parent node of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"> The type of the parent node to find. </typeparam>
        /// <param name="node"> The node to act on. </param>
        /// <returns> The closes parent node of type <typeparamref name="T"/> or null if no such node exists. </returns>
        public static SyntaxNode GetParent<T>(this SyntaxNode node) where T : SyntaxNode
        {
            if (node == null || node is T)
            {
                return node;
            }

            var parent = node.Parent;
            while (parent != null)
            {
                if (parent is T)
                {
                    break;
                }

                parent = parent.Parent;
            }

            return parent;
        }

        #endregion
    }
}