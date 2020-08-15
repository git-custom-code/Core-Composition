namespace CustomCode.Core.Composition.SourceGenerator
{
    using Extensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Text;

    /// <summary>
    /// Implementation of an <see cref="ISourceGenerator"/> that is used to generate a couple of extension
    /// methods for the "LightInject.IServiceRegistry" interface that will be used by the
    /// generated code of the <see cref="CompositionRootGenerator"/>.
    /// </summary>
    /// <example>
    /// This SourceGenerator will generate the following code:
    /// <![CDATA[
    /// namespace CustomCode.Core.GeneratedCode
    /// {
    ///     using LightInject;
    ///
    ///     public static class IServiceRegistryExtensions
    ///     {
    ///         public static void Register<TService, TImplementation>(this IServiceRegistry serviceRegistry, string serviceName)
    ///             where TImplementation : TService
    ///         {
    ///             serviceRegistry.Register<TService ,TImplementation>(serviceName, new PerRequestLifeTime());
    ///         }
    ///
    ///         public static void Register<TService>(this IServiceRegistry serviceRegistry, string serviceName)
    ///         {
    ///             serviceRegistry.Register<TService, TService>(serviceName);
    ///         }
    ///
    ///         public static void Register<TService>(this IServiceRegistry serviceRegistry, string serviceName, ILifetime lifetime)
    ///         {
    ///             serviceRegistry.Register<TService, TService>(serviceName, lifetime);
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </example>
    [Generator]
    public sealed class IServiceRegistryExtensionsGenerator : ISourceGenerator
    {
        #region Logic

        /// <inheritdoc />
        public void Execute(SourceGeneratorContext context)
        {
            try
            {
                if (context.Compilation.IsCoreCompositonReferenced())
                {
                    var code = CreateIServiceContainerExtensions();
                    context.AddSource("IServiceRegistryExtensions", SourceText.From(code, Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "CCSG003",
                        title: "Can't create IServiceRegistryExtensions implementation",
                        messageFormat: "IServiceRegistryExtensionsGenerator: {0}",
                        category: "Core.Composition.SourceGenerator",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        description: "There was an unexpected exception creating the IServiceRegistryExtensions implementation"),
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

        private string CreateIServiceContainerExtensions()
        {
            const string t = "    ";

            var code = new StringBuilder();
            code.AppendLine("namespace CustomCode.Core.GeneratedCode");
            code.AppendLine("{");
            code.AppendLine($"{t}using LightInject;");
            code.AppendLine();

            code.AppendLine($"{t}/// <summary>");
            code.AppendLine($"{t}/// Auto-generated extension methods for the <see cref=\"IServiceRegistry\"/> type.");
            code.AppendLine($"{t}/// </summary>");
            code.AppendLine($"{t}public static class IServiceRegistryExtensions");
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

            return code.ToString();
        }

        #endregion
    }
}
