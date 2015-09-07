using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine
{
    internal class QuickValidationReport : IValidationReport
    {
        private bool _hasError = false;

        public bool HasError<T, R>(T value, Expression<Func<T, R>> expression, out ValidationError[] validationErrors)
        {
            throw new NotImplementedException();
        }

        public bool HasError(object value, EquatableExpression expression)
        {
            throw new NotImplementedException();
        }

        public void AddError(ValidationError validationError)
        {
            _hasError = true;
        }

        public bool HasErrors
        {
            get { return _hasError; }
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
