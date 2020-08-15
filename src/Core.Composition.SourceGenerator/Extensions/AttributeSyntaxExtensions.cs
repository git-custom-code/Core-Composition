namespace CustomCode.Core.Composition.SourceGenerator.Extensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;

    /// <summary>
    /// Extension methods for the <see cref="AttributeSyntax"/> type.
    /// </summary>
    public static class AttributeSyntaxExtensions
    {
        #region Logic

        /// <summary>
        /// Parse the "lifetime" parameter of the given <paramref name="exportAttribute"/>.
        /// </summary>
        /// <param name="exportAttribute"> The attribute whose "lifetime" parameter should be parsed. </param>
        /// <returns> The value of the given <paramref name="exportAttribute"/>'s "lifetime" parameter. </returns>
        public static string? ParseLifetime(this AttributeSyntax exportAttribute)
        {
            var lifetimeValue = string.Empty;
            if (exportAttribute.ArgumentList != null)
            {
                var arguments = exportAttribute.ArgumentList.Arguments;
                foreach (var argument in arguments)
                {
                    var name = argument.NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                    var value = argument.Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                    if ("lifetime".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        (!"serviceName".Equals(name, StringComparison.OrdinalIgnoreCase) &&
                        value.StartsWith("Lifetime.", StringComparison.OrdinalIgnoreCase)))
                    {
                        lifetimeValue = value;
                        break;
                    }
                }
            }

            if (lifetimeValue.EndsWith("Lifetime.Singleton", StringComparison.OrdinalIgnoreCase))
            {
                return "PerContainerLifetime";
            }
            if (lifetimeValue.EndsWith("Lifetime.Scoped", StringComparison.OrdinalIgnoreCase))
            {
                return "PerScopeLifetime";
            }
            return null;
        }

        /// <summary>
        /// Parse the optional "ServiceName" parameter of the given <paramref name="exportAttribute"/>.
        /// </summary>
        /// <param name="exportAttribute"> The attribute whose "ServiceName" parameter should be parsed. </param>
        /// <returns> The value of the given <paramref name="exportAttribute"/>'s "ServiceName" parameter. </returns>
        public static string? ParseServiceName(this AttributeSyntax exportAttribute)
        {
            if (exportAttribute.ArgumentList != null)
            {
                var arguments = exportAttribute.ArgumentList.Arguments;
                foreach (var argument in arguments)
                {
                    var name = argument.NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                    var value = argument.Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                    if ("serviceName".Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Parse the "serviceType" parameter of the given <paramref name="exportAttribute"/>.
        /// </summary>
        /// <param name="exportAttribute"> The attribute whose "serviceType" parameter should be parsed. </param>
        /// <returns> The value of the given <paramref name="exportAttribute"/>'s "serviceType" parameter. </returns>
        public static string? ParseServiceType(this AttributeSyntax exportAttribute)
        {
            var serviceTypeValue = (string?)null;
            if (exportAttribute.ArgumentList != null)
            {
                var arguments = exportAttribute.ArgumentList.Arguments;
                foreach (var argument in arguments)
                {
                    var name = argument.NameEquals?.Name.Identifier.ValueText ?? string.Empty;
                    var value = argument.Expression?.NormalizeWhitespace().ToFullString() ?? string.Empty;

                    if ("serviceType".Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        (!"serviceName".Equals(name, StringComparison.OrdinalIgnoreCase) &&
                        value.StartsWith("typeof(", StringComparison.OrdinalIgnoreCase)))
                    {
                        serviceTypeValue = value;
                        break;
                    }
                }
            }

            if (serviceTypeValue != null && serviceTypeValue.EndsWith(")", StringComparison.OrdinalIgnoreCase))
            {
                return serviceTypeValue
                    .Substring("typeof(".Length, serviceTypeValue.Length - "typeof(".Length - ")".Length)
                    .Trim();
            }
            return null;
        }

        #endregion
    }
}
