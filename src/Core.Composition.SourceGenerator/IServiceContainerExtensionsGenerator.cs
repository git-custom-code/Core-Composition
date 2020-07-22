namespace CustomCode.Core.Composition.SourceGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    [Generator]
    public sealed class IServiceContainerExtensionsGenerator : ISourceGenerator
    {
        #region Logic

        /// <inheritdoc />
        public void Execute(SourceGeneratorContext context)
        {
            try
            {
                const string t = "    ";

                var code = new StringBuilder();
                code.AppendLine("namespace CustomCode.Core.GeneratedCode");
                code.AppendLine("{");
                code.AppendLine($"{t}using LightInject;");
                code.AppendLine();

                code.AppendLine($"{t}public static class IServiceContainerExtensions");
                code.AppendLine($"{t}{{");

                code.AppendLine($"{t}{t}public static void Register<TService, TImplementation>(this IServiceRegistry serviceRegistry, string serviceName)");
                code.AppendLine($"{t}{t}{t}where TImplementation : TService");
                code.AppendLine($"{t}{t}{{");
                code.AppendLine($"{t}{t}{t}serviceRegistry.Register<TService ,TImplementation>(serviceName, new PerRequestLifeTime());");
                code.AppendLine($"{t}{t}}}");

                code.AppendLine($"{t}{t}public static void Register<TService>(this IServiceRegistry serviceRegistry, string serviceName)");
                code.AppendLine($"{t}{t}{{");
                code.AppendLine($"{t}{t}{t}serviceRegistry.Register<TService, TService>(serviceName);");
                code.AppendLine($"{t}{t}}}");

                code.AppendLine($"{t}{t}public static void Register<TService>(this IServiceRegistry serviceRegistry, string serviceName, ILifetime lifetime)");
                code.AppendLine($"{t}{t}{{");
                code.AppendLine($"{t}{t}{t}serviceRegistry.Register<TService, TService>(serviceName, lifetime);");
                code.AppendLine($"{t}{t}}}");

                code.AppendLine($"{t}}}");
                code.AppendLine("}");

                var sourceCode = code.ToString();
                context.AddSource("IServiceContainerExtensions", SourceText.From(sourceCode, Encoding.UTF8));
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "CCSG003",
                        title: "Can't create IServiceContainerExtensions implementation",
                        messageFormat: "IServiceContainerExtensionsGenerator: {0}",
                        category: "Core.Composition.SourceGenerator",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        description: "There was an unexpected exception creating the IServiceContainerExtensions implementation"),
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