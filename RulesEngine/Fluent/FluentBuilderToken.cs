using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RulesEngine.Fluent
{
    internal class FluentBuilderToken
    {

        //Tuple<TrueEngine, FalseEngine, CurrentEngine>
        private readonly Stack<ConditionalEngineBuilderToken> _conditionalEngineTokens = new Stack<ConditionalEngineBuilderToken>();
        private readonly IEngine _rootEngine;
        private IEngine _currentEngine;
        private readonly IValueResolverFactory _valueResolverFactory;
        private readonly ICulpritResolverFactory _culpritResolverFactory;

        /// <summary>
        /// Gets RootEngine
        /// </summary>
        public IEngine RootEngine
        {
            get { return _rootEngine; }
        }
        /// <summary>
        /// Gets CurrentEngine
        /// </summary>
        public IEngine CurrentEngine
        {
            get { return _currentEngine; }
        }

        /// <summary>
        /// Gets ErrorResolver
        /// </summary>
        public IErrorResolver ErrorResolver
        {
            get { return _rootEngine.ErrorResolver; }
        }

        /// <summary>
        /// Gets ValueResolverFactory
        /// </summary>
        public IValueResolverFactory ValueResolverFactory
        {
            get { return _valueResolverFactory; }
        }

        /// <summary>
        /// Gets CulpritResolverFactory
        /// </summary>
        public ICulpritResolverFactory CulpritResolverFactory
        {
            get { return _culpritResolverFactory; }
        }

        public FluentBuilderToken(IEngine rootEngine, IValueResolverFactory valueResolverFactory, ICulpritResolverFactory culpritResolverFactory)
        {
            if (culpritResolverFactory == null) throw new ArgumentNullException("culpritResolverFactory");
            if (valueResolverFactory == null) throw new ArgumentNullException("valueResolverFactory");
            if (rootEngine == null) throw new ArgumentNullException("rootEngine");
            _rootEngine = rootEngine;
            _currentEngine = rootEngine;
            _valueResolverFactory = valueResolverFactory;
            _culpritResolverFactory = culpritResolverFactory;
        }

        public ConditionalEngineBuilderToken Condition()
        {
            var ifTrueEngine = new Engine(new InvokerRegistry(), ErrorResolver);
            var ifFalseEngine = new Engine(new InvokerRegistry(), ErrorResolver);
            var result = new ConditionalEngineBuilderToken(ifTrueEngine, ifFalseEngine, ifTrueEngine);
            _conditionalEngineTokens.Push(result);
            //Set CurrentEngine to the TrueEngine
            _currentEngine = ifTrueEngine;
            return result;
        }

        public IEngine Else()
        {
            //Set CurrentEngine to the FalseEngine
            var record = _conditionalEngineTokens.Peek();
            record.CurrentEngine = record.IfFalseEngine;
            _currentEngine = record.CurrentEngine;
            return _currentEngine;
        }

        public IEngine Endif()
        {
            _conditionalEngineTokens.Pop();
            if (_conditionalEngineTokens.Any())
            {
                //Set CurrentEngine to the top of the stack's CurrentEngine.
                var record = _conditionalEngineTokens.Peek();
                _currentEngine = record.CurrentEngine;
            }
            else
            {
                _currentEngine = _rootEngine;
            }
            return _currentEngine;
        }
    }
}
