using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace RulesEngine.Tests
{
    public class TestingValidationReport : IValidationReport
    {
        List<ValidationError> _errors = new List<ValidationError>();
        IEngine _engine;

        public bool HasError<T, R>(T value, System.Linq.Expressions.Expression<Func<T, R>> expression, out ValidationError[] validationErrors)
        {
            throw new NotImplementedException();
        }

        public void AssertError<T, R>(T value, Expression<Func<T, R>> expression, string ruleKind, params object[] validationArguments)
        {
            var cachedExp = new EquatableExpression(expression);
            var hasError =  _errors.Any(e => e.Expression == cachedExp 
                                && e.Rule.RuleKind == ruleKind 
                                && object.ReferenceEquals(e.Value, value)
                                && (validationArguments == null || AreEquivalent(e.ValidationArguments, validationArguments))
                                );
            if (!hasError) throw new AssertFailedException(string.Format("Expected rule failure for rulekind {0}, expression {1}.", ruleKind, expression));
        }

        public bool HasError(object value, EquatableExpression expression)
        {
            return _errors.Any(e => e.Expression == expression && object.ReferenceEquals(e.Value, value));
        }

        private bool AreEquivalent(object[] value1, object[] value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;
            if (value1.Length != value2.Length) return false;
            for (int i = 0; i < value1.Length; i++)
            {
                if (value1[i] == null && value2[i] == null) continue;
                if (value1[i] == null || value2[i] == null) return false;
                if (!value1[i].Equals(value2[i])) return false;
            }
            return true;
        }

        public void AddError(ValidationError validationError)
        {
            _errors.Add(validationError);
        }

        public bool HasErrors
        {
            get { return _errors.Any(); }
        }

        /// <summary>
        /// Gets Errors
        /// </summary>
        public ValidationError[] Errors
        {
            get { return _errors.ToArray(); }
        }

        public TestingValidationReport(IEngine engine)
        {
            if (engine == null) throw new System.ArgumentNullException("engine");
            _engine = engine;
        }

        public bool Validate(object value)
        {
            Clear();
            _engine.Validate(value, this, ValidationReportDepth.FieldShortCircuit);
            return !this.HasErrors;
        }

        public void Clear()
        {
            _errors.Clear();
        }


        public string GetErrorMessage<T, R>(T value, Expression<Func<T, R>> expression, IErrorResolver resolver)
        {
            throw new NotImplementedException();
        }

        public string GetErrorMessage(object value, EquatableExpression expression, IErrorResolver resolver)
        {
            throw new NotImplementedException();
        }
    }
}
