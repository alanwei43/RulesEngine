using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace RulesEngine.Mvc
{
    public class MvcValidationReport : IValidationReport
    {
        private ValidationReport _innerReport;
        private ModelStateDictionary _modelState;
        private IErrorResolver _errorResolver;
        private Engine _engine;

        public bool HasError<T, R>(T value, System.Linq.Expressions.Expression<Func<T, R>> expression, out ValidationError[] validationErrors)
        {
            return _innerReport.HasError<T, R>(value, expression, out validationErrors);
        }

        public bool HasError(object value, EquatableExpression expression)
        {
            return _innerReport.HasError(value, expression);
        }

        public string GetErrorMessage<T, R>(T value, System.Linq.Expressions.Expression<Func<T, R>> expression, IErrorResolver resolver)
        {
            return _innerReport.GetErrorMessage<T, R>(value, expression, resolver);
        }

        public void AddError(ValidationError validationError)
        {
            _innerReport.AddError(validationError);
            var visitor = new ProperyNameVisitor();
            visitor.Visit(validationError.Expression.Expression);

            if (visitor.PropertyName == null) return;

            var errorMessage = _innerReport.GetErrorMessage(validationError.Value, validationError.Expression, _errorResolver);
            var modelDictionary = _modelState[visitor.PropertyName];

            if (modelDictionary == null) return;

            //NOTE: Add errorMessage even when errorMessage is null.
            var errorCollection = modelDictionary.Errors;
            //Clear errorCollection (in case of re-use).
            errorCollection.Clear();
            errorCollection.Add(errorMessage);
        }

        public bool HasErrors
        {
            get { return _innerReport.HasErrors; }
        }

        public string[] GetErrorMessages(object value)
        {
            return _innerReport.GetErrorMessages(value);
        }

        public string[] GetErrorMessages(object value, IErrorResolver errorResolver)
        {
            return _innerReport.GetErrorMessages(value, errorResolver);
        }
        
        public bool Validate(object value)
        {
            if (_engine == null) throw new InvalidOperationException("Engine is null. Use the overloaded constructor to pass one in.");
            //Always re-create  _innerReport (just in case MvcValidationReport is used multiple times)
            CreateInnerReport();
            _engine.Validate(value, this, ValidationReportDepth.FieldShortCircuit);
            return !this.HasErrors;
        }


        public MvcValidationReport(ModelStateDictionary modelState, IErrorResolver errorResolver)
        {
            if (errorResolver == null) throw new System.ArgumentNullException("errorResolver");
            if (modelState == null) throw new System.ArgumentNullException("modelState");
            Initialize(modelState, null, errorResolver);
        }

        public MvcValidationReport(ModelStateDictionary modelState, Engine engine, IErrorResolver errorResolver)
        {
            if (errorResolver == null) throw new System.ArgumentNullException("errorResolver");
            if (engine == null) throw new System.ArgumentNullException("engine");
            if (modelState == null) throw new System.ArgumentNullException("modelState");
            Initialize(modelState, engine, errorResolver);
        }

        public MvcValidationReport(ModelStateDictionary modelState, Engine engine)
        {
            if (engine == null) throw new System.ArgumentNullException("engine");
            if (modelState == null) throw new System.ArgumentNullException("modelState");
            Initialize(modelState, engine, null);
        }

        private void Initialize(ModelStateDictionary modelState, Engine engine, IErrorResolver errorResolver)
        {
            _modelState = modelState;
            _engine = engine;

            //NOTE: errorResolver and engine cannot be null at the same time (see constructors)
            if (errorResolver == null)
                _errorResolver = engine.DefaultErrorResolver;
            else
                _errorResolver = errorResolver;

            CreateInnerReport();
        }

        private void CreateInnerReport()
        {
            if (_engine == null)
                _innerReport = new ValidationReport();
            else
                _innerReport = new ValidationReport(_engine);
        }

    }
}
