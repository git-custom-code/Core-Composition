namespace CustomCode.Core.Composition.LightInjectExtensions
{
    using LightInject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// A <see cref="IFactoryDelegateBuilder"/> implementation that uses <see cref="System.Linq.Expressions"/> to
    /// create compiled factory delegates for usage by LightInject's <see cref="ServiceContainer"/>.
    /// </summary>
    public sealed class FactoryDelegateBuilder : IFactoryDelegateBuilder
    {
        #region Dependencies

        /// <summary>
        /// Default ctor.
        /// </summary>
        public FactoryDelegateBuilder()
        {
            ServiceFactoryType = typeof(IServiceFactory);

            var methods = typeof(ServiceFactoryExtensions).GetTypeInfo().GetMethods();
            GetInstanceMethod = methods.FirstOrDefault(mi =>
                mi.Name == nameof(IServiceFactory.GetInstance) &&
                mi.GetGenericArguments().Count() == 1 &&
                mi.GetParameters().Count() == 1);
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the cached <see cref="IServiceFactory"/> type.
        /// </summary>
        private Type ServiceFactoryType { get; }

        /// <summary>
        /// Gets the cached <see cref="MethodInfo"/> for the <see cref="ServiceFactoryExtensions.GetInstance{TService}(IServiceFactory)"/> method.
        /// </summary>
        private MethodInfo GetInstanceMethod { get; }

        #endregion

        #region Logic

        /// <summary>
        /// Create a new dynamically compiled factory delegate for a given <paramref name="type"/>,
        /// if- and only if- one of the type's constructors is marked with a <see cref="FactoryParametersAttribute"/>.
        /// </summary>
        /// <param name="type"> The type that should be created via a factory. </param>
        /// <returns> A delegate that can create a new instance of the specified <paramref name="type"/>. </returns>
        public Delegate? CreateFactoryFor(TypeInfo type)
        {
            foreach (var ctor in type.DeclaredConstructors)
            {
                var factoryData = ctor.GetCustomAttribute<FactoryParametersAttribute>();
                if (factoryData != null)
                {
                    var paramInfos = ctor.GetParameters();
                    if (factoryData.ParameterNames == null)
                    {
                        return CreateFactoryDelegate(ctor, paramInfos);
                    }
                    else
                    {
                        return CreateFactoryDelegate(ctor, factoryData.ParameterNames, paramInfos);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Create a delegate that is able to call the specified <paramref name="ctor"/>.
        /// </summary>
        /// <param name="ctor"> The constructor to be called by the factory delegate. </param>
        /// <param name="paramInfos"> The <paramref name="ctor"/>'s parameters. </param>
        /// <returns> A delegate that is able to call the specified <paramref name="ctor"/>. </returns>
        private Delegate CreateFactoryDelegate(ConstructorInfo ctor, ParameterInfo[] paramInfos)
        {
            var ctorArgs = new List<ParameterExpression>(paramInfos.Length);
            var factoryArgs = new List<ParameterExpression>(paramInfos.Length + 1);
            for (var i = 0; i < paramInfos.Length; ++i)
            {
                ctorArgs.Add(Expression.Parameter(paramInfos[i].ParameterType, $"arg{i}"));
            }
            var factory = Expression.Parameter(ServiceFactoryType, "factory");
            factoryArgs.Add(factory);
            factoryArgs.AddRange(ctorArgs);
            var lambda = Expression.Lambda(Expression.New(ctor, ctorArgs), factoryArgs);
            return lambda.Compile();
        }

        /// <summary>
        /// Create a delegate that is able to call the specified <paramref name="ctor"/>.
        /// </summary>
        /// <param name="ctor"> The constructor to be called by the factory delegate. </param>
        /// <param name="parameterNames"> The <see cref="FactoryParametersAttribute"/>'s parameter names specified by the developer. </param>
        /// <param name="paramInfos"> The <paramref name="ctor"/>'s parameters. </param>
        /// <returns> A delegate that is able to call the specified <paramref name="ctor"/>. </returns>
        private Delegate CreateFactoryDelegate(ConstructorInfo ctor, string[] parameterNames, ParameterInfo[] paramInfos)
        {
            var ctorArgs = new List<Expression>(paramInfos.Length);
            var factoryArgs = new List<ParameterExpression>(parameterNames.Length);
            var factoryArgsLut = new HashSet<string>(parameterNames.Select(n => n.ToLowerInvariant()));
            var factory = Expression.Parameter(ServiceFactoryType, "factory");
            factoryArgs.Add(factory);

            for (var i = 0; i < paramInfos.Length; ++i)
            {
                if (factoryArgsLut.Contains(paramInfos[i].Name.ToLowerInvariant()))
                {
                    var param = Expression.Parameter(paramInfos[i].ParameterType, $"arg{i}");
                    ctorArgs.Add(param);
                    factoryArgs.Add(param);
                }
                else
                {
                    var info = GetInstanceMethod.MakeGenericMethod(paramInfos[i].ParameterType);
                    ctorArgs.Add(Expression.Call(info, factory));
                }
            }
            var lambda = Expression.Lambda(Expression.New(ctor, ctorArgs), factoryArgs);
            return lambda.Compile();
        }

        #endregion
    }
}