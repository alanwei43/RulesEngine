using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;
using RulesEngine.Fluent;

namespace RulesEngine
{
    public interface IEngine
    {
        IErrorResolver ErrorResolver { get; }
        IInvokerRegistry InvokerRegistry { get; }

        void Validate(object value, IValidationReport report, ValidationReportDepth depth);
        void Validate(object value, IValidationReport report);
        bool Validate(object value);
    }

    public class Engine : IEngine
    {
        //Create a default ExpressionCache, ignoring lambda parameter names.
        private readonly IInvokerRegistry _invokerRegistry = new InvokerRegistry();
        private IErrorResolver _errorResolver;
        private Engine _parent;

        public Engine(IInvokerRegistry invokerRegistry, IErrorResolver errorResolver)
        {
            if (invokerRegistry == null) throw new System.ArgumentNullException("invokerRegistry");
            if (errorResolver == null) throw new System.ArgumentNullException("errorResolver");
            _errorResolver = errorResolver;
            _invokerRegistry = invokerRegistry;
        }

        public void Validate(object value, IValidationReport report)
        {
            Validate(value, report, ValidationReportDepth.ShortCircuit);
        }

        public void Validate(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (value == null) return;
            foreach (var invoker in _invokerRegistry.GetInvokers(value.GetType()))
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

        /// <summary>
        /// Gets or Sets DefaultErrorResolver
        /// </summary>
        public IErrorResolver DefaultErrorResolver
        {
            get { return _errorResolver; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _errorResolver = value;
            }
        }

        /// <summary>
        /// Gets InvokerRegistry
        /// </summary>
        public IInvokerRegistry InvokerRegistry
        {
            get { return _invokerRegistry; }
        }

        /// <summary>
        /// Gets ErrorResolver
        /// </summary>
        public IErrorResolver ErrorResolver
        {
            get { return _errorResolver; }
        }
    }
}
