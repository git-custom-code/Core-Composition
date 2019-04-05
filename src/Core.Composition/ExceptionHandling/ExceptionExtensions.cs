namespace CustomCode.Core.Composition.ExceptionHandling
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="Exception"/> type.
    /// </summary>
    public static class ExceptionExtensions
    {
        #region Logic

        /// <summary>
        /// Convert a (lighinject) <paramref name="exception"/> to an <see cref="UnresolvedDependencyException"/>
        /// -if possible- or null otherwise.
        /// </summary>
        /// <param name="exception"> The exception to be converted. </param>
        /// <returns> The converted exception or null. </returns>
        public static UnresolvedDependencyException? AsUnresolvedDependencyException(this Exception exception)
        {
            var invalidOperationException = ExtractUnresolvedDependencyException(exception);
            if (invalidOperationException != null)
            {
                var messagePart = ExtractMessagePart(invalidOperationException.Message, "Target Type");
                if (messagePart == null)
                {
                    return null;
                }

                var serviceType = ExtractTypeName(messagePart);
                var serviceNamespace = ExtractNamespace(messagePart);

                messagePart = ExtractMessagePart(invalidOperationException.Message, "Parameter");
                if (messagePart == null)
                {
                    return null;
                }

                var index = messagePart.IndexOf('(') + 1;
                messagePart = messagePart.Substring(index, messagePart.Length - 1 - index);
                var dependencyType = ExtractTypeName(messagePart);
                var dependencyNamespace = ExtractNamespace(messagePart);

                return new UnresolvedDependencyException(
                    dependencyNamespace,
                    dependencyType,
                    serviceNamespace,
                    serviceType,
                    invalidOperationException);
            }

            return null;
        }

        /// <summary>
        /// Extract the <see cref="InvalidOperationException"/> that represents an unresolved dependency from
        /// lightinject.
        /// </summary>
        /// <param name="exception"> The exception to act on. </param>
        /// <returns> The extracted unresolved dependency exception or null. </returns>
        private static InvalidOperationException? ExtractUnresolvedDependencyException(Exception exception)
        {
            var current = exception;
            while (current != null)
            {
                if (current is InvalidOperationException e)
                {
                    if (e.Message.StartsWith("Unresolved dependency", StringComparison.OrdinalIgnoreCase))
                    {
                        return e;
                    }
                }

                current = current.InnerException;
            }

            return null;
        }

        /// <summary>
        /// Extract a specific part from a lightinject exception message.
        /// </summary>
        /// <param name="message"> The message to act on. </param>
        /// <param name="partIdentifier"> The identifier of the part to be extracted. </param>
        /// <returns> The extracted part or null. </returns>
        private static string? ExtractMessagePart(string message, string partIdentifier)
        {
            var startIndex = message.IndexOf($"[{partIdentifier}: ");
            if (startIndex >= 0)
            {
                startIndex += partIdentifier.Length + 3;
                var endIndex = message.IndexOf("]]", startIndex);
                if (endIndex >= 0)
                {
                    return message.Substring(startIndex, endIndex - startIndex + 1);
                }

                endIndex = message.IndexOf("]", startIndex);
                if (endIndex >= 0)
                {
                    return message.Substring(startIndex, endIndex - startIndex);
                }
            }

            return null;
        }

        /// <summary>
        /// Extract a c# type name from a string.
        /// </summary>
        /// <param name="string"> The string to act on. </param>
        /// <returns> The extracted type name. </returns>
        private static string ExtractTypeName(string @string)
        {
            var startIndex = @string.LastIndexOf('.');
            if (startIndex >= 0)
            {
                var typeName = @string.Substring(startIndex + 1);
                startIndex = typeName.LastIndexOf('+');
                if (startIndex >= 0)
                {
                    typeName = typeName.Substring(startIndex + 1);
                }

                startIndex = typeName.LastIndexOf("`1[");
                if (startIndex >= 0)
                {
                    typeName = typeName.Replace("`1[", "<");
                    typeName = typeName.Replace("]", ">");
                }

                return typeName;
            }

            return @string;
        }

        /// <summary>
        /// Extract a c# namespace from a string.
        /// </summary>
        /// <param name="string"> The string to act on. </param>
        /// <returns> The extracted namespace. </returns>
        private static string ExtractNamespace(string @string)
        {
            var startIndex = @string.LastIndexOf('.');
            if (startIndex >= 0)
            {
                return @string.Substring(0, startIndex);
            }

            return "global::";
        }

        #endregion
    }
}