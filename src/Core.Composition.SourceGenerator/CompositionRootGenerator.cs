namespace CustomCode.Core.Composition.SourceGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    ///     public partial class CompositionRoot : ICompositionRoot
    ///     {
    ///         public void Compose(IServiceRegistry serviceRegistry)
    ///         {
    ///             serviceRegistry.Register<IFoo1, Foo1>(new PerRequestLifetime());
    ///             serviceRegistry.Register<IFoo2, Foo2>("serviceName", new PerContainerLifetime());
    ///             ...
    ///             serviceRegistry.Register<FooN>(new PerRequestLifetime());
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </example>
    [Generator]
    public sealed class CompositionRootGenerator : ISourceGenerator
    {
        #region Logic

        /// <inheritdoc />
        public void Execute(SourceGeneratorContext context)
        {
            try
            {
                var services = new List<ExportedService>();
                foreach (var syntaxTree in context.Compilation.SyntaxTrees)
                {
                    var serviceWalker = new ExportedServiceWalker(context.Compilation, syntaxTree);
                    serviceWalker.Visit(syntaxTree.GetRoot());
                    services.AddRange(serviceWalker.DetectedServices);
                }

                var code = CreateCompositionRoot(services);
                context.AddSource("CompositionRoot", SourceText.From(code, Encoding.UTF8));
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "CCSG002",
                        title: "Can't create ICompositionRoot implementation",
                        messageFormat: "CompositionRootGenerator: {0}",
                        category: "Core.Composition.SourceGenerator",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        description: "There was an unexpected exception creating the ICompositionRoot implementation"),
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

        private string CreateCompositionRoot(IEnumerable<ExportedService> services)
        {
            const string t = "    ";

            var code = new StringBuilder();
            code.AppendLine("namespace CustomCode.Core.GeneratedCode");
            code.AppendLine("{");

            // using block
            code.AppendLine($"{t}using LightInject;");
            var usings = services
                .Select(s => s.ServiceNamespace)
                .Union(services.Select(s => s.ImplementationNamespace))
                .Distinct();
            foreach (var @namespace in usings.OrderBy(n => n))
            {
                code.AppendLine($"{t}using {@namespace};");
            }
            code.AppendLine();

            // CompositionRoot type
            code.AppendLine($"{t}/// <summary>");
            code.AppendLine($"{t}/// Auto-generated <see cref=\"ICompositionRoot\"/> implementation.");
            code.AppendLine($"{t}/// </summary>");
            code.AppendLine($"{t}public partial class CompositionRoot : ICompositionRoot");
            code.AppendLine($"{t}{{");

            // Compose logic
            code.AppendLine($"{t}{t}#region Logic");
            code.AppendLine();
            code.AppendLine($"{t}{t}/// <inheritdoc />");
            code.AppendLine($"{t}{t}public void Compose(IServiceRegistry serviceRegistry)");
            code.AppendLine($"{t}{t}{{");
            foreach (var service in services.OrderBy(s => s.ImplementationName).ThenBy(s => s.ServiceName))
            {
                var genericPart = service.ImplementationName;
                if (!string.Equals(service.ServiceName, service.ImplementationName, StringComparison.OrdinalIgnoreCase))
                {
                    genericPart = $"{service.ServiceName}, {service.ImplementationName}";
                }

                var parameterPart = service.ServiceId;
                if (service.ServiceId == null && service.Lifetime != null)
                {
                    parameterPart = $"new {service.Lifetime}()";
                }
                else if (service.Lifetime != null)
                {
                    parameterPart = $"{service.ServiceId}, new {service.Lifetime}()";
                }

                code.AppendLine($"{t}{t}{t}serviceRegistry.Register<{genericPart}>({parameterPart});");
            }
            code.AppendLine($"{t}{t}}}");
            code.AppendLine();
            code.AppendLine($"{t}{t}#endregion");

            code.AppendLine($"{t}}}");

            code.AppendLine("}");
            return code.ToString();
        }

        #endregion
    }
}
