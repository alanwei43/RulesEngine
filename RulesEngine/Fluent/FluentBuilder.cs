using System;
using System.Collections.Generic;
using System.Linq;

namespace RulesEngine.Fluent
{
    public class FluentBuilder : IEngineBuilder
    {
        private readonly List<FluentScope> _forScopes = new List<FluentScope>();
        private IValueResolverFactory _valueResolverFactory = new DefaultValueResolverFactory();
        private ICulpritResolverFactory _culpritResolverFactory = new DefaultCulpritResolverFactory();

        /// <summary>
        /// Gets or Sets ValueResolverFactory
        /// </summary>
        public IValueResolverFactory ValueResolverFactory
        {
            get { return _valueResolverFactory; }
            set { _valueResolverFactory = value; }
        }

        /// <summary>
        /// Gets or Sets CulpritResolverFactory
        /// </summary>
        public ICulpritResolverFactory CulpritResolverFactory
        {
            get { return _culpritResolverFactory; }
            set
            {
                _culpritResolverFactory = value;
            }
        }

        public IEngine Build()
        {
            return Build(null);
        }

        public IEngine Build(FluentBuilder baseBuilder, params Type[] types)
        {
            //TODO: Change the line below - should be using a factory of sorts to create the InvokerRegisytry and the ErrorResolber?
            //Pass the factory to the BuilderToken so it can create conditional engines down the track.
            var invokerRegistry = (baseBuilder == null) ? new InvokerRegistry() : CreateInvokerRegistry(baseBuilder.Build().InvokerRegistry, types);
            var errorResolver = (baseBuilder == null) ? new DefaultErrorResolver() : baseBuilder.Build().ErrorResolver;

            var token = new FluentBuilderToken(
                new Engine(invokerRegistry, errorResolver)
                , _valueResolverFactory ?? new DefaultValueResolverFactory()
                , _culpritResolverFactory ?? new DefaultCulpritResolverFactory()
                );

            foreach (var root in _forScopes)
            {
                var tmp = root;
                while (tmp != null)
                {
                    var fluentScopeAction = tmp.Get(RegistryKeys.FluentScopeAction, null);
                    if (fluentScopeAction != null)
                    {
                        fluentScopeAction.Execute(tmp, token);
                    }
                    tmp = tmp.Child;
                }
            }

            return token.RootEngine;
        }

        private IInvokerRegistry CreateInvokerRegistry(IInvokerRegistry baseRegistry, Type[] types)
        {
            var result = new InvokerRegistry();
            var includeAll = (types == null) || (types.Length == 0);

            foreach (var invoker in baseRegistry.GetInvokers())
            {
                if (includeAll || types.Contains(invoker.ParameterType))
                {
                    result.RegisterInvoker(invoker);
                }
            }

            return result;
        }

        public ForClass<T> For<T>()
        {
            var tmpScope = new FluentScope();

            tmpScope.Set(RegistryKeys.SourceType, typeof(T));


            var result = new ForClass<T>(tmpScope);
            _forScopes.Add(tmpScope);
            return result;
        }

    }
    
}
