using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace RulesEngine
{

    public class Engine : IRegisterInvoker
    {
        //Create a default ExpressionCache, ignoring lambda parameter names.
        private readonly InvokerRegistry _registry = new InvokerRegistry();
        private IErrorResolver _defaultErrorResolver;
        private Engine _parent;

        public Engine()
        {
            _defaultErrorResolver = new DefaultErrorResolver();
        }

        /// <summary>
        /// Creates a Rules Engine.
        /// </summary>
        /// <param name="basedOn">Copies rules from base Engine</param>
        public Engine(Engine basedOn)
        {
            if (basedOn == null) throw new System.ArgumentNullException("basedOn");
            _registry = basedOn._registry.Clone();
            _defaultErrorResolver = basedOn._defaultErrorResolver.Clone();
        }

        /// <summary>
        /// Creates a Rules Engine.
        /// </summary>
        /// <param name="basedOn">Copies specific rules from base Engine</param>
        /// <param name="types">Copies rules for the specified types only.</param>
        public Engine(Engine basedOn, params Type[] types)
        {
            if (types == null) throw new System.ArgumentNullException("types");
            if (basedOn == null) throw new System.ArgumentNullException("basedOn");
            _defaultErrorResolver = basedOn._defaultErrorResolver.Clone();
            var registry = basedOn._registry.Clone();

            foreach (var type in types)
            {
                var invokers = registry.GetInvokers(type);
                foreach (var invoker in invokers)
                {
                    _registry.RegisterInvoker(invoker);
                }
            }
        }

        public ForClass<T> For<T>()
        {
            return new ForClass<T>(this);
        }


        public void Validate(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (value == null) return;
            foreach (var invoker in _registry.GetInvokers(value.GetType()))
            {
                invoker.Invoke(value, report, depth);
                if (report.HasErrors && depth == ValidationReportDepth.ShortCircuit)
                {
                    return;
                }
            }
        }

        public bool Validate(object value)
        {
            var validationReport = new QuickValidationReport();
            Validate(value, validationReport, ValidationReportDepth.ShortCircuit);
            return !validationReport.HasErrors;
        }

        void IRegisterInvoker.RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            _registry.RegisterInvoker(ruleInvoker);
        }

        Engine IRegisterInvoker.RulesEngine
        {
            get { return this; }
        }

        /// <summary>
        /// Gets or Sets DefaultErrorResolver
        /// </summary>
        public IErrorResolver DefaultErrorResolver
        {
            get { return _defaultErrorResolver; }
            set 
            {
                if (value == null) throw new ArgumentNullException();
                _defaultErrorResolver = value; 
            }
        }

        /// <summary>
        /// Gets or Sets Parent
        /// </summary>
        internal Engine Parent
        {
            get { return _parent; }
            set 
            {
                _parent = value; 
            }
        }

        internal Engine Original
        {
            get
            {
                Engine result = this;
                while (result.Parent != null)
                {
                    result = result.Parent;
                }
                return result;
            }
        }
    }
}
